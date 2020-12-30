using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    internal sealed partial class PackageGuids
    {
        public const string guidSqliteDataSourceString = "27402929-CA43-48F6-B7DA-605E5D334774";
        public static Guid guidSqliteDataSource = new Guid(guidSqliteDataSourceString);

        public const string guidSqliteDataProviderString = "796A79E8-2579-4375-9E12-03A9E0D1FC02";
        public static Guid guidSqliteDataProvider = new Guid(guidSqliteDataProviderString);
    }
}
