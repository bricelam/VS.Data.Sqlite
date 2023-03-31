﻿using System.Windows.Forms;
using Microsoft.VisualStudio.Data.Framework;

namespace Microsoft.VisualStudio.Data.Sqlite;

public partial class SqliteConnectionUIControl : DataConnectionUIControl
{
    public SqliteConnectionUIControl()
    {
        InitializeComponent();
    }

    public override void LoadProperties()
    {
        _dataSourceTextBox.Text = (string)Site["Data Source"];
    }

    private void HandleDataSourceChanged(object sender, System.EventArgs e)
    {
        Site["Data Source"] = _openFileDialog.FileName = _dataSourceTextBox.Text;
    }

    private void HandleBrowse(object sender, System.EventArgs e)
    {
        var result = _openFileDialog.ShowDialog();
        if (result == DialogResult.Cancel)
            return;

        _dataSourceTextBox.Text = _openFileDialog.FileName;
    }

    private void HandleDragDrop(object sender, DragEventArgs e)
    {
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        _dataSourceTextBox.Text = files[0];
    }

    private void HandleDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.Link;
        }
    }
}
