using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using MySqlConnector;

namespace Aeon.Database
{
    [DefaultProperty(nameof(QueryString))]
    [Description("Runs the specified SQL query against each MySQL connection in the sequence.")]
    public class ExecuteQuery : Combinator<MySqlConnection, MySqlDataReader>
    {
        [Editor("Bonsai.Design.RichTextEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        [Description("Specifies the full SQL query to run against the MySQL connection.")]
        public string QueryString { get; set; }

        public override IObservable<MySqlDataReader> Process(IObservable<MySqlConnection> source)
        {
            return source.SelectMany(connection => ObservableDatabase.Query(QueryString, connection));
        }
    }
}
