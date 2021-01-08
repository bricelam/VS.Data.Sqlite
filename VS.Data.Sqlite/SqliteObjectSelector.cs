using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    class SqliteObjectSelector : AdoDotNetObjectSelector
    {
        readonly Dictionary<string, Func<SqliteConnection, object[], DataTable>> _objectSelectors
            = new Dictionary<string, Func<SqliteConnection, object[], DataTable>>
            {
                { "Tables",  SelectTables },
                { "TableColumns", SelectTableColumns },
                { "TableTriggers", SelectTableTriggers },
                { "Views", SelectViews },
                { "ViewColumns", SelectViewColumns },
                { "ViewTriggers", SelectViewTriggers }
            };

        public SqliteObjectSelector()
        {
        }

        public SqliteObjectSelector(IVsDataConnection connection)
            : base(connection)
        {
        }

        protected override IVsDataReader SelectObjects(
            string typeName,
            object[] restrictions,
            string[] properties,
            object[] parameters)
        {
            IVsDataReader dataReader;

            var connection = (SqliteConnection)Site.GetLockedProviderObject();
            try
            {
                Site.EnsureConnected();

                var collectionName = (string)parameters[0];
                if (!_objectSelectors.TryGetValue(collectionName, out var selectObjects))
                {
                    // TODO: Resourcify
                    throw new ArgumentException($"The requested collection ({collectionName}) is not defined.");
                }

                var dataTable = selectObjects(
                    connection,
                    restrictions ?? Array.Empty<object>());

                if (parameters.Length == 2)
                {
                    ApplyMappings(dataTable, GetMappings((object[])((DictionaryEntry)parameters[1]).Value));
                }

                dataReader = new AdoDotNetTableReader(dataTable);
            }
            finally
            {
                Site.UnlockProviderObject();
            }

            return dataReader;
        }

        static DataTable SelectTables(SqliteConnection connection, object[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT name, sql
                FROM sqlite_master
                WHERE type = 'table'
                    AND ($name IS NULL OR name = $name)
            ";
            command.Parameters.AddWithValue("$name", (restrictions.Length <= 0 ? null : restrictions[0]) ?? DBNull.Value);

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        static DataTable SelectTableColumns(SqliteConnection connection, object[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT t.name AS ""table"", cid, c.name, c.type, ""notnull"", dflt_value, pk, hidden
                FROM sqlite_master AS t
                JOIN pragma_table_xinfo(t.name) AS c
                WHERE t.type = 'table'
                    AND ($table IS NULL OR t.name = $table)
                    AND ($name IS NULL OR c.name = $name)
            ";
            command.Parameters.AddWithValue("$table", (restrictions.Length <= 0 ? null : restrictions[0]) ?? DBNull.Value);
            command.Parameters.AddWithValue("$name", (restrictions.Length <= 1 ? null : restrictions[1]) ?? DBNull.Value);

            var dataTable = new DataTable
            {
                Columns =
                {
                    { "table" },
                    { "cid", typeof(long) },
                    { "name" },
                    { "type" },
                    { "notnull", typeof(long) },
                    { "dflt_value" },
                    { "pk", typeof(long) },
                    { "hidden", typeof(long) }
                }
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var values = new object[8];
                    reader.GetValues(values);

                    // NB: Can't use Load because dflt_value is nullable without a declared type
                    dataTable.Rows.Add(values);
                }
            }

            // TODO: Get collSeq and autoinc via sqlite3_table_column_metadata
            return dataTable;
        }

        static DataTable SelectTableTriggers(SqliteConnection connection, object[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT t.name AS ""table"", r.name, r.sql
                FROM sqlite_master AS t
                JOIN sqlite_master AS r ON r.tbl_name = t.name
                WHERE t.type = 'table'
                    AND r.type == 'trigger'
                    AND ($table IS NULL OR t.name = $table)
                    AND ($name IS NULL OR r.name = $name)
            ";
            command.Parameters.AddWithValue("$table", (restrictions.Length <= 0 ? null : restrictions[0]) ?? DBNull.Value);
            command.Parameters.AddWithValue("$name", (restrictions.Length <= 1 ? null : restrictions[1]) ?? DBNull.Value);

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        static DataTable SelectViews(SqliteConnection connection, object[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT name, sql
                FROM sqlite_master
                WHERE type = 'view'
                    AND ($name IS NULL OR name = $name)
            ";
            command.Parameters.AddWithValue("$name", (restrictions.Length <= 0 ? null : restrictions[0]) ?? DBNull.Value);

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        static DataTable SelectViewColumns(SqliteConnection connection, object[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT v.name AS ""view"", cid, c.name, c.type
                FROM sqlite_master AS v
                JOIN pragma_table_xinfo(v.name) AS c
                WHERE v.type = 'view'
                    AND ($view IS NULL OR v.name = $view)
                    AND ($name IS NULL OR c.name = $name)
            ";
            command.Parameters.AddWithValue("$view", (restrictions.Length <= 0 ? null : restrictions[0]) ?? DBNull.Value);
            command.Parameters.AddWithValue("$name", (restrictions.Length <= 1 ? null : restrictions[1]) ?? DBNull.Value);

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        static DataTable SelectViewTriggers(SqliteConnection connection, object[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT v.name AS ""view"", r.name, r.sql
                FROM sqlite_master AS v
                JOIN sqlite_master AS r ON r.tbl_name = v.name
                WHERE v.type = 'view'
                    AND r.type == 'trigger'
                    AND ($view IS NULL OR v.name = $view)
                    AND ($name IS NULL OR r.name = $name)
            ";
            command.Parameters.AddWithValue("$view", (restrictions.Length <= 0 ? null : restrictions[0]) ?? DBNull.Value);
            command.Parameters.AddWithValue("$name", (restrictions.Length <= 1 ? null : restrictions[1]) ?? DBNull.Value);

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }
    }
}
