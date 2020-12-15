using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    [Guid("F4B49744-36C6-4F57-A196-E825094D31DD")]
    public interface SSqliteObjectFactory
    {
    }

    internal class SqliteObjectFactory : DataProviderObjectFactory, SSqliteObjectFactory
    {
        public override object CreateObject(Type objType)
        {
            if (objType == typeof(IVsDataConnectionSupport))
            {
                return new AdoDotNetConnectionSupport();
            }

            Debug.Fail("Unexpected type: " + objType.Name);

            return null;
        }
    }
}
