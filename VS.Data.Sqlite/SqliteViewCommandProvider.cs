using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Services;

namespace Microsoft.VisualStudio.Data.Sqlite;

class SqliteViewCommandProvider : DataViewCommandProvider
{
    protected override MenuCommand CreateCommand(int itemId, CommandID commandId, object[] parameters)
    {
        if (commandId.Guid == PackageGuids.SqliteData
            && commandId.ID == PackageIds.ShowSqlCommand)
        {
            return new DataViewMenuCommand(
                itemId,
                commandId,
                (_, _) => ShowSql(Site.ExplorerConnection.FindNode(itemId)));
        }

        return base.CreateCommand(itemId, commandId, parameters);
    }

    void ShowSql(IVsDataExplorerNode node)
    {
        var dte = (DTE)Site.ServiceProvider.GetService(typeof(DTE));
        var sql = (string)node.Object.Properties["Sql"];

        var window = dte.ItemOperations.NewFile();
        var textDocument = (TextDocument)window.Document.Object("TextDocument");
        var editPoint = textDocument.StartPoint.CreateEditPoint();
        editPoint.Insert(sql);
    }
}
