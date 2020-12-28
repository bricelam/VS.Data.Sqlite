using System;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    class SqliteConnectionProperties : AdoDotNetConnectionProperties
    {
        protected override void OnSiteChanged(EventArgs e)
        {
            base.OnSiteChanged(e);

            if (Site != null)
            {
                ConnectionStringBuilder.BrowsableConnectionString = false;
            }
        }
    }
}
