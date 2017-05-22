using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KaiutYoga.Models
{
    public class SpareClassModel
    {
        [Key]
        public long Id { get; set; }
        public bool IsTrial { get; set; } = false;

        // quando adicionar uma aula de reposição, adicionar os dados da antiga aqui
        public DateTime? OldDate { get; set; }
        public long? OldClassId { get; set; }
        [ForeignKey("OldClassId")]
        public ClassModel OldClass { get; set; }

        public DateTime NewDate { get; set; }
        public long NewClassId { get; set; }
        [ForeignKey("NewClassId")]
        public ClassModel NewClass { get; set; }
     
        public string GetMilliseconds
        {
            get {
                
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return (this.NewDate - dt).TotalMilliseconds.ToString();
                //return this.NewDate.ToString("yyyy-MM-ddTHH:mm:ss");
                //DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //return TimeZoneInfo.ConvertTimeFromUtc(this.NewDate, TimeZoneInfo.Local).ToString("yyyy-MM-ddTHH:mm:ss");
                //return this.NewDate.Year + "-" + this.NewDate.Month + "-" + this.NewDate.Day + "T" + this.NewDate.Hour + ":" + this.NewDate.Minute + ":00";
                //return (this.Date - dt).TotalMilliseconds;
            }
            set { }
        }   

        public PresenceModel PresenceList()
        {
            if (NewClass != null)
                return NewClass.PresenceList(NewDate);
            else if (NewClassId != 0)
            {
                KaiutYogaContext db = new KaiutYogaContext();
                ClassModel cm = db.ClassModels.Find(NewClassId);
                return cm.PresenceList(NewDate);
            }
            return null;
        }
    }
}