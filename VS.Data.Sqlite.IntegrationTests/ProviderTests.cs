using System;
using Microsoft.VisualStudio.Data.Core;
using Microsoft.VisualStudio.Data.Sqlite.Properties;
using Microsoft.VisualStudio.Shell;
using Xunit;

namespace Microsoft.VisualStudio.Data.Sqlite;

[Collection("IdeCollection")]
public class ProviderTests
{
    [IdeFact]
    public void Is_registered()
    {
        var providerManager = (IVsDataProviderManager)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataProviderManager));

        var provider = Assert.Contains(PackageGuids.guidSqliteDataProvider, providerManager.Providers);

        Assert.Equal("Microsoft.Data.Sqlite Provider", provider.Name);
        Assert.Equal(Resources.DataProvider_Description, provider.Description);
        Assert.Equal(Resources.DataProvider_DisplayName, provider.DisplayName);
        Assert.Equal("Microsoft.Data.Sqlite", provider.GetProperty("InvariantName"));
        Assert.Equal(Resources.DataProvider_ShortDisplayName, provider.ShortDisplayName);
        Assert.Equal(new Guid("77AB9A9D-78B9-4BA7-91AC-873F5338F1D2"), provider.Technology);
    }
}
