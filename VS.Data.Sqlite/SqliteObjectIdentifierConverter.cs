using System;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    // Needed to compensate for missing DbCommandBuilder support in the driver
    class SqliteObjectIdentifierConverter : AdoDotNetObjectIdentifierConverter
    {
        public SqliteObjectIdentifierConverter(IVsDataConnection connection)
            : base(connection)
        {
        }

        protected override string FormatPart(string typeName, object identifierPart, DataObjectIdentifierFormat format)
        {
            if (identifierPart == null || identifierPart is DBNull)
                return null;

            var identifierPartString = identifierPart.ToString();
            if (format.HasFlag(DataObjectIdentifierFormat.WithQuotes)
                && RequiresQuoting(identifierPartString))
            {
                return "\"" + identifierPartString.Replace("\"", "\"\"") + "\"";
            }

            return identifierPartString;
        }
    }
}