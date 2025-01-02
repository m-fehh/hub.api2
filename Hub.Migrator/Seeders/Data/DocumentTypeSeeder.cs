using Hub.Domain.Entities;
using Hub.Infrastructure.Internacionalization.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hub.Migrator.Seeders.Data
{
    public class DocumentTypeSeeder : ISeeder
    {
        public int Order => 3;

        public async Task SeedAsync(EntityDbContext dbContext)
        {
            var documentTypes = new List<DocumentType>
            {
                new DocumentType
                {
                    Abbrev = "CPF",
                    Name = "Cadastro de Pessoas Físicas",
                    Mask = "000.000.000-00",
                    MinLength = 11,
                    MaxLength = 11,
                    SpecialDocumentValidation = ESpecialDocumentValidation.CPF,
                    Inactive = false
                },
                new DocumentType
                {
                    Abbrev = "RNE",
                    Name = "Registro Nacional de Estrangeiros",
                    Mask = "AAAAAAA-A",
                    MinLength = 7,
                    MaxLength = 9,
                    SpecialDocumentValidation = ESpecialDocumentValidation.None,
                    Inactive = false
                },
                new DocumentType
                {
                    Abbrev = "CNPJ",
                    Name = "Cadastro Nacional de Pessoas Jurídicas",
                    Mask = "00.000.000/0000-00",
                    MinLength = 14,
                    MaxLength = 14,
                    SpecialDocumentValidation = ESpecialDocumentValidation.CNPJ,
                    Inactive = false
                },
                new DocumentType
                {
                    Abbrev = "RUT",
                    Name = "Rol Único Tributário",
                    Mask = "00.000.000-A || 00.000.00-A",
                    MinLength = 8,
                    MaxLength = 9,
                    SpecialDocumentValidation = ESpecialDocumentValidation.RUT,
                    Inactive = true
                }
            };

            foreach (var documentType in documentTypes)
            {
                var exists = await dbContext.DocumentTypes.AnyAsync(x => x.Name.Equals(documentType.Name));

                if (!exists)
                {
                    dbContext.DocumentTypes.Add(documentType);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
