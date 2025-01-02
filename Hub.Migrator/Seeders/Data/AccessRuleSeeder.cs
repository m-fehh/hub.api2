using Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hub.Migrator.Seeders.Data
{
    public class AccessRuleSeeder : ISeeder
    {
        public int Order => 2;

        public async Task SeedAsync(EntityDbContext dbContext)
        {
            // Inicial
            await CreateAccessRulesAsync("CB", "Basic Registration", false, null, dbContext);

            // User
            await CreateAccessRulesAsync("US", "User", false, null, dbContext);

            // System
            await CreateAccessRulesAsync("SYS", "System", false, null, dbContext);

            // PortalUser
            await CreateAccessRulesAsync("US_User", "PortalUser", true, "US", dbContext);

            // ProfileGroup
            await CreateAccessRulesAsync("CB_ProfileGroup", "ProfileGroup", true, "US", dbContext);

            // DocumentType
            await CreateAccessRulesAsync("CB_DocumentType", "DocumentType", true, "CB", dbContext);

            // Logs
            await CreateAccessRulesAsync("SYS_Logs", "Logs", true, "SYS", dbContext);
        }

        private async Task CreateAccessRulesAsync(string baseKeyName, string description, bool addOperations, string parentKey, EntityDbContext dbContext)
        {
            // Verificar se a regra raiz já existe
            var rootExists = await dbContext.AccessRules.AnyAsync(x => x.KeyName == baseKeyName);
            if (rootExists) return;

            // Criar a regra raiz
            var rootAccessRule = new AccessRule
            {
                ParentId = (parentKey != null) ? (await GetAccessRulesByKeyNameAsync(parentKey, dbContext)).Id : null,
                Description = description,
                KeyName = baseKeyName,
            };

            dbContext.AccessRules.Add(rootAccessRule);
            await dbContext.SaveChangesAsync();

            // Gerar a árvore para a regra raiz e atualizar o campo Tree
            rootAccessRule.Tree = GenerateTree(rootAccessRule.Id, dbContext);
            dbContext.AccessRules.Update(rootAccessRule);
            await dbContext.SaveChangesAsync();

            if (addOperations)
            {
                // Operações padrão
                var operations = new[]
                {
                    ("View", "Vis"),
                    ("Insert", "Ins"),
                    ("Edit", "Upd"),
                    ("Delete", "Del"),
                    ("Export", "Exp")
                };

                foreach (var (operationDescription, operationSuffix) in operations)
                {
                    var operationKeyName = $"{baseKeyName}_{operationSuffix}";

                    // Verificar se a regra filha já existe
                    var exists = await dbContext.AccessRules.AnyAsync(x => x.KeyName == operationKeyName);
                    if (exists) continue;

                    // Criar a regra filha
                    var childAccessRule = new AccessRule
                    {
                        ParentId = rootAccessRule.Id,
                        Description = operationDescription,
                        KeyName = operationKeyName
                    };

                    dbContext.AccessRules.Add(childAccessRule);
                }

                await dbContext.SaveChangesAsync();

                // Filtrando os filhos da regra raiz
                var childRules = await dbContext.AccessRules
                    .Where(x => x.ParentId == rootAccessRule.Id)
                    .ToListAsync();

                foreach (var childRule in childRules)
                {
                    // Gerando e atualizando a árvore de cada regra filha
                    childRule.Tree = GenerateTree(childRule.Id, dbContext);
                    dbContext.AccessRules.Update(childRule);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        // Método para gerar a árvore
        public string GenerateTree(long ruleId, EntityDbContext dbContext)
        {
            // Inicia com a ID da regra atual
            string returnList = $"({ruleId})";

            // Recupera o parent da regra, se houver
            var parent = dbContext.AccessRules.FirstOrDefault(x => x.Id == ruleId)?.ParentId;
            if (parent != null)
            {
                // Se o parent existir, gera recursivamente a árvore para o parent
                returnList = GenerateTree(parent.Value, dbContext) + "," + returnList;
            }

            return returnList;
        }

        // Método para filtrar as regras de acesso por KeyName
        public async Task<AccessRule> GetAccessRulesByKeyNameAsync(string keyName, EntityDbContext dbContext)
        {
            return await dbContext.AccessRules.Where(x => x.KeyName == keyName).FirstOrDefaultAsync();
        }
    }
}
