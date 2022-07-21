using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    internal class SqliteSourceInformation : AdoDotNetSourceInformation
    {
        public SqliteSourceInformation(IVsDataConnection connection)
            : base(connection)
        {
            Initialize();
        }

        private void Initialize()
        {
            AddProperty(DefaultSchema);
            AddProperty(DefaultCatalog, "main");
            AddProperty(SupportsAnsi92Sql, true);
            AddProperty(SupportsQuotedIdentifierParts, true);
            AddProperty(IdentifierOpenQuote, "\"");
            AddProperty(IdentifierCloseQuote, "\"");
            AddProperty(CatalogSeparator, ".");
            AddProperty(CatalogSupported, true);
            AddProperty(CatalogSupportedInDml, true);
            AddProperty(SchemaSupported, false);
            AddProperty(SchemaSupportedInDml, false);
            AddProperty(SchemaSeparator, string.Empty);
            AddProperty(ParameterPrefix, "$");
            AddProperty(ParameterPrefixInName, true);
        }

        protected override object RetrieveValue(string propertyName)
        {
            switch (propertyName)
            {
                case "DataSourceProduct":
                    return "SQLite";

                case "DataSourceVersion":
                    return Connection.ServerVersion;

                default:
                    return base.RetrieveValue(propertyName);
            }
        }
    }
}
