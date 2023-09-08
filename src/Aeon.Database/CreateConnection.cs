using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai;
using MySqlConnector;

namespace Aeon.Database
{
    [DefaultProperty(nameof(ConnectionString))]
    [Description("Creates a connection to the MySQL server using the specified connection string.")]
    public class CreateConnection : Source<MySqlConnection>
    {
        [Editor("Bonsai.Design.RichTextEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        [Description("Specifies the parameters used to establish a connection to the MySQL server.")]
        public string ConnectionString { get; set; }

        public override IObservable<MySqlConnection> Generate()
        {
            return Observable.Defer(async () =>
            {
                var connection = new MySqlConnection(ConnectionString);
                await connection.OpenAsync();
                return Observable.Return(connection);
            });
        }
    }
}
