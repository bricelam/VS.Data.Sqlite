using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Data.Sqlite.Properties;

using static SQLitePCL.raw;

namespace Microsoft.VisualStudio.Data.Sqlite;

class SqliteObjectSelector : AdoDotNetObjectSelector
{
    readonly Dictionary<string, Func<SqliteConnection, object[], DataTable>> _objectSelectors
        = new()
        {
            { "Tables",  SelectTables },
            { "TableColumns", SelectTableColumns },
            { "TableIndexes", SelectTableIndexes },
            { "TableIndexColumns", SelectTableIndexColumns },
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
                throw new ArgumentException(string.Format(Resources.UnknownCollection, collectionName));
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
            SELECT t.name, sql, t2.type, ncol, wr, strict
            FROM sqlite_master AS t
            JOIN pragma_table_list(t.name) AS t2
            WHERE t.type = 'table'
                AND ($name IS NULL OR t.name = $name)
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
                { "hidden", typeof(long) },
                { "collSeq" },
                { "autoInc", typeof(int) }
            }
        };
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var table = reader.GetString(0);
                var cid = reader.GetInt64(1);
                var name = reader.GetString(2);
                var type = reader.GetString(3);
                var notnull = reader.GetInt64(4);
                var dfltValue = reader.GetValue(5);
                var pk = reader.GetInt64(6);
                var hidden = reader.GetInt64(7);

                var rc = sqlite3_table_column_metadata(
                    connection.Handle,
                    connection.Database,
                    table,
                    name,
                    out _,
                    out var collSeq,
                    out _,
                    out _,
                    out var autoInc);
                SqliteException.ThrowExceptionForRC(rc, connection.Handle);

                dataTable.Rows.Add(
                    table,
                    cid,
                    name,
                    type,
                    notnull,
                    dfltValue,
                    pk,
                    hidden,
                    collSeq,
                    autoInc);
            }
        }

        return dataTable;
    }

    static DataTable SelectTableIndexes(SqliteConnection connection, object[] restrictions)
    {
        var command = connection.CreateCommand();
        command.CommandText =
        @"
            SELECT tbl_name, i.name, ""unique"", origin, partial, sql
            FROM sqlite_master AS i
            JOIN pragma_index_list(tbl_name) AS i2 ON i2.name = i.name
            WHERE type = 'index'
                AND ($table IS NULL OR tbl_name = $table)
                AND ($name IS NULL OR i.name = $name)
        ";
        command.Parameters.AddWithValue("$table", (restrictions.Length <= 0 ? null : restrictions[0]) ?? DBNull.Value);
        command.Parameters.AddWithValue("$name", (restrictions.Length <= 1 ? null : restrictions[1]) ?? DBNull.Value);

        var dataTable = new DataTable();
        using (var reader = command.ExecuteReader())
        {
            dataTable.Load(reader);
        }

        // TODO: Synthesize rowid pk constraints?
        return dataTable;
    }

    static DataTable SelectTableIndexColumns(SqliteConnection connection, object[] restrictions)
    {
        var command = connection.CreateCommand();
        command.CommandText =
        @"
            SELECT tbl_name, i.name AS ""index"", c.seqno, c.cid, c.name, desc, coll, key
            FROM sqlite_master AS i
            JOIN pragma_index_info(i.name) AS c
            JOIN pragma_index_xinfo(i.name) AS c2
            WHERE i.type = 'index'
                AND ($table IS NULL OR tbl_name = $table)
                AND ($index IS NULL OR i.name = $index)
                AND ($name IS NULL OR c.name = $name)
        ";
        command.Parameters.AddWithValue("$table", (restrictions.Length <= 0 ? null : restrictions[0]) ?? DBNull.Value);
        command.Parameters.AddWithValue("$index", (restrictions.Length <= 1 ? null : restrictions[1]) ?? DBNull.Value);
        command.Parameters.AddWithValue("$name", (restrictions.Length <= 2 ? null : restrictions[2]) ?? DBNull.Value);

        var dataTable = new DataTable();
        using (var reader = command.ExecuteReader())
        {
            dataTable.Load(reader);
        }

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
            SELECT v.name, sql, ncol
            FROM sqlite_master AS v
            JOIN pragma_table_list(v.name) AS v2
            WHERE v.type = 'view'
                AND ($name IS NULL OR v.name = $name)
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
