using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErtityFramework.Database.MsSql
{
    /*
     Server=ertugrulo\SQLEXPRESS;
     Database=en_otc;
     Trusted_Connection=Yes;
     MultipleActiveResultSets=True
    */

    /*
     Data Source=192.168.108.43;
     Initial Catalog=en_load;
     User Id=mtxenerji;
     Password=Qq123456;
     Max Pool Size=5000;
     Min Pool Size=1; 
    */

    public class MsSqlConnectionString : IConnectionString
    {
        private int? port = null;
        private int? maxPoolSize = null;
        private int? minPoolSize = null;

        public bool IsLocalServer { get; set; }

        public string Server { get; set; }

        public int Port
        {
            get
            {
                if (this.port == null)
                    this.port = 3306;

                return this.port.Value;
            }

            set
            {
                this.port = value;
            }
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public bool? MultipleActiveResultSets { get; set; }

        public bool? Trusted_Connection { get; set; }

        public string Charset { get; set; }

        public int MaxPoolSize
        {
            get
            {
                if (this.maxPoolSize == null)
                    this.maxPoolSize = 5000;

                return this.maxPoolSize.Value;
            }

            set
            {
                this.maxPoolSize = value;
            }
        }

        public int MinPoolSize
        {
            get
            {
                if (this.minPoolSize == null)
                    this.minPoolSize = 1;

                return this.minPoolSize.Value;
            }

            set
            {
                this.minPoolSize = value;
            }
        }

        public MsSqlConnectionString()
        {

        }

        public MsSqlConnectionString(string connectionString)
        {
            this.SetConnectionFields(connectionString);
        }

        public override string ToString()
        {
            if (this.IsLocalServer)
            {
                string serverPart = this.Server != null ? string.Format("Server={0};", this.Server) : string.Empty;
                string databasePart = this.Database != null ? string.Format("Database={0};", this.Database) : string.Empty;
                string trustedConnectionPart = this.Trusted_Connection != null ? string.Format("Trusted_Connection={0};", (this.Trusted_Connection.Value ? "Yes" : "No")) : string.Empty;
                string marsPart = this.MultipleActiveResultSets != null ? string.Format("MultipleActiveResultSets={0};", this.MultipleActiveResultSets.ToString()) : string.Empty;

                return serverPart + databasePart + trustedConnectionPart + marsPart;
            }
            else
            {
                string serverPart = this.Server != null ? string.Format("Data Source={0};", this.Server) : string.Empty;
                string databasePart = this.Database != null ? string.Format("Initial Catalog={0};", this.Database) : string.Empty;
                string portPart = this.port != null ? string.Format("Port={0};", this.port) : string.Empty;
                string usernamePart = this.Username != null ? string.Format("User Id={0};", this.Username) : string.Empty;
                string passwordPart = this.Password != null ? string.Format("Password={0};", this.Password) : string.Empty;
                string maxPoolSizePart = this.maxPoolSize != null ? string.Format("Max Pool Size={0};", this.maxPoolSize) : string.Empty;
                string minPoolSizePart = this.minPoolSize != null ? string.Format("Min Pool Size={0};", this.minPoolSize) : string.Empty;
                string marsPart = this.MultipleActiveResultSets != null ? string.Format("MultipleActiveResultSets={0};", this.MultipleActiveResultSets.ToString()) : string.Empty;
                string trustedConnectionPart = this.Trusted_Connection != null ? string.Format("Trusted_Connection={0};", (this.Trusted_Connection.Value ? "Yes" : "No")) : string.Empty;
                string chartsetPart = this.Charset != null ? string.Format("Charset={0};", this.Charset) : string.Empty;

                return serverPart + databasePart + portPart + usernamePart + passwordPart + maxPoolSizePart + minPoolSizePart + marsPart + trustedConnectionPart + chartsetPart;
            }
        }

        internal static string ExtractValue(string fieldPart)
        {
            if (fieldPart == null)
                return null;

            if (string.IsNullOrEmpty(fieldPart))
                return string.Empty;

            int indexOfEqual = fieldPart.IndexOf('=');
            if (indexOfEqual > 0)
                return fieldPart.Substring(indexOfEqual + 1);
            else
                return null;
        }

        private void SetConnectionFields(string connString)
        {
            var connStringParts = connString.Split(';');

            string serverPart = connStringParts.FirstOrDefault(x => x.StartsWith("Server=", StringComparison.InvariantCulture));
            this.Server = MsSqlConnectionString.ExtractValue(serverPart);

            serverPart = connStringParts.FirstOrDefault(x => x.StartsWith("Data Source=", StringComparison.InvariantCulture));
            this.Server = MsSqlConnectionString.ExtractValue(serverPart);

            string userPart = connStringParts.FirstOrDefault(x => x.StartsWith("User Id=", StringComparison.InvariantCulture));
            this.Username = MsSqlConnectionString.ExtractValue(userPart);

            string passwordPart = connStringParts.FirstOrDefault(x => x.StartsWith("Password=", StringComparison.InvariantCulture));
            this.Password = MsSqlConnectionString.ExtractValue(passwordPart);

            string databasePart = connStringParts.FirstOrDefault(x => x.StartsWith("Database=", StringComparison.InvariantCulture));
            this.Database = MsSqlConnectionString.ExtractValue(databasePart);

            databasePart = connStringParts.FirstOrDefault(x => x.StartsWith("Initial Catalog=", StringComparison.InvariantCulture));
            this.Database = MsSqlConnectionString.ExtractValue(databasePart);

            string marsPart = connStringParts.FirstOrDefault(x => x.StartsWith("MultipleActiveResultSets=", StringComparison.InvariantCulture));
            this.MultipleActiveResultSets = Convert.ToBoolean(MsSqlConnectionString.ExtractValue(marsPart).ToLower());

            string trustedConnPart = connStringParts.FirstOrDefault(x => x.StartsWith("Trusted_Connection=", StringComparison.InvariantCulture));
            this.Trusted_Connection = MsSqlConnectionString.ExtractValue(trustedConnPart) == "Yes";

            string portPart = connStringParts.FirstOrDefault(x => x.StartsWith("Port=", StringComparison.InvariantCulture));
            this.Port = Convert.ToInt32(MsSqlConnectionString.ExtractValue(portPart));

            string maxPoolSizePart = connStringParts.FirstOrDefault(x => x.StartsWith("Max Pool Size=", StringComparison.InvariantCulture));
            this.MaxPoolSize = Convert.ToInt32(MsSqlConnectionString.ExtractValue(maxPoolSizePart));

            string minPoolSizePart = connStringParts.FirstOrDefault(x => x.StartsWith("Min Pool Size=", StringComparison.InvariantCulture));
            this.MinPoolSize = Convert.ToInt32(MsSqlConnectionString.ExtractValue(minPoolSizePart));

            string charsetPart = connStringParts.FirstOrDefault(x => x.StartsWith("Charset=", StringComparison.InvariantCulture));
            this.Charset = MsSqlConnectionString.ExtractValue(charsetPart);
        }
    }
}
