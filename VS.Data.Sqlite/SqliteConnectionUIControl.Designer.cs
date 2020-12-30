
namespace Microsoft.VisualStudio.Data.Sqlite
{
    partial class SqliteConnectionUIControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Button _browseButton;
            System.Windows.Forms.Label _dataSourceLabel;
            this._dataSourceTextBox = new System.Windows.Forms.TextBox();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            _browseButton = new System.Windows.Forms.Button();
            _dataSourceLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _browseButton
            // 
            _browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            _browseButton.Location = new System.Drawing.Point(297, 16);
            _browseButton.Name = "_browseButton";
            _browseButton.Size = new System.Drawing.Size(75, 23);
            _browseButton.TabIndex = 1;
            _browseButton.Text = "&Browse...";
            _browseButton.UseVisualStyleBackColor = true;
            _browseButton.Click += new System.EventHandler(this.HandleBrowse);
            // 
            // _dataSourceLabel
            // 
            _dataSourceLabel.AutoSize = true;
            _dataSourceLabel.Location = new System.Drawing.Point(-3, 0);
            _dataSourceLabel.Name = "_dataSourceLabel";
            _dataSourceLabel.Size = new System.Drawing.Size(101, 13);
            _dataSourceLabel.TabIndex = 2;
            _dataSourceLabel.Text = "&Database file name:";
            // 
            // _dataSourceTextBox
            // 
            this._dataSourceTextBox.AccessibleName = "Database file name";
            this._dataSourceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dataSourceTextBox.Location = new System.Drawing.Point(0, 18);
            this._dataSourceTextBox.Name = "_dataSourceTextBox";
            this._dataSourceTextBox.Size = new System.Drawing.Size(292, 20);
            this._dataSourceTextBox.TabIndex = 0;
            this._dataSourceTextBox.TextChanged += new System.EventHandler(this.HandleDataSourceChanged);
            // 
            // _openFileDialog
            // 
            this._openFileDialog.CheckFileExists = false;
            this._openFileDialog.DefaultExt = "db";
            this._openFileDialog.Filter = "SQLite Database Files|*.db;*.db3;*.sqlite;*.sqlite3;*.dat|All Files|*.*";
            this._openFileDialog.Title = "Open SQLite Database File";
            // 
            // SqliteConnectionUIControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(_dataSourceLabel);
            this.Controls.Add(_browseButton);
            this.Controls.Add(this._dataSourceTextBox);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SqliteConnectionUIControl";
            this.Size = new System.Drawing.Size(371, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _dataSourceTextBox;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
    }
}
