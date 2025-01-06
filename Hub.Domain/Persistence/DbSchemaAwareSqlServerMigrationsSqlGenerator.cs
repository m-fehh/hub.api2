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
                    ensureSchemaOperation.Name = _tenantProvider.DbSchemaName;
                    break;

                case CreateTableOperation createTableOperation:
                    createTableOperation.Schema = _tenantProvider.DbSchemaName;

                    foreach (var foreignKey in createTableOperation.ForeignKeys)
                    {
                        ChangeSchema(foreignKey);
                    }
                    break;

                case DropTableOperation dropTableOperation:
                    dropTableOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case CreateIndexOperation createIndexOperation:
                    createIndexOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case AddColumnOperation addColumnOperation:
                    addColumnOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case AlterColumnOperation alterColumnOperation:
                    alterColumnOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case DropColumnOperation dropColumnOperation:
                    dropColumnOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case RenameColumnOperation renameColumnOperation:
                    renameColumnOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case AddForeignKeyOperation addForeignKeyOperation:
                    addForeignKeyOperation.Schema = _tenantProvider.DbSchemaName;
                    addForeignKeyOperation.PrincipalSchema = _tenantProvider.DbSchemaName;
                    break;

                case DropForeignKeyOperation dropForeignKeyOperation:
                    dropForeignKeyOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case RenameTableOperation renameTableOperation:
                    renameTableOperation.Schema = _tenantProvider.DbSchemaName;
                    renameTableOperation.NewSchema = _tenantProvider.DbSchemaName;
                    break;

                case RenameIndexOperation renameIndexOperation:
                    // Define o schema ao renomear um índice
                    renameIndexOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case SqlOperation sqlOperation:
                    break;

                case CreateSequenceOperation createSequenceOperation:
                    createSequenceOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case DropSequenceOperation dropSequenceOperation:
                    dropSequenceOperation.Schema = _tenantProvider.DbSchemaName;
                    break;

                case AlterSequenceOperation alterSequenceOperation:
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
