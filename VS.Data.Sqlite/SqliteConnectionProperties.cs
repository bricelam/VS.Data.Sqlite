using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;

namespace Microsoft.VisualStudio.Data.Sqlite;

class SqliteConnectionProperties : AdoDotNetConnectionProperties
{
    static SqliteConnectionProperties()
        => TypeDescriptor.AddProvider(
            new AssociatedMetadataTypeTypeDescriptionProvider(
                typeof(SqliteConnectionStringBuilder),
                typeof(SqliteConnectionStringBuilderMetadata)),
            typeof(SqliteConnectionStringBuilder));

    protected override void OnSiteChanged(EventArgs e)
    {
        base.OnSiteChanged(e);

        if (Site is not null)
        {
            ConnectionStringBuilder.BrowsableConnectionString = false;
        }
    }

    public override bool IsComplete => !string.IsNullOrEmpty(ConnectionStringBuilder["DataSource"]?.ToString()) &&
        (File.Exists(ConnectionStringBuilder["DataSource"].ToString())
            || ConnectionStringBuilder["DataSource"].ToString().Equals(":memory:", StringComparison.Ordinal));

    class SqliteConnectionStringBuilderMetadata
    {
        [Category("Source")]
        [DisplayName("Data Source")]
        [Description("The path to the database file.")]
        public string DataSource { get; set; }

        [Category("Source")]
        [Description("The connection mode.")]
        public SqliteOpenMode Mode { get; set; }

        [Category("Advanced")]
        [Description("The caching mode used by the connection.")]
        public SqliteCacheMode Cache { get; set; }

        [Category("Security")]
        [Description("The encryption key.")]
        [PasswordPropertyText(true)]
        public string Password { get; set; }

        [Category("Advanced")]
        [DisplayName("Foreign Keys")]
        [Description("A value indicating whether to enable foreign key constraints.")]
        public bool? ForeignKeys { get; set; }

        [Category("Advanced")]
        [DisplayName("Recursive Triggers")]
        [Description("A value that indicates whether to enable recursive triggers.")]
        public bool RecursiveTriggers { get; set; }

        [Category("Advanced")]
        [DisplayName("Default Timeout")]
        [Description("The default timeout value for commands.")]
        public int DefaultTimeout { get; set; }

        [Category("Pooling")]
        [Description("A value indicating whether the connection will be pooled.")]
        public bool Pooling { get; set; }
    }
}
