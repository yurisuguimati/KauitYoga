using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;

namespace KaiutYoga.Models
{
    public enum Gender { Masculino, Feminino }
    public enum MaritalStatus { Solteiro, Casado, Viúvo, Divorciado}

    public class SkipPropertyAttribute : Attribute
    {
    }
    public static class TypeExtensions
    {
        public static PropertyInfo[] GetFilteredProperties(this Type type)
        {
            return type.GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(SkipPropertyAttribute))).ToArray();
        }
    }

    public class StudentModel
    {
        [Key]
        public long Id { get; set; }
        [DisplayName("Nome completo")]
        public string Name
        {
            get { return FirstName + " " + LastName; }
        }

        [DisplayName("Ativo")]
        public bool Active { get; set; }

        [DisplayName("Nome")]
        public string FirstName { get; set; }
        [DisplayName("Sobrenome")]
        public string LastName { get; set; }

        [Phone]
        [DisplayName("Telefone residencial")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [EmailAddress]
        [DisplayName("E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Gênero")]
        public Gender Gender { get; set; }

        [DisplayName("Data de nascimento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [DisplayName("Idade")]
        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - this.BirthDate.Year;
                if (this.BirthDate > today.AddYears(-age)) age--;

                return age;
            }
            set { }
        }


        [DisplayName("Data de cadastro")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RegisterDate { get; set; }
        
        public DateTime? ChangedPlan { get; set; }

        [DisplayName("Observações")]
        [DataType(DataType.MultilineText)]
        public string Observations { get; set; }
        [DisplayName("Pagamentos")]
        public List<PaymentModel> Payments { get; set; }

        [DisplayName("Plano de frequencia")]
        public long PlanId { get; set; }
        [ForeignKey("PlanId")]
        public PlanModel Plan { get; set; }

        [DisplayName("Endereço")]
        public string Address { get; set; }
        [DisplayName("Bairro")]
        public string Neighborhood { get; set; }
        [DisplayName("Cidade")]
        public string City { get; set; }
        public string CEP { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        [DisplayName("Estado civil")]
        public MaritalStatus MaritalStatus { get; set; }
        [DisplayName("Ocupação")]
        public string Occupation { get; set; }
        
        [Phone]
        [DisplayName("Telefone celular")]
        [DataType(DataType.PhoneNumber)]
        public string CelPhone { get; set; }
        
        /// <summary>
        /// Weekly classes
        /// </summary>
        public virtual ICollection<ClassModel> Classes { get; set; }

        /// <summary>
        /// Replacements and trial classes currently (DONT USE FOR PRESENCE)
        /// </summary>
        public virtual ICollection<SpareClassModel> ReplacamentClasses { get; set; }

        /// <summary>
        /// Weekly classes that the student participated
        /// </summary>
        public virtual ICollection<PresenceModel> WeeklyPresence { get; set; }

        /// <summary>
        /// Trial classes that the student participated
        /// </summary>
        public virtual ICollection<PresenceModel> TrialPresence { get; set; }
        /// <summary>
        /// Replacement classes that the student participated
        /// </summary>
        public virtual ICollection<PresenceModel> ReplacementPresence { get; set; }

        /// <summary>
        /// Weekly classes that the student should have participated
        /// </summary>
        public virtual ICollection<PresenceModel> EnroledWeeklyClasses { get; set; }

        /// <summary>
        /// Trial classes that the student should have participated
        /// </summary>
        public virtual ICollection<PresenceModel> EnroledTrialClasses { get; set; }

        /// <summary>
        /// Replacement classes that the student should have participated
        /// </summary>
        public virtual ICollection<PresenceModel> EnroledReplacementClasses { get; set; }
        
        public long AmountMissedClasses()
        {
            return this.AmountMissedNotTrialClasses() + this.AmountMissedTrialClasses();
        }

        public long AmountMissedNotTrialClasses()
        {
            return AmountMissedWeeklyClasses()+AmountMissedReplacementClasses();
        }

        public string Test()
        {
            int replacementToHave = this.ReplacamentClasses.Where(c => c.IsTrial == false && c.NewDate >= DateTime.Now.AddMonths(-2)).ToList().Count;
            int replacementsDone = this.EnroledReplacementClasses.Where(c => c.Date >= DateTime.Now.AddMonths(-2)).ToList().Count;
            
            int missedClasses = this.MissedWeeklyClasses().Count;
            int replacedMissedClasses = (int)this.ReplacementClassesPresent();

            return (replacementToHave - replacementsDone)+"/"+(missedClasses-replacedMissedClasses);
        }

        public long AmountMissedTrialClasses()
        {
            /*
            if (this.EnroledTrialClasses == null || this.TrialPresence == null)
                return 0;
            return this.EnroledTrialClasses.Count - this.TrialPresence.Count;
            */
            return MissedTrialClasses().Count;
        }

        public long AmountMissedWeeklyClasses()
        {
            /*
            if (this.WeeklyPresence == null || this.EnroledWeeklyClasses == null)
                return 0;
            return this.EnroledWeeklyClasses.Count - this.WeeklyPresence.Count;
            */
            return this.MissedWeeklyClasses().Count - this.MissedWeeklyClassesReplaced().Count;
        }

        public long AmountMissedReplacementClasses()
        {
            /*
            if (this.EnroledReplacementClasses == null || this.ReplacementPresence == null)
                return 0;
            return this.EnroledReplacementClasses.Count - this.ReplacementPresence.Count;
            */
            return this.MissedReplacementClasses().Count - this.MissedReplacementClassesReplaced().Count;
        }

        public List<PresenceModel> AllMissedWeeklyClasses()
        {
            List<PresenceModel> lpm = new List<PresenceModel>();

            if (this.ReplacamentClasses == null)
                this.ReplacamentClasses = new List<SpareClassModel>();

            if (this.WeeklyPresence == null || this.EnroledWeeklyClasses == null)
                return lpm;

            foreach (PresenceModel pm in this.EnroledWeeklyClasses.OrderBy(c => c.Date))
            {
                if (!this.WeeklyPresence.Contains(pm))
                {
                    lpm.Add(pm);
                }
            }
            return lpm;
        }

        public List<PresenceModel> MissedWeeklyClasses()
        {
            List<PresenceModel> lpm = new List<PresenceModel>();

            if (this.ReplacamentClasses == null)
                this.ReplacamentClasses = new List<SpareClassModel>();

            if (this.WeeklyPresence == null || this.EnroledWeeklyClasses == null)
                return lpm;

            foreach (PresenceModel pm in this.EnroledWeeklyClasses.OrderBy(c=>c.Date).Where(c=>c.Date >= DateTime.Now.AddMonths(-2)))
            {
                if (!this.WeeklyPresence.Contains(pm))
                {
                    lpm.Add(pm);
                }
            }
            return lpm;
        }

        public List<PresenceModel> MissedWeeklyClassesReplaced()
        {
            List<PresenceModel> lpm = new List<PresenceModel>();

            if (this.ReplacamentClasses == null)
                this.ReplacamentClasses = new List<SpareClassModel>();

            if (this.WeeklyPresence == null || this.EnroledWeeklyClasses == null)
                return lpm;

            foreach (PresenceModel pm in this.EnroledWeeklyClasses.OrderBy(c => c.Date).Where(c => c.Date >= DateTime.Now.AddMonths(-2)))
            {
                if (!this.WeeklyPresence.Contains(pm))
                {
                    foreach (SpareClassModel spareClass in this.ReplacamentClasses)
                    {
                        if (spareClass.OldClassId == pm.ClassId && spareClass.OldDate == pm.Date)
                        {
                            lpm.Add(pm);
                            break;
                        }
                    }
                }
            }
            return lpm;
        }

        public List<PresenceModel> MissedReplacementClasses()
        {
            List<PresenceModel> lpm = new List<PresenceModel>();

            if (this.ReplacamentClasses == null)
                this.ReplacamentClasses = new List<SpareClassModel>();

            if (this.ReplacementPresence == null || this.EnroledReplacementClasses == null)
                return lpm;

            
            foreach (PresenceModel pm in this.EnroledReplacementClasses.OrderBy(c => c.Date).Where(c => c.Date >= DateTime.Now.AddMonths(-2)))
            {
                if (!this.ReplacementPresence.Contains(pm))
                {
                    
                        lpm.Add(pm);
                }
            }
            return lpm;
        }

        public List<PresenceModel> MissedReplacementClassesReplaced()
        {
            List<PresenceModel> lpm = new List<PresenceModel>();

            if (this.ReplacamentClasses == null)
                this.ReplacamentClasses = new List<SpareClassModel>();

            if (this.ReplacementPresence == null || this.EnroledReplacementClasses == null)
                return lpm;


            foreach (PresenceModel pm in this.EnroledReplacementClasses.OrderBy(c => c.Date))
            {
                if (!this.ReplacementPresence.Contains(pm))
                {
                    foreach (SpareClassModel spareClass in this.ReplacamentClasses)
                    {
                        if (spareClass.OldClassId == pm.ClassId && spareClass.OldDate == pm.Date)
                        {
                           lpm.Add(pm);
                            break;
                        }
                    }
                }
            }
            return lpm;
        }

        public List<SpareClassModel> SpareClassesWithoutPresenceList()
        {
            KaiutYogaContext db = new KaiutYogaContext();
            List<PresenceModel> lpm = db.PresenceModels.ToList();
            List<SpareClassModel> replacementclasses = new List<SpareClassModel>();

            foreach (SpareClassModel scm in this.ReplacamentClasses)
            {
                if (scm.PresenceList() == null)
                    replacementclasses.Add(scm);
            }

            return replacementclasses;
        }

        public List<PresenceModel> MissedTrialClasses()
        {
            List<PresenceModel> lpm = new List<PresenceModel>();

            if (this.ReplacamentClasses == null)
                this.ReplacamentClasses = new List<SpareClassModel>();

            if (this.TrialPresence == null || this.EnroledTrialClasses == null)
                return lpm;

            foreach (PresenceModel pm in this.EnroledTrialClasses.OrderBy(c => c.Date).Where(c => c.Date >= DateTime.Now.AddMonths(-2)))
            {
                if (!this.TrialPresence.Contains(pm))
                {
                    bool replacedClass = false;
                    foreach (SpareClassModel spareClass in this.ReplacamentClasses)
                    {
                        if (spareClass.OldClassId == pm.ClassId && spareClass.OldDate == pm.Date)
                        {
                            replacedClass = true;
                            break;
                        }
                    }
                    if (!replacedClass)
                        lpm.Add(pm);
                }
            }
            return lpm;
        }

        public List<PresenceModel> MissedTrialClassesReplaced()
        {
            List<PresenceModel> lpm = new List<PresenceModel>();

            if (this.ReplacamentClasses == null)
                this.ReplacamentClasses = new List<SpareClassModel>();

            if (this.ReplacementPresence == null || this.EnroledTrialClasses == null)
                return lpm;


            foreach (PresenceModel pm in this.EnroledTrialClasses.OrderBy(c => c.Date))
            {
                if (!this.TrialPresence.Contains(pm))
                {
                    foreach (SpareClassModel spareClass in this.ReplacamentClasses)
                    {
                        if (spareClass.OldClassId == pm.ClassId && spareClass.OldDate == pm.Date)
                        {
                            lpm.Add(pm);
                            break;
                        }
                    }
                }
            }
            return lpm;
        }

        public List<PresenceModel> MissedSpareClassNotReplaced()
        {
            List<PresenceModel> lpm = new List<PresenceModel>();

            if (this.ReplacamentClasses == null)
                this.ReplacamentClasses = new List<SpareClassModel>();

            if (this.ReplacementPresence == null || this.EnroledReplacementClasses == null)
                return lpm;


            foreach (PresenceModel pm in this.EnroledReplacementClasses.OrderBy(c => c.Date))
            {
                if (!this.ReplacementPresence.Contains(pm))
                {
                    bool replacedClass = false;
                    foreach (SpareClassModel spareClass in this.ReplacamentClasses)
                    {
                        if (spareClass.OldClassId == pm.ClassId && spareClass.OldDate == pm.Date)
                        {
                            replacedClass = true;
                            break;
                        }
                    }
                    if (!replacedClass)
                        lpm.Add(pm);
                }
            }
            return lpm;
        }

        public long ReplacementClassesPresent()
        {
            long presentInClasses = 0;
            if (this.ReplacementPresence != null)
            {
                foreach (PresenceModel pm in this.ReplacementPresence.Where(c=>c.Date >= DateTime.Now.AddMonths(-2)))
                {
                    if (pm.AllPresentStudents().Find(c => c.Id == this.Id) != null)
                        presentInClasses++;
                }
            }
            return presentInClasses;
        }

        public long TrialClassesPresent()
        {
            long presentInClasses = 0;
            if (this.TrialPresence != null)
            {
                foreach (PresenceModel pm in this.TrialPresence)
                {
                    if (pm.AllPresentStudents().Find(c => c.Id == this.Id) != null)
                        presentInClasses++;
                }
            }
            return presentInClasses;
        }

        public List<PresenceModel> ClassesToReplace()
        {
            PresenceModel pm = new PresenceModel();
            List<PresenceModel> allmissedclasses = new List<PresenceModel>();
            allmissedclasses = this.MissedWeeklyClasses();
            allmissedclasses = allmissedclasses.Concat(this.MissedTrialClasses()).ToList();
            allmissedclasses = allmissedclasses.Concat(this.MissedReplacementClasses()).ToList();

            foreach (SpareClassModel scm in this.ReplacamentClasses)
            {
                pm = allmissedclasses.Find(c => c.ClassId == scm.OldClassId && c.Date == scm.OldDate);
                // if found any wih the OldId, then it was replaced already
                if ( pm != null)
                {
                    allmissedclasses.Remove(pm);
                }
            }

            return allmissedclasses;
        }

        public List<PresenceModel> AllReplacedClasses()
        {
            PresenceModel pm = new PresenceModel();
            List<PresenceModel> allreplacedclasses = new List<PresenceModel>();
            allreplacedclasses = this.MissedWeeklyClassesReplaced();
            allreplacedclasses = allreplacedclasses.Concat(this.MissedTrialClasses()).ToList();
            allreplacedclasses = allreplacedclasses.Concat(this.MissedReplacementClassesReplaced()).ToList();

            foreach (SpareClassModel scm in this.ReplacamentClasses)
            {
                pm = allreplacedclasses.Find(c => c.ClassId == scm.OldClassId && c.Date == scm.OldDate);
                // if found any wih the OldId, then it was replaced already
                if (pm != null)
                {
                    allreplacedclasses.Remove(pm);
                }
            }

            return allreplacedclasses;
        }

        public long TotalClassesToReplace()
        {
            long missedweekly = this.MissedWeeklyClasses().Count;
            long replacedweekly = this.MissedWeeklyClassesReplaced().Count;

            long missedreplacement = this.MissedReplacementClasses().Count;
            long replacedreplacement = this.MissedReplacementClassesReplaced().Count;

            long spareclassnotreplaced = this.MissedSpareClassNotReplaced().Count;

            return missedweekly - replacedweekly + missedreplacement - replacedreplacement;
        }

        public bool PresentInClass(PresenceModel presence)
        {
            foreach (PresenceModel pm in WeeklyPresence)
            {
                if (pm.Id == presence.Id)
                    return true;
            }
            foreach (PresenceModel pm in TrialPresence)
            {
                if (pm.Id == presence.Id)
                    return true;
            }
            foreach (PresenceModel pm in ReplacementPresence )
            {
                if (pm.Id == presence.Id)
                    return true;
            }
            return false;
        }

        public List<PaymentModel> CreateOrGetPayment(DateTime date)
        {
            KaiutYogaContext db = new KaiutYogaContext();
            PaymentModel payment;
            List<PaymentModel> lpm = new List<PaymentModel>();
            PlanModel planmodel = db.PlanModels.Find(this.PlanId);
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            DateTime start = new DateTime(date.Year, date.Month, 1);
            DateTime end = new DateTime(date.Year, date.Month, daysInMonth);

            if (this.Payments != null)
            {
                foreach (PaymentModel pm in this.Payments)
                {
                    if (pm.Month == start.Month && pm.Year == start.Year) {
                        lpm.Add(pm);
                    }
                }
                if (lpm.Count > 0)
                    return lpm;
            }
            if (!planmodel.IsTrial)
            {
                payment = new PaymentModel(this, planmodel, date);
                //payment.PeriodStart = date;
                lpm.Add(payment);
            }
            return lpm;
        }

        public string ConvertRegisterDateToString()
        {
            if (this.RegisterDate != null)
                return TimeZoneInfo.ConvertTimeFromUtc(this.RegisterDate, TimeZoneInfo.Local).ToString("yyyy-MM-ddT00:00:00");
            return "";
        }
    }
}