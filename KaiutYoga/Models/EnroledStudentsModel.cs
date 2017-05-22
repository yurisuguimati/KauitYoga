using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KaiutYoga.Models
{
    public class EnroledStudentsModel
    {
        [Key]
        public long Id { get; set; }
        public long StudentId { get; set; }
        [ForeignKey("StudentId")]
        public StudentModel Student { get; set; }

        public void ConvertToEnroledStudents(StudentModel sm)
        {
            this.StudentId = sm.Id;
            /*this.FirstName = sm.FirstName;
            this.LastName = sm.LastName;
            this.Classes = sm.Classes;
            this.BirthDate = sm.BirthDate;
            this.Presence = sm.Presence;
            this.PlanId = sm.PlanId;
            this.Observations = sm.Observations;
            */
        }

    }
}