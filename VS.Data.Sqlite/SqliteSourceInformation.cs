using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    internal class SqliteSourceInformation : AdoDotNetSourceInformation
    {
        public SqliteSourceInformation(IVsDataConnection connection)
            : base(connection)
        {
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
