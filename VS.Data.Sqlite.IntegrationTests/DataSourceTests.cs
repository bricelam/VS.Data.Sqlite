using System;
using Microsoft.VisualStudio.Data.Core;
using Microsoft.VisualStudio.Data.Sqlite.Properties;
using Microsoft.VisualStudio.Shell;
using Xunit;

namespace VisualStudio.Data.Sqlite
{
    public class DataSourceTests
    {
        [VsFact]
        public void Is_registered()
        {
            var dataSourceManager = (IVsDataSourceManager)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataSourceManager));

            var dataSource = Assert.Contains(new Guid("27402929-CA43-48F6-B7DA-605E5D334774"), dataSourceManager.Sources);

            Assert.Equal("SQLite", dataSource.Name);
            Assert.Equal(new Guid("796A79E8-2579-4375-9E12-03A9E0D1FC02"), dataSource.DefaultProvider);
            Assert.Equal(Resources.DataSource_Description, dataSource.GetDescription(new Guid("796A79E8-2579-4375-9E12-03A9E0D1FC02")));
            Assert.Equal(Resources.DataSource_DisplayName, dataSource.DisplayName);
            Assert.Equal(new[] { new Guid("796A79E8-2579-4375-9E12-03A9E0D1FC02") }, dataSource.GetProviders());
        }
    }
}
