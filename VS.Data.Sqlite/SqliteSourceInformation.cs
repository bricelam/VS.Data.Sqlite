using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;

namespace Microsoft.VisualStudio.Data.Sqlite;

class SqliteSourceInformation : AdoDotNetSourceInformation
{
    public SqliteSourceInformation(IVsDataConnection connection)
        : base(connection)
    {
        AddProperty(SupportsQuotedIdentifierParts, true);
        AddProperty(IdentifierOpenQuote, "\"");
        AddProperty(IdentifierCloseQuote, "\"");
    }

    protected override object RetrieveValue(string propertyName)
        => propertyName switch
        {
            DataSourceProduct => "SQLite",
            DataSourceVersion => Connection.ServerVersion,
            _ => base.RetrieveValue(propertyName)
        };
}
