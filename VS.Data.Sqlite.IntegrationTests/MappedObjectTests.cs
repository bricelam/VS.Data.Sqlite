using System;
using System.Data;
using Microsoft.Data.Sqlite;
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
                CREATE TABLE table1 (
                    column1 INTEGER NOT NULL CONSTRAINT pk1 PRIMARY KEY AUTOINCREMENT,
                    column2 INTEGER CONSTRAINT ak1 UNIQUE,
                    column3 INTEGER CONSTRAINT fk1 REFERENCES table2 (id) ON DELETE CASCADE ON UPDATE CASCADE,
                    column4 AS (1),
                    column4 AS (1) STORED
                );

                CREATE UNIQUE INDEX ix1 ON table1 (column3);

                CREATE TABLE table2 (
                    id INTEGER PRIMARY KEY
                );
            ");

            var selector = (IVsDataMappedObjectSelector)connection.GetService(typeof(IVsDataMappedObjectSelector));
            var tables = selector.SelectMappedObjects<IVsDataTable>();

            Assert.Collection(
                tables,
                t =>
                {
                    Assert.Equal("sqlite_sequence", t.Name);
                    Assert.True(t.IsSystemObject);
                },
                t =>
                {
                    Assert.Equal("main", t.Catalog);
                    Assert.Null(t.Schema);
                    Assert.Equal("table1", t.Name);
                    Assert.Equal(new object[] { "main", null, "table1" }, t.Identifier);
                    Assert.Collection(
                        t.Columns,
                        c =>
                        {
                            // TODO: Verify DefaultValue
                            Assert.Equal("column1", c.Name);
                            Assert.Equal(new object[] { "main", null, "table1", "column1" }, c.Identifier);
                            Assert.Equal(0, c.Ordinal);
                            Assert.Equal("INTEGER", c.NativeDataType);
                            Assert.Equal((int)SqliteType.Integer, c.AdoDotNetDataType);
                            Assert.Equal((int)DbType.Int64, c.AdoDotNetDbType);
                            Assert.Equal(typeof(long), c.FrameworkDataType);
                            Assert.False(c.IsNullable);
                        },
                        c => Assert.Equal("column2", c.Name),
                        c => Assert.Equal("column3", c.Name),
                        c =>
                        {
                            Assert.Equal("column4", c.Name);
                            Assert.True(c.IsComputed);
                        },
                        c =>
                        {
                            Assert.Equal("column5", c.Name);
                            Assert.True(c.IsComputed);
                        });
                    Assert.Throws<NotSupportedException>(() => t.ForeignKeys);
                    Assert.Throws<NotSupportedException>(() => t.UniqueKeys);
                },
                t => Assert.Equal("table2", t.Name));
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
