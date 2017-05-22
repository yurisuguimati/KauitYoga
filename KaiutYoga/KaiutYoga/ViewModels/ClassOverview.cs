using KaiutYoga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.ViewModels
{
    public class ClassOverview
    {
        public ClassModel Class { get; set; }
        public DateTime ClassDate { get; set; }
        public List<StudentModel> EnroledWeeklyStudents { get; set; }
        public List<StudentModel> EnroledTrialStudents { get; set; }
        public List<StudentModel> EnroledReplacementStudents { get; set; }
        public List<StudentModel> PresentWeeklyStudents { get; set; }
        public List<StudentModel> PresentTrialStudents { get; set; }
        public List<StudentModel> PresentReplacementStudents { get; set; }

        public ClassOverview(long classId, string date)
        {
            if (date != "")
            {
                DateTime dateValue;
                if (DateTime.TryParse(date, out dateValue))
                    this.ClassDate = dateValue;
                else
                    this.ClassDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(Convert.ToDouble(date));
            }
            KaiutYogaContext db = new KaiutYogaContext();
            this.Class = db.ClassModels.Include("Professor").Include("Room").Where(c => c.Id == classId).Single();
            List<PlanModel> lpm = db.PlanModels.ToList();
            if (this.HavePresenceList())
            {
                PresenceModel pm = this.Class.PresenceList(this.ClassDate);


                this.EnroledWeeklyStudents = pm.EnroledWeeklyStudents.ToList();
                foreach (StudentModel sm in this.EnroledWeeklyStudents)
                {
                    sm.Plan = lpm.Find(c => c.Id == sm.PlanId);
                }
                this.EnroledTrialStudents = pm.EnroledTrialStudents.ToList();
                foreach (StudentModel sm in this.EnroledTrialStudents)
                {
                    sm.Plan = lpm.Find(c => c.Id == sm.PlanId);
                }
                this.EnroledReplacementStudents = pm.EnroledReplacementStudents.ToList();
                foreach (StudentModel sm in this.EnroledReplacementStudents)
                {
                    sm.Plan = lpm.Find(c => c.Id == sm.PlanId);
                }

                this.PresentWeeklyStudents = pm.PresentWeeklyStudents.ToList();
                foreach (StudentModel sm in this.PresentWeeklyStudents)
                {
                    sm.Plan = lpm.Find(c => c.Id == sm.PlanId);
                }
                this.PresentTrialStudents = pm.PresentTrialStudents.ToList();
                foreach (StudentModel sm in this.PresentTrialStudents)
                {
                    sm.Plan = lpm.Find(c => c.Id == sm.PlanId);
                }
                this.PresentReplacementStudents = pm.PresentReplacementStudents.ToList();
                foreach (StudentModel sm in this.PresentReplacementStudents)
                {
                    sm.Plan = lpm.Find(c => c.Id == sm.PlanId);
                }
            }
            else
            {
                this.EnroledWeeklyStudents = this.Class.WeeklyStudents(this.ClassDate);
                this.EnroledTrialStudents = this.Class.TrialStudents(this.ClassDate);
                this.EnroledReplacementStudents = this.Class.ReplacementStudents(this.ClassDate);

                this.PresentWeeklyStudents = new List<StudentModel>();
                this.PresentTrialStudents= new List<StudentModel>();
                this.PresentReplacementStudents = new List<StudentModel>();
            }
        }


        public List<StudentModel> AllEnroledStudents
        {
            get
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
            set { }
        }
        public List<StudentModel> AllPresentStudents
        {
            get
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
            set { }
        }


        public bool HavePresenceList()
        {
            KaiutYogaContext db = new KaiutYogaContext();
            List<PresenceModel> lpm = db.PresenceModels.Include("EnroledWeeklyStudents").Include("PresentWeeklyStudents").Where(c => c.ClassId == this.Class.Id).Where(c => c.Date == this.ClassDate).ToList();

            return lpm.Count > 0;
        }

        public PresenceModel PresenceList()
        {
            KaiutYogaContext db = new KaiutYogaContext();
            List<PresenceModel> lpm = db.PresenceModels.Include("EnroledWeeklyStudents").Include("PresentWeeklyStudents").Where(c => c.ClassId == this.Class.Id).Where(c => c.Date == this.ClassDate).ToList();
            if (lpm == null || lpm.Count == 0)
                return null;
            return lpm.First();
        }

        public double DateInMilliseconds()
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0); //Set default date 1/1/1970
            return (ClassDate - start).TotalMilliseconds;
        }

    }
}
