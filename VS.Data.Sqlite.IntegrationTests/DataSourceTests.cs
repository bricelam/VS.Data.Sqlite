using Microsoft.VisualStudio.Data.Core;
using Microsoft.VisualStudio.Data.Sqlite.Properties;
using Microsoft.VisualStudio.Shell;
using Xunit;

namespace Microsoft.VisualStudio.Data.Sqlite;

public class DataSourceTests
{
    [VsFact]
    public void Is_registered()
    {
        var dataSourceManager = (IVsDataSourceManager)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataSourceManager));

        var dataSource = Assert.Contains(PackageGuids.guidSqliteDataSource, dataSourceManager.Sources);

        Assert.Equal("SQLite", dataSource.Name);
        Assert.Equal(PackageGuids.guidSqliteDataProvider, dataSource.DefaultProvider);
        Assert.Equal(Resources.DataSource_Description, dataSource.GetDescription(PackageGuids.guidSqliteDataProvider));
        Assert.Equal(Resources.DataSource_DisplayName, dataSource.DisplayName);
        Assert.Equal(new[] { PackageGuids.guidSqliteDataProvider }, dataSource.GetProviders());
    }
}
