# ErtityFramework
MidiORM for MySql&amp;MsSql

##### Entity Classes #####
```c#
using ErtityFramework.Data;
using ErtityFramework.Entities;
using ErtityFramework.Scheme;

[TableInfo("TableName")]
public class SampleEntity : EntityBase
{
    [ColumnInfo("Name", DbType = DatabaseType.VARCHAR, PropertyName = "Name")]
    public string Name { get; set; }

    [ColumnInfo("SurName", DbType = DatabaseType.VARCHAR, PropertyName = "Surname")]
    public string Surname { get; set; }

    [ColumnInfo("SessionID", DbType = DatabaseType.VARCHAR, PropertyName = "SessionId")]
    public string SessionId { get; set; }

    public SampleEntity(long id) : base(id)
    {

    }

    public override string ToString()
    {
        return string.Format("[Id:{0}] ", this.Id) + string.Format("{0} {1}", this.Name, this.Surname);
    }
}
```

##### Connection & Mapping #####
```c#
MsSqlConnectionString connectionString = new MsSqlConnectionString("yourconnectionstring");

// Connection
var databaseConnector = new MsSqlDatabaseConnector(connectionString);
this.Database = new MsSqlDatabase(databaseConnector);
this.Database.Mapping.RegisterTableModel<SampleEntity>();

// Fetch & Bind Data
Task.Run(() =>
{
    try
    {
        var sampleTable = this.Database.GetTable<SampleEntity>();
        var list = sampleTable.Select();

        UIDispatcher.UiInvoke(() =>
        {
            if (list != null)
                this.DataCollection = new ObservableRangeCollection<SampleEntity>(list);
        });
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Error");
    }
});

```
