using System.Collections;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Microsoft.VisualStudio.Data.Sqlite;

class SqliteRootObjectSelector : AdoDotNetRootObjectSelector
{
    protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
    {
        IVsDataReader dataReader;

        var connection = (SqliteConnection)Site.GetLockedProviderObject();
        try
        {
            Site.EnsureConnected();

            var dataTable = new DataTable
            {
                Columns =
                {
                    { "DatabaseName" }
                },
                Rows =
                {
                    new object[]
                    {
                        Path.GetFileNameWithoutExtension(connection.DataSource)
                    }
                }
            };

            if (parameters?.Length == 1)
            {
                ApplyMappings(dataTable, GetMappings((object[])((DictionaryEntry)parameters[0]).Value));
            }

            dataReader = new AdoDotNetTableReader(dataTable);
        }
        finally
        {
            Site.UnlockProviderObject();
        }

        return dataReader;
    }
}
