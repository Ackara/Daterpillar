using System.IO;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    public abstract class FlywayCmdletWrapper : Cmdlet
    {
        private string _migrationDirectory = null;

        // ===== Parameter Set: all ===== //

        /// <summary>
        /// <para type="description">The SQL language of the script.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("lang", "type", "connection-type")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public Language ConnectionType { get; set; }

        /// <summary>
        /// <para type="description">The absolute-path of the directory that host the migration scripts.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("path", "fullname", "sql")]
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 6)]
        public string MigrationsDirectory
        {
            get { return (string.IsNullOrEmpty(_migrationDirectory) ? InpuObject : _migrationDirectory); }
            set { _migrationDirectory = value; }
        }

        /// <summary>
        /// <para type="description">The absolute-path of the flyway executable file.</para>
        /// </summary>
        [Alias("flyway")]
        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string FlywayFilePath { get; set; }

        /// <summary>
        /// <para type="description">The operation timeout interval in seconds. Defaults to 5 minutes.</para>
        /// </summary>
        [ValidateRange(1, int.MaxValue)]
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public int Timeout { get; set; }

        /// <summary>
        /// <para type="description">The absolute-path of the directory that host the migration scripts.</para>
        /// </summary>
        [Parameter(ValueFromPipeline = true)]
        public string InpuObject { get; set; }

        // ===== Parameter Set: default ===== //

        /// <summary>
        /// <para type="description">The server address.</para>
        /// </summary>
        [Alias("h")]
        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 2)]
        public string Host { get; set; }

        /// <summary>
        /// <para type="description">The authorized user.</para>
        /// </summary>
        [Alias("u")]
        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 3)]
        public string User { get; set; }

        /// <summary>
        /// <para type="description">The database password.</para>
        /// </summary>
        [Alias("p")]
        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 4)]
        public string Password { get; set; }

        /// <summary>
        /// <para type="description">The database name.</para>
        /// </summary>
        [Alias("d")]
        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 5)]
        public string Database { get; set; }

        // ===== Parameter Set: connection-string ===== //

        /// <summary>
        /// Gets or sets the connection string.
        /// <para type="description">The database connection-string. Must be formatted as &lt;key&gt;=&lt;value&gt;</para>
        /// </summary>
        [Alias("c", "connstr")]
        [ValidateNotNullOrEmpty]
        [ValidatePattern("(?i)[^=;]+=[^=;]+")]
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string ConnectionString { get; set; }

        protected override void BeginProcessing()
        {
            if (Timeout <= 0) Timeout = Flyway.DEFAULT_TIMEOUT;

            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = $"server={Host};user={User};password={Password};database={Database};";

            if (string.IsNullOrEmpty(FlywayFilePath))
                FlywayFilePath = Flyway.Install();
            else
                FlywayFilePath = FlywayFilePath.Expand();

            if (!File.Exists(FlywayFilePath))
                throw new FileNotFoundException($"Cound not find flyway at '{FlywayFilePath}'; you can download it from 'https://flywaydb.org/download'.");
        }

        protected PSObject CreateInputObject()
        {
            var obj = new PSObject();

            obj.Members.Add(new PSNoteProperty(nameof(ConnectionType), ConnectionType));
            obj.Members.Add(new PSNoteProperty(nameof(FlywayFilePath), FlywayFilePath));
            obj.Members.Add(new PSNoteProperty(nameof(MigrationsDirectory), MigrationsDirectory));
            obj.Members.Add(new PSNoteProperty(nameof(Timeout), Timeout));

            obj.Members.Add(new PSNoteProperty(nameof(Host), Host));
            obj.Members.Add(new PSNoteProperty(nameof(User), User));
            obj.Members.Add(new PSNoteProperty(nameof(Password), Password));
            obj.Members.Add(new PSNoteProperty(nameof(Database), Database));

            obj.Members.Add(new PSNoteProperty(nameof(ConnectionString), ConnectionString));

            return obj;
        }
    }
}