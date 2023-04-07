using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Data.Sqlite;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid(PackageGuids.SqliteDataString)]
[ProvideService(typeof(SSqliteProviderObjectFactory), IsAsyncQueryable = true)]
[ProvideMenuResource("Menus.ctmenu", 1)]
public sealed class SqliteDataPackage : AsyncPackage
{
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        AddService(
            typeof(SSqliteProviderObjectFactory),
            (_, _, _) => Task.FromResult<object>(new SqliteProviderObjectFactory()),
            promote: true);

        // When initialized asynchronously, the current thread may be a background thread at this point.
        // Do any initialization that requires the UI thread after switching to the UI thread.
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
    }
}
