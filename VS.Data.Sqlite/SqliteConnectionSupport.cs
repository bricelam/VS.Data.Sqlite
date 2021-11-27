﻿using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    class SqliteConnectionSupport : AdoDotNetConnectionSupport
    {
        protected override object CreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(IVsDataSourceInformation))
            {
                return new SqliteSourceInformation(Site);
            }
            if (serviceType == typeof(IVsDataObjectIdentifierConverter))
            {
                return new SqliteObjectIdentifierConverter(Site);
            }
            if (serviceType == typeof(IVsDataObjectSupport))
            {
                return new DataObjectSupport(
                    GetType().Namespace + ".SqliteObjectSupport",
                    GetType().Assembly);
            }
            if (serviceType == typeof(IVsDataMappedObjectConverter))
            {
                return new SqliteMappedObjectConverter(Site);
            }

            return base.CreateService(container, serviceType);
        }
    }
}
