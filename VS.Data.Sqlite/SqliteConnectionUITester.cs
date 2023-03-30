using System;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Microsoft.VisualStudio.Data.Sqlite;

class SqliteConnectionUITester : IVsDataConnectionUITester
{
    public void Test(IVsDataConnectionUIProperties connectionUIProperties)
    {
        var connectionOptions = new SqliteConnectionStringBuilder(connectionUIProperties.ToString());

        if (connectionOptions.DataSource.Length == 0)
        {
            throw new Exception("A temporary on-disk database will be created.");
        }

        if (connectionOptions.DataSource == ":memory:"
            || connectionOptions.Mode == SqliteOpenMode.Memory)
        {
            throw new Exception("A temporary in-memory database will be created.");
        }

        // Throw 'unable to open database file' when the file doesn't exist
        connectionOptions.Mode = SqliteOpenMode.ReadOnly;
        connectionOptions.Pooling = false;

        using var connection = new SqliteConnection(connectionOptions.ToString());
        connection.Open();
    }
}