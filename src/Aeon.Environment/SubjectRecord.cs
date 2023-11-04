using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.Environment
{
    public class SubjectRecord
    {
        [Column("subject")]
        public string Id { get; set; }

        [Column("sex")]
        public SubjectSex Sex { get; set; }

        [Column("subject_birth_date")]
        public DateTime? BirthDate { get; set; }

        [Column("subject_description")]
        public string Description { get; set; }

        [Column("lab_id")]
        public string LabId { get; set; }

        [Column("available")]
        public bool Available { get; set; }

        public override string ToString()
        {
            return $"({Id}, " +
                $"sex: {Sex}, " +
                $"dob: {BirthDate}, " +
                $"description: {Description}, " +
                $"lab_id: {LabId}, " +
                $"available: {Available})";
        }
    }

    public enum SubjectSex
    {
        Male = 'M',
        Female = 'F',
        Unspecified = 'U'
    }
}
