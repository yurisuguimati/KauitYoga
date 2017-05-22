using KaiutYoga.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaiutYoga.Models
{

    public class ClassModel
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Professor")]
        public long ProfessorId { get; set; }
        [ForeignKey("ProfessorId")]
        public ProfessorModel Professor { get; set; }

        [DisplayName("Sala")]
        public long RoomId { get; set; }
        [ForeignKey("RoomId ")]
        public RoomModel Room { get; set; }

        [Display(Name = "Capacidade")]
        public int Capacity
        {
            get
            {
                //this.GetProfessor();
                RoomModel rm = this.GetRoom();
                return rm.Capacity + rm.SpareCapacity;
            }
            set { }
        }

        [Display(Name = "Horário")]
        public IEnumerable<SelectListItem> Time { get; set; }

        [Display(Name = "Horário")]
        public string SelectedTime { get; set; }
        [Display(Name = "Dia da semana")]
        public DayOfWeek Day { get; set; }

        public bool Active { get; set; }

        public ClassModel()
        {
            this.Active = true;
        }
        
        public virtual ICollection<PresenceModel> Presence { get; set; }
        public virtual ICollection<StudentModel> Students { get; set; }

        public int AmountEnroledStudents(KaiutYogaContext context = null)
        {
            if (context == null)
                context = new KaiutYogaContext();
            ClassRepository cr = new ClassRepository(context);
            return cr.GetStudentsInClass(this.Id).Count;
        }

        public List<StudentModel> AllEnroledStudents(KaiutYogaContext context = null)
        {
            if (context == null)
                context = new KaiutYogaContext();
            ClassRepository cr = new ClassRepository(context);
            return cr.GetStudentsInClass(this.Id);
        }

        public List<StudentModel> WeeklyStudents(List<StudentModel> liststudentmodel)
        {
            
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in liststudentmodel)
            {
                if (sm.Classes.ToList().Exists(i => i.Id == this.Id))
                    lsm.Add(sm);
            }

            return lsm;
        }

        public List<StudentModel> WeeklyStudents()
        {
            KaiutYogaContext context = new KaiutYogaContext();
            List<StudentModel> liststudentmodel = context.StudentModels.ToList();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in liststudentmodel)
            {
                if (sm.Classes.ToList().Exists(i => i.Id == this.Id))
                    lsm.Add(sm);
            }

            return lsm;
        }

        public List<StudentModel> WeeklyStudents(DateTime classDate, KaiutYogaContext context = null)
        {
            if (context == null)
                context = new KaiutYogaContext();
            ClassRepository cr = new ClassRepository(context);
            return cr.GetWeeklyStudentsInClass(this.Id,classDate);
        }

        // All extra students
        public List<StudentModel> SpareStudents(KaiutYogaContext context = null)
        {
            if (context == null)
                context = new KaiutYogaContext();
            ClassRepository cr = new ClassRepository(context);
            return cr.GetSpareStudentsInClass(this.Id);
        }

        // Replacement students
        public List<StudentModel> ReplacementStudents(KaiutYogaContext context = null)
        {
            if (context == null)
                context = new KaiutYogaContext();
            ClassRepository cr = new ClassRepository(context);
            return cr.GetReplacementStudentsInClass(this.Id);
        }

        public List<StudentModel> ReplacementStudents(DateTime classDate, KaiutYogaContext context = null)
        {
            if (context == null)
                context = new KaiutYogaContext();
            ClassRepository cr = new ClassRepository(context);
            return cr.GetReplacementStudentsInClass(this.Id, classDate);
        }

        //Trial students
        public List<StudentModel> TrialStudents(KaiutYogaContext context = null)
        {
            if (context == null)
                context = new KaiutYogaContext();
            ClassRepository cr = new ClassRepository(context);
            return cr.GetTrialStudentsInClass(this.Id);
        }

        public List<StudentModel> TrialStudents(DateTime classDate, KaiutYogaContext context = null)
        {
            if (context == null)
                context = new KaiutYogaContext();
            ClassRepository cr = new ClassRepository(context);
            return cr.GetTrialStudentsInClass(this.Id, classDate);
        }

        public IEnumerable<ClassModel> GetClasses(KaiutYogaContext context)
        {
            ClassRepository cr = new ClassRepository(context);
            List<ClassModel> lcm = cr.GetClasses().ToList();
            return lcm;
        }

        public string ClassColor()
        {
            //if (this.AmountEnroledStudents() >= this.Capacity)
            if (this.WeeklyStudents().Count >= this.Capacity)
                return "Gray";
            if (this.WeeklyStudents().Count > this.Capacity - 5)
                return "Red";
            return "#800000";
        }

        public ProfessorModel GetProfessor()
        {
            if (this.Professor != null)
                return this.Professor;
            KaiutYogaContext context = new KaiutYogaContext();
            ProfessorRepository pr = new ProfessorRepository(context);
            return pr.GetProfessor(this.ProfessorId);
        }

        public RoomModel GetRoom()
        {
            if (this.Room != null)
                return this.Room;
            KaiutYogaContext context = new KaiutYogaContext();
            RoomRepository rr = new RoomRepository(context);
            return rr.GetRoom(this.RoomId);
        }

        public bool HaveAnyPresenceList()
        {
            KaiutYogaContext db = new KaiutYogaContext();

            List<PresenceModel> lpm = db.PresenceModels
                .Include("EnroledWeeklyStudents").Include("PresentWeeklyStudents")
                .Include("EnroledTrialStudents").Include("PresentTrialStudents")
                .Include("EnroledReplacementStudents").Include("PresentReplacementStudents")
                .Where(c => c.ClassId == this.Id).ToList();

            return lpm.Count > 0;
        }

        public bool HavePresenceList(string dt)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(Convert.ToDouble(dt));
            KaiutYogaContext db = new KaiutYogaContext();

            List<PresenceModel> lpm = db.PresenceModels
                .Include("EnroledWeeklyStudents").Include("PresentWeeklyStudents")
                .Include("EnroledTrialStudents").Include("PresentTrialStudents")
                .Include("EnroledReplacementStudents").Include("PresentReplacementStudents")
                .Where(c => c.ClassId == this.Id).Where(c => c.Date == date).ToList();

            return lpm.Count > 0;
        }

        public PresenceModel PresenceList(DateTime dt)
        {
            KaiutYogaContext db = new KaiutYogaContext();

            List<PresenceModel> lpm = db.PresenceModels
                .Include("EnroledWeeklyStudents").Include("PresentWeeklyStudents")
                .Include("EnroledTrialStudents").Include("PresentTrialStudents")
                .Include("EnroledReplacementStudents").Include("PresentReplacementStudents")
                .Where(c => c.ClassId == this.Id).Where(c => c.Date == dt).ToList();

            if (lpm.Count == 0)
                return null;
            return lpm.First();
        }
    }
}