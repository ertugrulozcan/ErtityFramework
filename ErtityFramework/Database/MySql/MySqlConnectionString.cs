using System;
using System.Linq;

namespace ErtityFramework.Database.MySql
{
    public class MySqlConnectionString : IConnectionString
    {
        private int? port = null;

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

        public bool? PersistSecurityInfo { get; set; }

        public string Charset { get; set; }

        public MySqlConnectionString()
        {

        }

        public MySqlConnectionString(string connectionString)
        {
            this.SetConnectionFields(connectionString);
        }

        public override string ToString()
        {
            string serverPart = this.Server != null ? string.Format("server={0};", this.Server) : string.Empty;
            string usernamePart = this.Username != null ? string.Format("user id={0};", this.Username) : string.Empty;
            string passwordPart = this.Password != null ? string.Format("password={0};", this.Password) : string.Empty;
            string psiPart = this.PersistSecurityInfo != null ? string.Format("persistsecurityinfo={0};", this.PersistSecurityInfo.ToString().ToLower()) : string.Empty;
            string portPart = this.port != null ? string.Format("port={0};", this.port) : string.Empty;
            string databasePart = this.Database != null ? string.Format("database={0};", this.Database) : string.Empty;
            string chartsetPart = this.Charset != null ? string.Format("charset={0};", this.Charset) : string.Empty;

            return serverPart + usernamePart + passwordPart + psiPart + portPart + databasePart + chartsetPart;
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

            string serverPart = connStringParts.FirstOrDefault(x => x.StartsWith("server=", StringComparison.InvariantCulture));
            this.Server = MySqlConnectionString.ExtractValue(serverPart);

            string userPart = connStringParts.FirstOrDefault(x => x.StartsWith("user id=", StringComparison.InvariantCulture));
            this.Username = MySqlConnectionString.ExtractValue(userPart);

            string passwordPart = connStringParts.FirstOrDefault(x => x.StartsWith("password=", StringComparison.InvariantCulture));
            this.Password = MySqlConnectionString.ExtractValue(passwordPart);

            string psiPart = connStringParts.FirstOrDefault(x => x.StartsWith("persistsecurityinfo=", StringComparison.InvariantCulture));
            this.PersistSecurityInfo = Convert.ToBoolean(MySqlConnectionString.ExtractValue(psiPart).ToLower());

            string portPart = connStringParts.FirstOrDefault(x => x.StartsWith("port=", StringComparison.InvariantCulture));
            this.Port = Convert.ToInt32(MySqlConnectionString.ExtractValue(portPart));

            string databasePart = connStringParts.FirstOrDefault(x => x.StartsWith("database=", StringComparison.InvariantCulture));
            this.Database = MySqlConnectionString.ExtractValue(databasePart);

            string charsetPart = connStringParts.FirstOrDefault(x => x.StartsWith("charset=", StringComparison.InvariantCulture));
            this.Charset = MySqlConnectionString.ExtractValue(charsetPart);
        }
    }
}
