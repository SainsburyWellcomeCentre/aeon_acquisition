using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using MySqlConnector;

namespace Aeon.Database
{
    [Description("Enumerates all records in the colony table for each MySQL connection in the sequence.")]
    public class EnumerateColony : Combinator<MySqlConnection, ColonyRecord>
    {
        public override IObservable<ColonyRecord> Process(IObservable<MySqlConnection> source)
        {
            return source.SelectMany(connection => ObservableDatabase.Query<ColonyRecord>(
                "SELECT * FROM `#colony`;",
                connection));
        }
    }

    public class ColonyRecord
    {
        public string Subject { get; set; }

        public float? ReferenceWeight { get; set; }

        public SubjectSex Sex { get; set; }

        public DateTime? SubjectBirthDate { get; set; }

        public string Note { get; set; }

        public override string ToString()
        {
            return $"({Subject}, wt: {ReferenceWeight}, sex: {Sex}, dob: {SubjectBirthDate}, {Note})";
        }
    }

    public enum SubjectSex
    {
        Male = 'M',
        Female = 'F',
        Unspecified = 'U'
    }
}
