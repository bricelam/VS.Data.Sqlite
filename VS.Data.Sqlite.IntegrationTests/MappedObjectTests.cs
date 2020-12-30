using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.RelationalObjectModel;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Shell;
using Xunit;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    public class MappedObjectTests
    {
        [VsFact]
        public void Can_select_tables()
        {
            var connectionFactory = (IVsDataConnectionFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionFactory));
            var connection = connectionFactory.CreateConnection(
                PackageGuids.guidSqliteDataProvider,
                "Data Source=:memory:",
                encryptedString: false);

            var command = (IVsDataCommand)connection.GetService(typeof(IVsDataCommand));
            command.ExecuteWithoutResults(@"
                CREATE TABLE table1 (id INTEGER PRIMARY KEY AUTOINCREMENT);
            ");

            var selector = (IVsDataMappedObjectSelector)connection.GetService(typeof(IVsDataMappedObjectSelector));
            var tables = selector.SelectMappedObjects<IVsDataTable>();

            Assert.Collection(
                tables,
                t =>
                {
                    Assert.Equal("main", t.Catalog);
                    Assert.Null(t.Schema);
                    Assert.Equal("sqlite_sequence", t.Name);
                    Assert.True(t.IsSystemObject);
                    Assert.Equal(new object[] { "main", null, "sqlite_sequence" }, t.Identifier);
                },
                t => Assert.Equal("table1", t.Name));
        }

        [VsFact]
        public void Can_select_tables_with_restrictions()
        {
            var connectionFactory = (IVsDataConnectionFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionFactory));
            var connection = connectionFactory.CreateConnection(
                PackageGuids.guidSqliteDataProvider,
                "Data Source=:memory:",
                encryptedString: false);

            var command = (IVsDataCommand)connection.GetService(typeof(IVsDataCommand));
            command.ExecuteWithoutResults(@"
                CREATE TABLE A (X);
                CREATE TABLE B (X);
            ");

            var selector = (IVsDataMappedObjectSelector)connection.GetService(typeof(IVsDataMappedObjectSelector));
            var tables = selector.SelectMappedObjects<IVsDataTable>(new object[] { null, null, "A" });

            Assert.Collection(
                tables,
                t => Assert.Equal("A", t.Name));
        }
    }
}
