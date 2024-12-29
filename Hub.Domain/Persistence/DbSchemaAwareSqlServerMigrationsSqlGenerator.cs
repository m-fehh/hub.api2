using Hub.Infrastructure.Database.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Update;

namespace Hub.Domain.Persistence
{
    public class DbSchemaAwareSqlServerMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        private readonly ITenantProvider _tenantProvider;

        public DbSchemaAwareSqlServerMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer, ITenantProvider tenantProvider) : base(dependencies, commandBatchPreparer)  
        {
            _tenantProvider = tenantProvider;
        }

        protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            ChangeSchema(operation);
            base.Generate(operation, model, builder);
        }

        private void ChangeSchema(MigrationOperation? operation)
        {
            if (operation == null)
            {
                return;
            }

            switch (operation)
            {
                case SqlServerCreateDatabaseOperation _:
                case SqlServerDropDatabaseOperation _:
                    // Não é necessário tratar schema para criação ou exclusão de banco de dados
                    break;

                case EnsureSchemaOperation ensureSchemaOperation:
                    // Define o schema para garantir a existência do schema
                    ensureSchemaOperation.Name = _tenantProvider.DbSchemaName;
                    break;

                case CreateTableOperation createTableOperation:
                    // Define o schema na criação de uma tabela
                    createTableOperation.Schema = _tenantProvider.DbSchemaName;

                    // Tratar as chaves estrangeiras associadas à criação da tabela
                    foreach (var foreignKey in createTableOperation.ForeignKeys)
                    {
                        ChangeSchema(foreignKey);
                    }
                    break;

                case DropTableOperation dropTableOperation:
                    // Define o schema na exclusão de uma tabela
                    dropTableOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case CreateIndexOperation createIndexOperation:
                    // Define o schema na criação de um índice
                    createIndexOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case AddColumnOperation addColumnOperation:
                    // Define o schema na adição de uma coluna
                    addColumnOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case AlterColumnOperation alterColumnOperation:
                    // Define o schema na alteração de uma coluna
                    alterColumnOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case DropColumnOperation dropColumnOperation:
                    // Define o schema na exclusão de uma coluna
                    dropColumnOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case RenameColumnOperation renameColumnOperation:
                    // Define o schema ao renomear uma coluna
                    renameColumnOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case AddForeignKeyOperation addForeignKeyOperation:
                    // Define o schema ao adicionar uma chave estrangeira
                    addForeignKeyOperation.Schema = _tenantProvider.DbSchemaName;
                    addForeignKeyOperation.PrincipalSchema = _tenantProvider.DbSchemaName;
                    break;

                case DropForeignKeyOperation dropForeignKeyOperation:
                    // Define o schema ao excluir uma chave estrangeira
                    dropForeignKeyOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case RenameTableOperation renameTableOperation:
                    // Define o schema ao renomear uma tabela
                    renameTableOperation.Schema = _tenantProvider.DbSchemaName;
                    renameTableOperation.NewSchema = _tenantProvider.DbSchemaName;
                    break;

                case SqlOperation sqlOperation:
                    break;

                case CreateSequenceOperation createSequenceOperation:
                    // Define o schema ao criar uma sequência
                    createSequenceOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case DropSequenceOperation dropSequenceOperation:
                    // Define o schema ao excluir uma sequência
                    dropSequenceOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case AlterSequenceOperation alterSequenceOperation:
                    // Define o schema ao alterar uma sequência
                    alterSequenceOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                default:
                    throw new NotImplementedException(
                        $"Migration operation of type {operation.GetType().Name} is not supported by DbSchemaAwareSqlServerMigrationsSqlGenerator.");
            }
        }

        private string ReplaceSchemaInSql(string sql, string schema)
        {
            // Exemplo simples de substituição de schema para tabelas
            // Isso pode ser expandido dependendo dos tipos de operações SQL que você está manipulando.

            // Substitui os esquemas nas instruções SQL relacionadas a tabelas
            return sql.Replace("dbo.", $"{schema}.", StringComparison.OrdinalIgnoreCase);
        }

    }
}
