using System;
using System.Data.Common;
using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    [Guid("F4B49744-36C6-4F57-A196-E825094D31DD")]
    public interface SSqliteProviderObjectFactory
    {
    }

    class SqliteProviderObjectFactory : DataProviderObjectFactory, SSqliteProviderObjectFactory
    {
        static SqliteProviderObjectFactory()
        {
            DbProviderFactoriesEx.RegisterFactory("Microsoft.Data.Sqlite", typeof(SqliteFactory));

            // TODO: Find a better way
            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            {
                var assembly = typeof(SqliteFactory).Assembly;

                return args.Name == assembly.FullName
                    ? assembly
                    : null;
            };
        }

        public override object CreateObject(Type objType)
        {
            if (objType == typeof(IVsDataConnectionSupport))
            {
                return new SqliteConnectionSupport();
            }
            if (objType == typeof(IVsDataConnectionUIControl))
            {
                return new SqliteConnectionUIControl();
            }
            if (objType == typeof(IVsDataConnectionProperties)
                || objType == typeof(IVsDataConnectionUIProperties))
            {
                return new SqliteConnectionProperties();
            }
            if (objType == typeof(IVsDataViewSupport))
            {
                return new DataViewSupport(
                    "Microsoft.VisualStudio.Data.Sqlite.SqliteViewSupport",
                    typeof(SqliteProviderObjectFactory).Assembly);
            }

            return null;
        }
    }
}
