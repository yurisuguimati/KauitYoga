using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KaiutYoga.Models
{
    public class PresenceModel
    {
        [Key]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public long ClassId { get; set; }

        /// <summary>
        /// Weekly students that were in that class
        /// </summary>
        public virtual ICollection<StudentModel> PresentWeeklyStudents { get; set; }
        /// <summary>
        /// Weekly students that were enroled to that class
        /// </summary>
        public virtual ICollection<StudentModel> EnroledWeeklyStudents { get; set; }
        /// <summary>
        /// Trial students that were in that class
        /// </summary>
        public virtual ICollection<StudentModel> PresentTrialStudents { get; set; }
        /// <summary>
        /// Trial students that were enroled to that class
        /// </summary>
        public virtual ICollection<StudentModel> EnroledTrialStudents { get; set; }
        /// <summary>
        /// Weekly students that were in that class
        /// </summary>
        public virtual ICollection<StudentModel> PresentReplacementStudents { get; set; }
        /// <summary>
        /// Weekly students that were enroled to that class
        /// </summary>
        public virtual ICollection<StudentModel> EnroledReplacementStudents { get; set; }

        public string ConvertToString
        {
            get
            {
                
                //return TimeZoneInfo.ConvertTimeFromUtc(this.Date, TimeZoneInfo.Local).ToString("yyyy-MM-ddTHH:mm:ss");
                return this.Date.ToString("yyyy-MM-ddTHH:mm:ss");
                //return this.Date.Year + "-" + this.Date. + "-" + this.Date.Day + "T" + this.Date.Hour + ":" + this.Date.Minute + ":00";
                //return (this.Date - dt).TotalMilliseconds;
            }
            set { }
        }

        public string ConvertToMilliseconds
        {
            get
            {
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //return this.Date.ToString("yyyy-MM-ddTHH:mm:ss");
                //return this.Date.Year + "-" + this.Date. + "-" + this.Date.Day + "T" + this.Date.Hour + ":" + this.Date.Minute + ":00";
                return (this.Date - dt).TotalMilliseconds.ToString();
            }
            set { }
        }

        public List<StudentModel> AllPresentStudents()
        {
            List<StudentModel> lsm = new List<StudentModel>();
            if (PresentWeeklyStudents != null)
                lsm = lsm.Concat(PresentWeeklyStudents).ToList();
            if (PresentTrialStudents != null)
                lsm = lsm.Concat(PresentTrialStudents).ToList();
            if (PresentReplacementStudents != null)
                lsm = lsm.Concat(PresentReplacementStudents).ToList();

            return lsm;
        }

        public List<StudentModel> AllEnroledStudents()
        {
            List<StudentModel> lsm = new List<StudentModel>();
            if (EnroledWeeklyStudents != null)
                lsm = lsm.Concat(EnroledWeeklyStudents).ToList();
            if (EnroledTrialStudents != null)
                lsm = lsm.Concat(EnroledTrialStudents).ToList();
            if (EnroledReplacementStudents != null)
                lsm = lsm.Concat(EnroledReplacementStudents).ToList();

            return lsm;
        }


    }
}