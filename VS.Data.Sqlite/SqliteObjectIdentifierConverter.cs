using System;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;

namespace Microsoft.VisualStudio.Data.Sqlite
{
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
                // TODO: Why doesn't Query Designer actually use this?
                return "\"" + identifierPartString.Replace("\"", "\"\"") + "\"";
            }

            return identifierPartString;
        }
    }
}
