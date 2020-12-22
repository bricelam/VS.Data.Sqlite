using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.ExtensionManager;

namespace Microsoft.VisualStudio.Data.Sqlite
{
    class SqliteViewCommandProvider : DataViewCommandProvider
    {
        public const string ToolboxId = "41521019-e4c7-480c-8ea8-fc4a2c6f50aa";

        protected override MenuCommand CreateCommand(int itemId, CommandID commandId, object[] parameters)
        {
            if (commandId.Guid == PackageGuids.guidSqliteDataPackageCmdSet
                && commandId.ID == PackageIds.InstallToolboxId)
            {
                var command = new DataViewMenuCommand(itemId, commandId, ExecuteInstallToolbox);

                var extensionManager = (IVsExtensionManager)Shell.Package.GetGlobalService(typeof(SVsExtensionManager));
                command.Visible = !extensionManager.GetInstalledExtensions()
                    .Any(e => e.Header.Identifier == ToolboxId);

                return command;
            }

            return base.CreateCommand(itemId, commandId, parameters);
        }

        void ExecuteInstallToolbox(object sender, EventArgs e)
        {
            var repository = (IVsExtensionRepository)Shell.Package.GetGlobalService(typeof(SVsExtensionRepository));
            var manager = (IVsExtensionManager)Shell.Package.GetGlobalService(typeof(SVsExtensionManager));
            var entry = repository
                .CreateQuery<GalleryEntry>(
                    includeTypeInQuery: false,
                    includeSkuInQuery: true,
                    searchSource: "ExtensionManagerUpdate")
                .Where(r => r.VsixID == ToolboxId)
                .AsEnumerable()
                .FirstOrDefault();
            var installable = repository.Download(entry);
            manager.Install(installable, perMachine: false);

            // TODO: Need serviceProvider
            //VsShellUtilities.ShowMessageBox(
            //    serviceProvider,
            //    "SQLite/SQL Server Compact Toolbox has been scheduled for install. It will begin when all Microsoft Visual Studio windows are closed.",
            //    "Install SQLite Toolbox",
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            var command = (MenuCommand)sender;
            command.Enabled = false;
        }

        class GalleryEntry : IRepositoryEntry
        {
            public string VsixID { get; set; }
            public string DownloadUrl { get; set; }
            public string DownloadUpdateUrl { get; set; }
            public string VsixReferences { get; set; }
        }
    }
}
