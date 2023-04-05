using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data.Framework;

namespace Microsoft.VisualStudio.Data.Sqlite;

public partial class SqliteConnectionUIControl : DataConnectionUIControl
{
    public SqliteConnectionUIControl()
        => InitializeComponent();

    public override void LoadProperties()
        => _dataSourceTextBox.Text = (string)Site["Data Source"];

    void HandleDataSourceChanged(object sender, EventArgs e)
        => Site["Data Source"] = _openFileDialog.FileName = _dataSourceTextBox.Text;

    void HandleBrowse(object sender, EventArgs e)
    {
        var result = _openFileDialog.ShowDialog();
        if (result == DialogResult.Cancel)
            return;

        _dataSourceTextBox.Text = _openFileDialog.FileName;
    }
}
