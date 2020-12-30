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
                CREATE TABLE A (X);
                CREATE TABLE B (X);
            ");

            var selector = (IVsDataMappedObjectSelector)connection.GetService(typeof(IVsDataMappedObjectSelector));
            var tables = selector.SelectMappedObjects<IVsDataTable>();

            Assert.Collection(
                tables,
                t => Assert.Equal("A", t.Name),
                t => Assert.Equal("B", t.Name));
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
