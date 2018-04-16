using System;
namespace ErtityFramework.Database.MySql
{
    public class MySqlConnectionString
    {
        public string Server { get; set; }

        public string Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public bool PersistSecurityInfo { get; set; }

        public string Charset { get; set; }

        public MySqlConnectionString()
        {

        }

        public override string ToString()
        {
            return string.Format("server={0};user id={1};password={2};persistsecurityinfo={3};port={4};database={5};charset={6};",
                                 this.Server, this.Username, this.Password, this.PersistSecurityInfo, this.Port, this.Database, this.Charset);
        }

        internal static string ExtractValue(string fieldPart)
        {
            int indexOfEqual = fieldPart.IndexOf('=');
            if (indexOfEqual > 0)
                return fieldPart.Substring(indexOfEqual + 1);
            else
                return null;
        }
    }
}
