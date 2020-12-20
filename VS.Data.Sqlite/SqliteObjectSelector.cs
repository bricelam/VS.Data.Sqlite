using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    class SqliteObjectSelector : AdoDotNetObjectSelector
    {
        readonly Dictionary<string, Func<SqliteConnection, string[], DataTable>> _objectSelectors
            = new Dictionary<string, Func<SqliteConnection, string[], DataTable>>
            {
                { "Tables",  SelectTables },
                { "Columns", SelectColumns },
                { "Views", SelectViews },
                { "Triggers", SelectTriggers }
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

                if (!_objectSelectors.TryGetValue((string)parameters[0], out var selectObjects))
                {
                    // TODO: Message
                    throw new NotSupportedException();
                }

                var dataTable = selectObjects(
                    connection,
                    restrictions?.Cast<string>().ToArray() ?? Array.Empty<string>());

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

        static DataTable SelectTables(SqliteConnection connection, string[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT *
                FROM sqlite_master
                WHERE type = 'table'
                    AND name NOT LIKE 'sqlite_%'
            ";

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        static DataTable SelectColumns(SqliteConnection connection, string[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT $table AS ""table"", *
                FROM pragma_table_info($table)
            ";
            command.Parameters.AddWithValue("$table", restrictions[0]);

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            // TODO: Get collSeq and autoinc via sqlite3_table_column_metadata
            return dataTable;
        }

        static DataTable SelectViews(SqliteConnection connection, string[] restrictions)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT *
                FROM sqlite_master
                WHERE type = 'view'
                    AND name NOT LIKE 'sqlite_%'
            ";

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        static DataTable SelectTriggers(SqliteConnection connection, string[] restrictions)
        {
            var table = restrictions.Length < 1 ? DBNull.Value : (object)restrictions[0];

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT *
                FROM sqlite_master
                WHERE type = 'trigger'
                    AND ($table IS NULL OR tbl_name = $table)
            ";
            command.Parameters.AddWithValue("$table", table);

            var dataTable = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }
    }
}
