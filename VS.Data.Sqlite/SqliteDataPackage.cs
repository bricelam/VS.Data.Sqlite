﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using static Microsoft.VisualStudio.VSConstants;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideService(typeof(SSqliteProviderObjectFactory), IsAsyncQueryable = true)]
    // TODO: Is Debugging too late?
    [ProvideAutoLoad(UICONTEXT.Debugging_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class SqliteDataPackage : AsyncPackage
    {
        /// <summary>
        /// SqliteDataPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "0feac8da-2a45-4623-8179-2e5b82928098";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            AddService(
                typeof(SSqliteProviderObjectFactory),
                (_, __, ___) => Task.FromResult<object>(new SqliteProviderObjectFactory()),
                promote: true);

            var sourceDir = Path.GetDirectoryName(new Uri(typeof(SqliteDataPackage).Assembly.CodeBase).LocalPath);

            var debuggerAssembly = Path.Combine(sourceDir, "SqliteVisualizer.UI.dll");
            var debuggeeAssembly = Path.Combine(sourceDir, "SqliteVisualizer.DebuggeeSide.dll");

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var shell = (IVsShell)await GetServiceAsync(typeof(SVsShell));
            var hr = shell.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out var visualStudioDir);
            Marshal.ThrowExceptionForHR(hr);

            var destDir = Path.Combine((string)visualStudioDir, "Visualizers");

            CopyFileIfNewer(debuggerAssembly, destDir);
            CopyFileIfNewer(debuggeeAssembly, Path.Combine(destDir, "netstandard2.0"));
        }

        #endregion

        private static void CopyFileIfNewer(string sourceFileName, string destDir)
        {
            var destFileName = Path.Combine(destDir, Path.GetFileName(sourceFileName));

            if (File.GetLastWriteTime(sourceFileName) > File.GetLastWriteTime(destFileName))
            {
                Directory.CreateDirectory(destDir);
                File.Copy(sourceFileName, destFileName, overwrite: true);
            }
        }
    }
}
