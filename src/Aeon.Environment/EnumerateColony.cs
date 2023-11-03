using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Aeon.Database;
using Bonsai;
using MySqlConnector;

namespace Aeon.Environment
{
    [Description("Enumerates all records in the colony table for each MySQL connection in the sequence.")]
    public class EnumerateColony : Combinator<MySqlConnection, SubjectRecord>
    {
        internal static IObservable<SubjectRecord> Query(MySqlConnection connection)
        {
            return ObservableDatabase.Query<SubjectRecord>(
                "SELECT `subject`,`sex`,`subject_birth_date`,`subject_description`,`lab_id`," +
                "`responsible_fullname`,`gen_bg_id`,`strain_id`,`cage_number`,`available` " +
                "FROM `aeon_subject`.`subject` NATURAL JOIN `aeon_subject`.`_subject_detail`;",
                connection);
        }

        public override IObservable<SubjectRecord> Process(IObservable<MySqlConnection> source)
        {
            return source.SelectMany(Query);
        }
    }
}
