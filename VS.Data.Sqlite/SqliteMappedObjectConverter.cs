using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;

using static SQLitePCL.raw;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    internal class SqliteMappedObjectConverter : AdoDotNetMappedObjectConverter
    {
        public SqliteMappedObjectConverter(IVsDataConnection connection)
            : base(connection)
        {
        }

        protected override int GetProviderTypeFromNativeType(string nativeType)
        {
            var typeRules = new Func<string, int?>[]
            {
                name => name.IndexOf("INT", StringComparison.OrdinalIgnoreCase) >= 0
                    ? SQLITE_INTEGER
                    : (int?)null,
                name => name.IndexOf("CHAR", StringComparison.OrdinalIgnoreCase) >= 0
                        || name.IndexOf("CLOB", StringComparison.OrdinalIgnoreCase) >= 0
                        || name.IndexOf("TEXT", StringComparison.OrdinalIgnoreCase) >= 0
                    ? SQLITE_TEXT
                    : (int?)null,
                name => name.IndexOf("BLOB", StringComparison.OrdinalIgnoreCase) >= 0
                    ? SQLITE_BLOB
                    : (int?)null,
                name => name.IndexOf("REAL", StringComparison.OrdinalIgnoreCase) >= 0
                        || name.IndexOf("FLOA", StringComparison.OrdinalIgnoreCase) >= 0
                        || name.IndexOf("DOUB", StringComparison.OrdinalIgnoreCase) >= 0
                    ? SQLITE_FLOAT
                    : (int?)null
            };

            return typeRules.Select(r => r(nativeType)).FirstOrDefault(r => r != null) ?? SQLITE_TEXT;
        }

        protected override Type GetFrameworkTypeFromNativeType(string nativeType)
        {
            var providerType = GetProviderTypeFromNativeType(nativeType);
            switch (providerType)
            {
                case SQLITE_INTEGER:
                    return typeof(long);

                case SQLITE_FLOAT:
                    return typeof(double);

                case SQLITE_TEXT:
                    return typeof(string);

                case SQLITE_BLOB:
                    return typeof(byte[]);

                default:
                    Debug.Fail("Unexpected type: " + providerType);
                    return typeof(string);
            }
        }
    }
}
