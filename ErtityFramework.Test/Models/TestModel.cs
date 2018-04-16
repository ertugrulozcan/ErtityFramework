using System;
using ErtityFramework.Scheme;
using ErtityFramework.Data;
using ErtityFramework.Entities;

namespace ErtityFramework.Test.Models
{
    [TableInfo("Credentials")]
    internal class TestModel : EntityBase
    {
        #region Properties

        [ColumnInfo("Name", DbType = DatabaseType.VARCHAR)]
        public string Name { get; set; }

        [ColumnInfo("Surname", DbType = DatabaseType.VARCHAR)]
        public string Surname { get; set; }

        [ColumnInfo("EmailAddress", DbType = DatabaseType.VARCHAR)]
        public string EmailAddress { get; set; }

        [ColumnInfo("PhoneNumber", DbType = DatabaseType.VARCHAR)]
        public string PhoneNumber { get; set; }

        [ColumnInfo("BirthDate", DbType = DatabaseType.DATE)]
        public DateTime? BirthDate { get; set; }

        #endregion

        #region Constructors

        public TestModel(long id) : base(id)
        {

        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("[Id:{0}] ", this.Id) + string.Format("{0} {1} (EmailAddress : {2}, PhoneNumber : {3}, BirthDate : {4})", this.Name, this.Surname, this.EmailAddress, this.PhoneNumber, this.BirthDate);
        }

        #endregion
    }
}
