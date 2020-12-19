using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    class SqliteObjectSelector : AdoDotNetObjectSelector
    {
        readonly Dictionary<string, IList<string>> _supportedRestrictions = new Dictionary<string, IList<string>>();

        public SqliteObjectSelector()
        {
        }

        public SqliteObjectSelector(IVsDataConnection connection)
            : base(connection)
        {
        }

        protected override IList<string> GetSupportedRestrictions(string typeName, object[] parameters)
            => _supportedRestrictions[typeName] = base.GetSupportedRestrictions(typeName, parameters);

        protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            if (parameters == null
                || parameters.Length == 0 || parameters.Length > 2
                || !(parameters[0] is string commandText))
            {
                throw new ArgumentException("The restrictions array passed to this object selector is invalid; it must be either null or an empty array.");
            }

            IVsDataReader dataReader;

            var connection = (DbConnection)Site.GetLockedProviderObject();
            try
            {
                Site.EnsureConnected();

                var dataTable = new DataTable();

                var command = connection.CreateCommand();
                command.CommandText = commandText;

                if (restrictions != null)
                {
                    for (var i = 0; i < restrictions.Length; i++)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = _supportedRestrictions[typeName][i];
                        parameter.Value = restrictions[i];
                        command.Parameters.Add(parameter);
                    }
                }

                using (var reader = command.ExecuteReader())
                {
                    dataTable.Load(reader);
                }

                if (parameters.Length == 2
                    && parameters[1] is DictionaryEntry mappingsEntry
                    && mappingsEntry.Value is object[] mappings)
                {
                    ApplyMappings(dataTable, GetMappings(mappings));
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
}
