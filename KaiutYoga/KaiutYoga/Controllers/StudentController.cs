using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaiutYoga.Models;
using KaiutYoga.DAL;
using PagedList;
using KaiutYoga.ViewModels;
using Newtonsoft.Json;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace KaiutYoga.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private KaiutYogaContext db = new KaiutYogaContext();

        //
        // GET: /Client/

        public ActionResult Index(string sortOrder, string currentFilter, string searchName, int? page, bool studentStatus = true)
        {
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "";
            ViewBag.LastNameSortParm = sortOrder == "lastname" ? "lastname_desc" : "lastname";
            ViewBag.PlanSortParm = sortOrder == "plan" ? "plan_desc" : "plan";
            ViewBag.studentStatus = studentStatus;

            if (searchName != null)
            {
                page = 1;
            }
            else
            {
                searchName = currentFilter;
            }

            ViewBag.searchName = searchName;
            ViewBag.CurrentFilter = searchName;

            var studentmodel = db.StudentModels.Include(c => c.Classes).Include(c => c.Plan).Where(c=>c.Active == studentStatus).ToList();
            if (!String.IsNullOrEmpty(searchName))
            {
                studentmodel = studentmodel.Where(c => c.Name.ToUpper().Contains(searchName.ToUpper())).ToList();
            }

            switch (sortOrder)
            {
                case "lastname":
                    studentmodel = studentmodel.OrderBy(c => c.LastName).ToList();
                    break;
                case "lastname_desc":
                    studentmodel = studentmodel.OrderByDescending(c => c.LastName).ToList();
                    break;
                case "plan":
                    studentmodel = studentmodel.OrderBy(c => c.Plan.Name).ToList();
                    break;
                case "plan_desc":
                    studentmodel = studentmodel.OrderByDescending(c => c.Plan.Name).ToList();
                    break;
                case "firstname_desc":
                    studentmodel = studentmodel.OrderByDescending(c => c.FirstName).ToList();
                    break;
                default:
                    studentmodel = studentmodel.OrderBy(c => c.FirstName).ToList();
                    break;

            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(studentmodel.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Client/Details/5

        public ActionResult Details(long id = 0)
        {
            StudentModel StudentModel = db.StudentModels.Find(id);
            if (StudentModel == null)
            {
                return HttpNotFound();
            }            
            return View(StudentModel);
        }

        //
        // GET: /Client/Create

        public ActionResult Create()
        {
            ViewBag.PlanId = new SelectList(db.PlanModels, "Id", "Name");
            return View();
        }

        //
        // POST: /Client/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentModel StudentModel)
        {
            if (ModelState.IsValid)
            {
                ClassRepository cr = new ClassRepository(db);
                StudentModel.Classes = new List<ClassModel>();
                //StudentModel.Register = DateTime.Now;
                if (TempData["SelectedClasses"] != null)
                {
                    foreach (long classId in TempData["SelectedClasses"] as List<long>)
                    {
                        StudentModel.Classes.Add(cr.GetClass(classId));
                    }
                }
                PlanModel pm = db.PlanModels.Find(StudentModel.PlanId);
                StudentModel.ReplacamentClasses = new List<SpareClassModel>();

                if (TempData["SpareClasses"] != null){
                    foreach (SpareClassModel scm in TempData["SpareClasses"] as List<SpareClassModel>)
                    {
                        scm.IsTrial = pm.IsTrial;
                        StudentModel.ReplacamentClasses.Add(scm);
                    }
                }
                //StudentModel.ChangedPlan = DateTime.Now;
                StudentModel.RegisterDate = DateTime.Now;
                StudentModel.Active = true;
                //StudentModel.BirthDate = DateTime.Now;
                //StudentModel.Payments = new List<PaymentModel>();
                
                db.StudentModels.Add(StudentModel);
                db.SaveChanges();

                if (!pm.IsTrial)
                {
                    StudentModel.Payments = new List<PaymentModel>();
                    StudentModel.Payments.Add(new PaymentModel(StudentModel.Id, pm.Id, DateTime.Now,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.DaysInMonth(DateTime.Now.Year,DateTime.Now.Month))));
                }

                db.Entry(StudentModel).State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("Index");
            }

            ViewBag.PlanId = new SelectList(db.PlanModels, "Id", "Name", StudentModel.PlanId);
            return View(StudentModel);
        }

        public ActionResult GetClassCalendar(int planId = 0, int studentId = 0)
        {
            TempData.Remove("SelectedClasses");
            TempData.Remove("SpareClasses");
            TempData.Remove("RemovedSpareClasses");

            StudentModel sm = new StudentModel();
            PlanModel pm = db.PlanModels.Where(i => i.Id == planId).Single();
            sm.PlanId = planId;
            sm.Plan = pm;
            if (studentId != 0)
            {
                List<PresenceModel> lpm = db.PresenceModels.ToList();
                sm = db.StudentModels.Include(c=>c.WeeklyPresence).Where(c => c.Id == studentId).SingleOrDefault();
                sm.PlanId = planId;
                sm.Plan = pm;

                List<long> classes = new List<long>();
                
                if (sm.Classes != null)
                {
                    foreach (ClassModel cm in sm.Classes)
                    {
                        classes.Add(cm.Id);
                    }
                    TempData["SelectedClasses"] = classes;
                }
                List<SpareClassModel> replacementclasses = new List<SpareClassModel>();
                if (sm.ReplacamentClasses != null)
                {
                    foreach (SpareClassModel scm in sm.ReplacamentClasses.Where(c=>c.NewDate >= DateTime.Now.AddMonths(-2)))
                    {
                        //if (lpm.Find(c=>c.ClassId == scm.NewClassId && c.Date == scm.NewDate) == null)
                            replacementclasses.Add(scm);
                    }
                    TempData["SpareClasses"] = replacementclasses;
                }
            }

            return PartialView("_StudentCalendar", sm);
        }

        [HttpPost]
        public string AddClass(long studentId = 0, long classId = 0, long planId = 0, long amountspare = 0, string date = "", bool onlySpare = false)
        {
            StudentModel student = new StudentModel();
            if (studentId != 0)
            {
                student = db.StudentModels.Include(c => c.EnroledWeeklyClasses)
                    .Include(c => c.EnroledReplacementClasses)
                    .Include(c => c.EnroledTrialClasses)
                    .Include(c => c.WeeklyPresence)
                    .Include(c => c.ReplacementPresence)
                    .Include(c => c.TrialPresence)
                    .Include(c => c.ReplacamentClasses)
                    .Where(c => c.Id == studentId).Single();
            }
            PlanModel pm = db.PlanModels.Find(planId);
            long frequency = pm.Frequency;
            if (pm.IsTrial)
            {
                if (studentId == 0)
                    amountspare += pm.Frequency;
                frequency = 0;
            }

            DateTime classDate = new DateTime(1970, 1, 1, 0, 0, 0, 0); //Set default date 1/1/1970
            classDate = classDate.AddMilliseconds(Convert.ToDouble(date));

            List<long> classes = new List<long>();
            List<SpareClassModel> spareclasses = new List<SpareClassModel>();
            List<SpareClassModel> removedspareclasses = new List<SpareClassModel>();

            AddClassStudent acs = new AddClassStudent();
            acs.HavePresenceList = db.ClassModels.Find(classId).HavePresenceList(date);
            
            if (TempData["SelectedClasses"] != null)
            {
                classes = TempData["SelectedClasses"] as List<long>;
                TempData.Keep("SelectedClasses");
            }

            long amountclasseswithpresencelist = 0;
            if (TempData["SpareClasses"] != null)
            {
                spareclasses = TempData["SpareClasses"] as List<SpareClassModel>;
                TempData.Keep("SpareClasses");
                if (spareclasses == null)
                    spareclasses = new List<SpareClassModel>();

                foreach (SpareClassModel scm in spareclasses)
                {
                    if (scm.PresenceList() != null)
                        amountclasseswithpresencelist++;
                }
            }

            if (TempData["RemovedSpareClasses"] != null)
            {
                removedspareclasses = TempData["RemovedSpareClasses"] as List<SpareClassModel>;
                TempData.Keep("RemovedSpareClasses");
                if (removedspareclasses == null)
                    removedspareclasses = new List<SpareClassModel>();
            }

            if (!onlySpare && !classes.Contains(classId) && classes.Count < frequency && (!acs.HavePresenceList || !pm.IsTrial))
            {
                acs.WeeklyAdded = true;
                classes.Add(classId);
            }
            TempData["SelectedClasses"] = classes;
            
            if (classes.Count >= frequency || onlySpare)
            {
                
                // Add spare classes
                if (!acs.WeeklyAdded && !acs.HavePresenceList)
                {
                    TempData["SpareClasses"] = spareclasses;
                    // se for menor e se nao existir chamada para aquela aula
                    
                    if (spareclasses.Count >= amountspare+amountclasseswithpresencelist && !pm.IsTrial)
                    {
                        acs.SpareAdded = false;
                    }
                    else if (spareclasses.Find(c=>c.NewClassId == classId && c.NewDate == classDate) == null && (spareclasses.Count < amountspare+amountclasseswithpresencelist || (pm.IsTrial && (spareclasses.Count < pm.Frequency + amountspare))))
                    {

                        acs.SpareAdded = true;
                        SpareClassModel scm = new SpareClassModel();
                        if (!pm.IsTrial)
                        {
                            foreach (PresenceModel presence in student.MissedWeeklyClasses())
                            {
                                // if the class was not replaced
                                if (student.MissedWeeklyClassesReplaced().Find(c => c.ClassId == presence.ClassId && c.Date == presence.Date) == null &&
                                    spareclasses.Find(c => c.OldClassId == presence.ClassId && c.OldDate == presence.Date) == null)
                                {
                                    scm.OldClassId = presence.ClassId;
                                    scm.OldDate = presence.Date;
                                    break;
                                }
                            }
                            if (scm.OldClassId == 0)
                            {
                                foreach (PresenceModel presence in student.MissedReplacementClasses())
                                {
                                    if (student.MissedReplacementClassesReplaced().Find(c => c.ClassId == presence.ClassId && c.Date == presence.Date) == null &&
                                        spareclasses.Find(c => c.OldClassId == presence.ClassId && c.OldDate == presence.Date) == null)
                                    {
                                        scm.OldClassId = presence.ClassId;
                                        scm.OldDate = presence.Date;
                                        break;
                                    }
                                }
                            }
                            if (scm.OldClassId == 0)
                            {
                                foreach (PresenceModel presence in student.MissedTrialClasses())
                                {
                                    if (student.MissedTrialClassesReplaced().Find(c => c.ClassId == presence.ClassId && c.Date == presence.Date) == null &&
                                        spareclasses.Find(c => c.OldClassId == presence.ClassId && c.OldDate == presence.Date) == null)
                                    {
                                        scm.OldClassId = presence.ClassId;
                                        scm.OldDate = presence.Date;
                                        break;
                                    }
                                }
                            }
                            if (scm.OldClassId == 0)
                            {
                                SpareClassModel classToRemove = new SpareClassModel();

                                if (removedspareclasses.Count > 0)
                                {
                                    classToRemove = removedspareclasses.OrderBy(c => c.NewDate).First();
                                    scm.OldClassId = classToRemove.NewClassId;
                                    scm.OldDate = classToRemove.NewDate;
                                    removedspareclasses.Remove(classToRemove);
                                }
                                TempData["RemovedSpareClasses"] = spareclasses;
                            }
                        }
                        

                        scm.NewClassId = classId;
                        scm.NewDate = classDate;
                        spareclasses.Add(scm);
                        TempData["SpareClasses"] = spareclasses;
                    }
                }
                else
                {
                    if (!acs.WeeklyAdded && acs.HavePresenceList)
                    {
                        //return havePresenceList.ToString();\
                        return JsonConvert.SerializeObject(acs, Formatting.Indented);
                    }
                    
                }
            }


            if (pm.IsTrial)
            {
                if (studentId == 0)
                    amountspare -= pm.Frequency;

                long diff = spareclasses.Count - pm.Frequency;
                // this means there is a trial class to replace, so the amount of spare classes is greater than the frequency
                if (spareclasses.Count >= pm.Frequency)
                {   
                    acs.WeeklyMsg = (spareclasses.Count -diff) + "/" + pm.Frequency;
                    acs.SpareMsg = diff + "/" + amountspare;
                }
                else
                {
                    acs.WeeklyMsg = spareclasses.Count + "/" + pm.Frequency;
                    acs.SpareMsg = "0/"+amountspare;
                }
            }
            else
            {
                acs.WeeklyMsg = classes.Count + "/" + frequency;
                acs.SpareMsg = spareclasses.Count-amountclasseswithpresencelist + "/" + amountspare;
            }
            TempData["SelectedClasses"] = classes;
            TempData["SpareClasses"] = spareclasses;

            return JsonConvert.SerializeObject(acs, Formatting.Indented);
        }

        [HttpPost]
        public string RemoveClass(long studentId = 0,long classId = 0,long planId = 0, long amountspare = 0, string date="")
        {
            PlanModel pm = db.PlanModels.Find(planId);
            long frequency = pm.Frequency;
            if (pm.IsTrial)
            {
                amountspare += pm.Frequency;
                frequency = 0;
            }

            List<long> classes = new List<long>();
            List<SpareClassModel> spareclasses = new List<SpareClassModel>();
            List<SpareClassModel> removedspareclasses = new List<SpareClassModel>();
            RemoveClassStudent rcs = new RemoveClassStudent();
            rcs.Removed = false;
            classes = TempData["SelectedClasses"] as List<long>;
            spareclasses = TempData["SpareClasses"] as List<SpareClassModel>;
            removedspareclasses = TempData["RemovedSpareClasses"] as List<SpareClassModel>;
            TempData.Keep("SelectedClasses");
            TempData.Keep("SpareClasses");
            if (TempData["SelectedClasses"] != null)
            {
                if (classes.Contains(classId))
                {
                    rcs.Removed = true;
                    classes.Remove(classId);
                    TempData["SelectedClasses"] = classes;
                }
            }

            DateTime classDate = new DateTime(1970, 1, 1, 0, 0, 0, 0); //Set default date 1/1/1970
            classDate = classDate.AddMilliseconds(Convert.ToDouble(date));

            if (!rcs.Removed && TempData["SpareClasses"] != null)
            {
                List<PresenceModel> lpm = db.PresenceModels.Where(c => c.ClassId == classId && c.Date == classDate).ToList();

                if (lpm.Count == 0)
                {
                    rcs.Removed = true;
                    TempData.Keep("SpareClasses");
                    if (spareclasses.Find(c => c.NewClassId == classId && c.NewDate == classDate) != null)
                    {
                        SpareClassModel classToRemove = spareclasses.Find(c => c.NewClassId == classId && c.NewDate == classDate);
                        if (removedspareclasses == null)
                            removedspareclasses = new List<SpareClassModel>();
                        removedspareclasses.Add(classToRemove);
                        spareclasses.Remove(classToRemove);
                    }
                }
                TempData["RemovedSpareClasses"] = removedspareclasses;
                TempData["SpareClasses"] = spareclasses;
            }
            
            if (pm.IsTrial)
            {
                amountspare -= pm.Frequency;
                long diff = spareclasses.Count - pm.Frequency;
                if (spareclasses.Count >= pm.Frequency)
                {
                    rcs.WeeklyMsg = (spareclasses.Count - diff) + "/" + pm.Frequency;
                    rcs.SpareMsg = diff + "/" + amountspare;
                }
                else
                {
                    rcs.WeeklyMsg = spareclasses.Count + "/" + pm.Frequency;
                    rcs.SpareMsg = "0/" + amountspare;
                }
            }
            else
            {
                rcs.WeeklyMsg = classes.Count + "/" + frequency;

                StudentModel student = db.StudentModels.Include(c => c.EnroledWeeklyClasses)
                .Include(c => c.EnroledReplacementClasses)
                //.Include(c => c.EnroledTrialClasses)
                //.Include(c => c.WeeklyPresence)
                //.Include(c => c.ReplacementPresence)
                //.Include(c => c.TrialPresence)
                //.Include(c => c.ReplacamentClasses)
                .Where(c => c.Id == studentId).Single();

                rcs.SpareMsg = (spareclasses.Count - student.EnroledReplacementClasses.Where(c => c.Date >= DateTime.Now.AddMonths(-2)).Count())+"/" + amountspare;
            }

            
            

            return JsonConvert.SerializeObject(rcs, Formatting.Indented);
            //return removed.ToString() + "|" + amountnormalmsg + "|" + amountsparemsg;
        }


        //
        // GET: /Client/Edit/5

        public ActionResult Edit(long id = 0)
        {
            StudentModel StudentModel = db.StudentModels.Include(c=>c.Plan).Include(c=>c.ReplacamentClasses).Include(c=>c.WeeklyPresence).Include(c=>c.Payments).Where(c=>c.Id == id).Single();
            if (StudentModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlanId = new SelectList(db.PlanModels, "Id", "Name", StudentModel.PlanId);
            ClassRepository cr = new ClassRepository(db);
            if (!StudentModel.Plan.IsTrial && StudentModel.Active)
            {
                List<PaymentModel> payments = StudentModel.CreateOrGetPayment(DateTime.Now);
                // need to insert into database
                foreach (PaymentModel pm in payments)
                {
                    if (pm.Id == 0)
                    {
                        db.PaymentModels.Add(pm);
                        db.SaveChanges();
                    }
                }
            }
            return View(StudentModel);
        }

        //
        // POST: /Client/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentModel StudentModel)
        {
            if (ModelState.IsValid)
            {
                StudentModel studentToUpdate = db.StudentModels.Include(c=>c.Payments).Where(c=>c.Id == StudentModel.Id).Single();// Include(i => i.Classes).Include(i => i.Plan).Where(i => i.Id == StudentModel.Id).Single();

                if (studentToUpdate.PlanId != StudentModel.PlanId)
                {
                    studentToUpdate.ChangedPlan = DateTime.Now;
                    DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    List<PaymentModel> paymentFromSameMonth = studentToUpdate.CreateOrGetPayment(DateTime.Now);
                    PaymentModel lastPayment = new PaymentModel();
                    // get payment with the higher PeriodStart
                    foreach (PaymentModel pm in paymentFromSameMonth)
                    {
                        if (pm.PeriodStart <= DateTime.Now && pm.PeriodEnd>= DateTime.Now && pm.PlanId != StudentModel.PlanId)
                        {
                            lastPayment = pm;
                        }
                    }

                    // found a payment for the same month and diferent plan
                    if (lastPayment.Id != 0)
                    {
                        //lastPayment.PeriodEnd = DateTime.Now.AddDays(-1);
                        studentToUpdate.Payments.Add(new PaymentModel(ref lastPayment, studentToUpdate, StudentModel.PlanId, DateTime.Now));
                    }

                    studentToUpdate.Payments = studentToUpdate.CreateOrGetPayment(DateTime.Now);
                }

                if (TryUpdateModel(studentToUpdate, "", new string[] { "FirstName", "LastName", "Active", "Gender", "BirthDate","PlanId",
                                                                        "Address","Neighborhood","City","CEP","RG","CPF","MaritalStatus",
                                                                        "Occupation","Phone","CelPhone","Email","Observations"}))
                {
                    List<long> classesId = TempData["SelectedClasses"] as List<long>;
                    if (studentToUpdate.Classes == null)
                        studentToUpdate.Classes = new List<ClassModel>();

                    var selectedClassesHS = new HashSet<long>(classesId);
                    var studentClasses = new HashSet<long>(studentToUpdate.Classes.Select(i => i.Id));

                    PlanModel pm = db.PlanModels.Find(studentToUpdate.PlanId);
                    if (!pm.IsTrial)
                    {
                        foreach (ClassModel cm in db.ClassModels)
                        {
                            if (selectedClassesHS.Contains(cm.Id))
                            {
                                if (!studentClasses.Contains(cm.Id))
                                {
                                    studentToUpdate.Classes.Add(cm);
                                }
                            }
                            else
                            {
                                if (studentClasses.Contains(cm.Id))
                                {
                                    studentToUpdate.Classes.Remove(cm);
                                }
                            }
                        }
                    }
                    // Trial plan cannot have weeklyclasses
                    else
                    {
                        studentToUpdate.Classes.Clear();
                    }


                    List<SpareClassModel> spareclasses = TempData["SpareClasses"] as List<SpareClassModel>;
                    List<long> longspareclasses = new List<long>();
                    if (studentToUpdate.ReplacamentClasses== null)
                        studentToUpdate.ReplacamentClasses = new List<SpareClassModel>();

                    bool added = false;
                    foreach (SpareClassModel scm in spareclasses)
                    {
                        scm.IsTrial = pm.IsTrial;
                        if (scm.Id == 0)
                        {
                            db.SpareClassModels.Add(scm);
                            added = true;
                        }
                    }
                    if (added)
                        db.SaveChanges();

                    foreach (SpareClassModel scm in spareclasses)
                    {
                        longspareclasses.Add(scm.Id);
                    }

                    var selectedSpareClassesHS = new HashSet<long>(longspareclasses);
                    var studentSpareClasses = new HashSet<long>(studentToUpdate.ReplacamentClasses.Select(i => i.Id));
                    //studentToUpdate.SpareClasses.Clear();
                    foreach (SpareClassModel scm in db.SpareClassModels.Include(c=>c.NewClass))
                    {
                        if (selectedSpareClassesHS.Contains(scm.Id))
                        {
                            if (!studentSpareClasses.Contains(scm.Id))
                            {
                                studentToUpdate.ReplacamentClasses.Add(scm);
                            }
                        }
                        else
                        {
                            if (studentSpareClasses.Contains(scm.Id))
                            {
                                studentToUpdate.ReplacamentClasses.Remove(scm);
                                db.SpareClassModels.Remove(scm);
                            }
                        }
                    }

                    db.Entry(studentToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.PlanId = new SelectList(db.PlanModels, "Id", "Name", StudentModel.PlanId);
            return View(StudentModel);
        }

        //
        // GET: /Client/Delete/5

        public ActionResult Delete(long id = 0)
        {
            StudentModel StudentModel = db.StudentModels.Include(c=>c.Plan).Where(c=>c.Id == id).SingleOrDefault();
            if (StudentModel == null)
            {
                return HttpNotFound();
            }
            return View(StudentModel);
        }

        //
        // POST: /Client/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            StudentModel StudentModel = db.StudentModels.Include(i => i.Classes).Include(i=>i.Payments).Include(i=>i.ReplacamentClasses).Where(i => i.Id == id).SingleOrDefault();
            StudentModel.Classes = null;
            if (StudentModel.Payments.Count > 0)
            {
                List<PaymentModel> lpm = new List<PaymentModel>(StudentModel.Payments);
                foreach (PaymentModel pm in lpm)
                {
                    db.PaymentModels.Remove(pm);
                }
            }
            if (StudentModel.ReplacamentClasses.Count > 0)
            {
                List<SpareClassModel> lscm = new List<SpareClassModel>(StudentModel.ReplacamentClasses);
                foreach (SpareClassModel scm in lscm)
                {
                    db.SpareClassModels.Remove(scm);
                }
            }

            db.StudentModels.Remove(StudentModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Report()
        {
            List<StudentModel> students = db.StudentModels.Include(c => c.Classes).Include(c => c.EnroledReplacementClasses).Include(c => c.EnroledTrialClasses).Include(c => c.EnroledWeeklyClasses).Include(c => c.Plan).ToList();
            //this.CreateReport(students);
            foreach (var sm in students)
            {
                foreach (var cm in sm.Classes)
                {
                    cm.Professor = db.ProfessorModels.Where(c => c.Id == cm.Id).SingleOrDefault();
                }
            }
            return View("Report", students);
        }

        public void CreateReport(List<StudentModel> students)
        {
            var studentsGrid = new System.Data.DataTable("teste");
            studentsGrid.Columns.Add("Aluno", typeof(string));
            studentsGrid.Columns.Add("Idade", typeof(int));
            studentsGrid.Columns.Add("Plano", typeof(string));
            studentsGrid.Columns.Add("Aulas experimentais", typeof(int));
            studentsGrid.Columns.Add("Ativo", typeof(string));
            studentsGrid.Columns.Add("Data de cadastro", typeof(DateTime));
            studentsGrid.Columns.Add("Data de matricula", typeof(DateTime));
            studentsGrid.Columns.Add("Aluno inativo (data da ultima aula)", typeof(DateTime));

            DataRow row;
            
            foreach (StudentModel item in students)
            {
                row = studentsGrid.NewRow();
                row["Aluno"] = item.Name;
                row["Idade"] = item.Age;
                row["Plano"] = item.Plan.Name;
                row["Aulas experimentais"] = item.TrialClassesPresent();
                if (item.Active)
                    row["Ativo"] = "Sim";
                else
                    row["Ativo"] = "Não";
                row["Data de cadastro"] = item.RegisterDate;
                //row["Data de matricula"] = item.Name;
                //row["Aluno inativo (data da ultima aula)"] = item.Name;
                
                studentsGrid.Rows.Add(row);
            }

            var grid = new GridView();
            grid.DataSource = studentsGrid;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Relatorio.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            foreach (GridViewRow r in grid.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    for (int columnIndex = 0; columnIndex < r.Cells.Count; columnIndex++)
                    {
                        r.Cells[columnIndex].Attributes.Add("class", "text");
                    }
                }
            }

            grid.RenderControl(htw);

            string style = @"&lt;style> .text { mso-number-format:\@; } </style> ";
            Response.Output.Write(style);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        public FileResult Download(string file)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(file);
            string fileName = Path.GetFileName(file);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult OpenCalendarSubtitle()
        {
            return PartialView("_CalendarSubtitle");
        }
    }
}