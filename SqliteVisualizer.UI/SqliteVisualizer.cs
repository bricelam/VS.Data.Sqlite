using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Interop;
using Microsoft.VisualStudio.DebuggerVisualizers;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;
using IWin32Window = System.Windows.Forms.IWin32Window;

[assembly: DebuggerVisualizer(
    typeof(SqliteVisualizer.SqliteVisualizer),
    "SqliteVisualizer.SqliteVisualizerSource, SqliteVisualizer.DebuggeeSide",
    TargetTypeName = "Microsoft.Data.Sqlite.SqliteConnection, Microsoft.Data.Sqlite",
    Description = "SQLite Visualizer")]

namespace SqliteVisualizer
{
    public class SqliteVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var owner = (IWin32Window)windowService;
            var objectProvider2 = (IVisualizerObjectProvider2)objectProvider;

            string[] tables;
            using (var outgoingData = new MemoryStream())
            {
                objectProvider2.Serialize("/tables", outgoingData);
                var incomingData = objectProvider2.TransferData(outgoingData);

                tables = (string[])objectProvider2.Deserialize(incomingData);
            }

            var window = new MainWindow();

            var interop = new WindowInteropHelper(window);
            interop.Owner = owner.Handle;

            var handle = (HWND)interop.EnsureHandle();

            // Remove minimize button
            var style = (WINDOW_STYLE)GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            SetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)(style & ~WINDOW_STYLE.WS_MINIMIZEBOX));

            // Remove icon
            var styleEx = (WINDOW_EX_STYLE)GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
            SetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (int)(styleEx | WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME));
            SendMessage(handle, WM_SETICON, (UIntPtr)ICON_SMALL, IntPtr.Zero);
            SendMessage(handle, WM_SETICON, (UIntPtr)ICON_BIG, IntPtr.Zero);

            window.ShowDialog();
        }
    }
}
