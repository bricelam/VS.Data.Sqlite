using System.Diagnostics;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA2;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Shell;
using Xunit;

namespace Microsoft.VisualStudio.Data.Sqlite;

[Collection("IdeCollection")]
public class ConnectionDialogTests
{
    [IdeFact]
    public void Works()
    {
        var dialogFactory = (IVsDataConnectionDialogFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionDialogFactory));

        var dialog = dialogFactory.CreateConnectionDialog();
        dialog.AddAllSources();
        dialog.SelectedSource = PackageGuids.guidSqliteDataSource;

        new Thread(AutomateConnectionCreation).Start(dialog);
        dialog.ShowDialog();

        Assert.Equal("Data Source=:memory:", dialog.SafeConnectionString);
    }

    static void AutomateConnectionCreation(object obj)
    {
        var dataConnectionDialog = (IVsDataConnectionDialog)obj;

        var application = Application.Attach(Process.GetCurrentProcess().Id);
        var automation = new UIA2Automation();
        var conditionally = automation.ConditionFactory;
        var mainWindow = application.GetMainWindow(automation);

        Window dialog;
        do
        {
            Thread.Yield();
            dialog = mainWindow.ModalWindows.FirstOrDefault(w => w.Name == dataConnectionDialog.Title);
        }
        while (dialog is null);


        dialog
            .FindFirstDescendant(
                conditionally.ByName("Database file name").And(conditionally.ByControlType(ControlType.Edit)))
            .AsTextBox()
            .Text = ":memory:";

        dialog
            .FindFirstDescendant(
                conditionally.ByName(dataConnectionDialog.AcceptButtonText))
            .AsButton()
            .Invoke();
    }
}
