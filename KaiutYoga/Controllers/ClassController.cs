using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaiutYoga.Models;
using KaiutYoga.ViewModels;
using PagedList;
using Newtonsoft.Json;

namespace KaiutYoga.Controllers
{
    [Authorize]
    public class ClassController : Controller
    {
        private KaiutYogaContext db = new KaiutYogaContext();

        // GET: /CreateClass/
        public ActionResult Index(string sortOrder, string currentFilter, string searchDayOfWeek, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TimeSortParm = String.IsNullOrEmpty(sortOrder) ? "time_desc" : "";
            ViewBag.ProfessorSortParm = sortOrder == "professor" ? "professor_desc" : "professor";
            ViewBag.RoomSortParm = sortOrder == "room" ? "room_desc" : "room";
            ViewBag.DaySortParm = sortOrder == "day" ? "day_desc" : "day";

            if (searchDayOfWeek != null)
            {
                page = 1;
            }
            else
            {
                searchDayOfWeek = currentFilter;
            }

            ViewBag.CurrentFilter = searchDayOfWeek;


            var classmodel = db.ClassModels.Include(c => c.Professor).Include(c => c.Room).Where(c => c.Active).ToList();
            if (!String.IsNullOrEmpty(searchDayOfWeek))
            {
                classmodel = classmodel.Where(c => c.Day == (DayOfWeek)Enum.Parse(typeof(DayOfWeek), searchDayOfWeek, true)).ToList();// Professor.Name.Contains(searchString) || s.Room.Name.Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "professor":
                    classmodel = classmodel.OrderBy(c => c.Professor.Name).ToList();
                    break;
                case "professor_desc":
                    classmodel = classmodel.OrderByDescending(c => c.Professor.Name).ToList();
                    break;
                case "room":
                    classmodel = classmodel.OrderBy(c => c.Room.Name).ToList();
                    break;
                case "room_desc":
                    classmodel = classmodel.OrderByDescending(c => c.Room.Name).ToList();
                    break;
                case "day":
                    classmodel = classmodel.OrderBy(c => c.Day).ToList();
                    break;
                case "day_desc":
                    classmodel = classmodel.OrderByDescending(c => c.Day).ToList();
                    break;
                case "time_desc":
                    classmodel = classmodel.OrderByDescending(c => c.SelectedTime).ToList();
                    break;
                default:
                    classmodel = classmodel.OrderBy(c => c.SelectedTime).ToList();
                    break;

            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(classmodel.ToPagedList(pageNumber, pageSize));
        }

        // GET: /CreateClass/Details/5
        public ActionResult Details(long id = 0)
        {
            ClassModel ClassModel = db.ClassModels.Include(c => c.Professor).Include(c => c.Room).Where(c => c.Id == id).Single();
            if (ClassModel == null)
            {
                return HttpNotFound();
            }
            return View(ClassModel);
        }

        // GET: /CreateClass/Create
        public ActionResult Create()
        {
            ViewBag.ProfessorId = new SelectList(db.ProfessorModels, "Id", "Name");
            ViewBag.RoomId = new SelectList(db.RoomModels, "Id", "Name");

            ClassModel classmodel = new ClassModel();
            classmodel.Time = this.PopulateTime().ToList().Select(time => new SelectListItem { Value = time, Text = time });

            return View(classmodel);
        }

        // POST: /CreateClass/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassModel classmodel)
        {
            if (ModelState.IsValid)
            {
                db.ClassModels.Add(classmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProfessorId = new SelectList(db.ProfessorModels, "Id", "Name", classmodel.ProfessorId);
            ViewBag.RoomId = new SelectList(db.RoomModels, "Id", "Name", classmodel.RoomId);
            return View(classmodel);
        }

        // GET: /CreateClass/CreateBatch
        public ActionResult Batch()
        {
            ViewBag.ProfessorId = new SelectList(db.ProfessorModels, "Id", "Name");
            ViewBag.RoomId = new SelectList(db.RoomModels, "Id", "Name");

            CreateClassModel createclassmodel = new CreateClassModel();
            createclassmodel.Time = this.PopulateTime().ToList().Select(time => new SelectListItem { Value = time, Text = time });

            return View(createclassmodel);
        }

        // POST: /CreateClass/CreateBatch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Batch(CreateClassModel createclassmodel)
        {
            if (ModelState.IsValid)
            {
                List<ClassModel> listclassmodel = this.CreateClasses(createclassmodel);
                foreach (ClassModel classmodel in listclassmodel)
                {
                    db.ClassModels.Add(classmodel);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            ViewBag.ProfessorId = new SelectList(db.ProfessorModels, "Id", "Name", createclassmodel.ProfessorId);
            ViewBag.RoomId = new SelectList(db.RoomModels, "Id", "Name", createclassmodel.RoomId);
            return View(createclassmodel);
        }

        // GET: /CreateClass/Edit/5
        public ActionResult Edit(long id = 0)
        {
            ClassModel classmodel = db.ClassModels.Find(id);

            if (classmodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProfessorId = new SelectList(db.ProfessorModels, "Id", "Name", classmodel.ProfessorId);
            ViewBag.RoomId = new SelectList(db.RoomModels, "Id", "Name", classmodel.RoomId);

            classmodel.Time = this.PopulateTime().ToList().Select(time => new SelectListItem { Value = time, Text = time });

            return View(classmodel);
        }

        // POST: /CreateClass/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClassModel classmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProfessorId = new SelectList(db.ProfessorModels, "Id", "Name", classmodel.ProfessorId);
            ViewBag.RoomId = new SelectList(db.RoomModels, "Id", "Name", classmodel.RoomId);
            return View(classmodel);
        }

        // GET: /CreateClass/Delete/5
        public ActionResult Delete(long id = 0)
        {
            ClassModel classmodel = db.ClassModels.Find(id);
            classmodel.Professor = db.ProfessorModels.Find(classmodel.ProfessorId);
            classmodel.Room = db.RoomModels.Find(classmodel.RoomId);
            if (classmodel == null)
            {
                return HttpNotFound();
            }
            return View(classmodel);
        }

        // POST: /CreateClass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClassModel classmodel = db.ClassModels.Find(id);
            if (classmodel.Students.Count != 0 || classmodel.HaveAnyPresenceList())
                classmodel.Active = false;
            else
                db.ClassModels.Remove(classmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public List<string> PopulateTime()
        {
            //Set the Start Time Value
            DateTime start = DateTime.ParseExact("07:00", "HH:mm", null);
            //Set the End Time Value
            DateTime end = DateTime.ParseExact("21:00", "HH:mm", null);
            //Set the interval time 
            int interval = 15;
            //List to hold the values of intervals
            List<string> lstTimeIntervals = new List<string>();
            //Populate the list with 30 minutes interval values
            for (DateTime i = start; i <= end; i = i.AddMinutes(interval))
                lstTimeIntervals.Add(i.ToString("HH:mm"));
            return lstTimeIntervals;
        }

        public List<ClassModel> CreateClasses(CreateClassModel createclassmodel)
        {
            List<ClassModel> classes = new List<ClassModel>();
            ClassModel aula;
            DateTime start = DateTime.ParseExact(createclassmodel.FirstClass, "HH:mm", null);
            DateTime end = DateTime.ParseExact(createclassmodel.LastClass, "HH:mm", null);
            for (DateTime i = start; i <= end; i = i.AddMinutes(createclassmodel.Interval))
            {
                aula = new ClassModel();
                aula.Day = createclassmodel.Day;
                aula.Professor = createclassmodel.Professor;
                aula.ProfessorId = createclassmodel.ProfessorId;
                aula.Room = createclassmodel.Room;
                aula.RoomId = createclassmodel.RoomId;
                aula.SelectedTime = i.ToString("HH:mm");
                classes.Add(aula);
            }
            return classes;
        }

        public ActionResult FilterDayOfWeek(DayOfWeek dayOfWeek)
        {
            var classmodels = db.ClassModels.Include(c => c.Professor).Include(c => c.Room);
            classmodels = classmodels.Where(c => c.Day == (DayOfWeek)dayOfWeek);
            List<ClassModel> listclassmodels = new List<ClassModel>();
            listclassmodels = classmodels.ToList();
            return View("Index", listclassmodels.OrderBy(c => c.SelectedTime));
        }

        public ActionResult Overview(long id = 0, string date = "")
        {
            ClassOverview co = new ClassOverview(id, date);

            return View(co);
        }

        public DateTime ConvertMillisecondsToDate(string date)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(Convert.ToDouble(date));
        }

        // GET
        public ActionResult Presence(long classId = 0, string date = "")
        {
            DateTime classDate = this.ConvertMillisecondsToDate(date);
            PresenceModel presencemodel = null;

            List<PresenceModel> lpm = db.PresenceModels.Include(c => c.EnroledWeeklyStudents).Include(c => c.PresentWeeklyStudents)
                    .Include(c => c.EnroledTrialStudents).Include(c => c.PresentTrialStudents)
                    .Include(c => c.EnroledReplacementStudents).Include(c => c.PresentTrialStudents)
                    .Where(c => c.ClassId == classId).Where(c => c.Date == classDate).ToList();

            if (lpm.Count > 0)
                presencemodel = lpm[0];
            else
                presencemodel = null;

            // first time checking presence, need to add a record
            if (presencemodel == null)
            {
                ClassModel classmodel = db.ClassModels.Include(c => c.Students).Include(c => c.Room).Include(c => c.Professor).Where(c => c.Id == classId).SingleOrDefault();
                if (classmodel == null)
                    return HttpNotFound();
                presencemodel = new PresenceModel();
                //presencemodel.Class = classmodel;
                presencemodel.ClassId = classmodel.Id;
                presencemodel.Date = classDate;
                presencemodel.PresentWeeklyStudents = new List<StudentModel>();
                presencemodel.PresentReplacementStudents = new List<StudentModel>();
                presencemodel.PresentTrialStudents = new List<StudentModel>();

                presencemodel.EnroledWeeklyStudents = classmodel.WeeklyStudents(classDate, db);
                presencemodel.EnroledTrialStudents = classmodel.TrialStudents(classDate, db);
                presencemodel.EnroledReplacementStudents = classmodel.ReplacementStudents(classDate, db);

                db.PresenceModels.Add(presencemodel);
                db.SaveChanges();
            }

            PopulatePresentStudents(presencemodel);

            return View(presencemodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Presence(long id, FormCollection formCollection, string[] presentStudent)
        {
            //PresenceModel presencemodel = db.PresenceModels.Include(c => c.Class).Include(c => c.StudentsAttendance).Include("EnroledStudents").Where(c => c.Id == id).Single();
            //PresenceModel presencemodel = db.PresenceModels.Include(c => c.Class).Include(c => c.StudentsAttendance).Where(c => c.Id == id).Single();
            List<PresenceModel> lpm = db.PresenceModels.ToList();
            PresenceModel presencemodel = lpm.Find(c => c.Id == id);

            if (presentStudent != null)
            {
                var presentStudentsHS = new HashSet<string>(presentStudent);

                // WEEKLY STUDENTS
                var studentsInClass = new HashSet<long>(presencemodel.PresentWeeklyStudents.Select(c => c.Id));
                foreach (var student in presencemodel.EnroledWeeklyStudents)
                {
                    if (presentStudentsHS.Contains(student.Id.ToString()))
                    {
                        if (!studentsInClass.Contains(student.Id))
                        {
                            presencemodel.PresentWeeklyStudents.Add(student);
                        }
                    }
                    else
                    {
                        if (studentsInClass.Contains(student.Id))
                        {
                            presencemodel.PresentWeeklyStudents.Remove(student);
                        }
                    }
                }

                // REPLACEMENT STUDENTS
                studentsInClass = new HashSet<long>(presencemodel.PresentReplacementStudents.Select(c => c.Id));
                foreach (var student in presencemodel.EnroledReplacementStudents)
                {
                    if (presentStudentsHS.Contains(student.Id.ToString()))
                    {
                        if (!studentsInClass.Contains(student.Id))
                        {
                            presencemodel.PresentReplacementStudents.Add(student);
                        }
                    }
                    else
                    {
                        if (studentsInClass.Contains(student.Id))
                        {
                            presencemodel.PresentReplacementStudents.Remove(student);
                        }
                    }
                }

                // TRIAL STUDENTS
                studentsInClass = new HashSet<long>(presencemodel.PresentTrialStudents.Select(c => c.Id));
                foreach (var student in presencemodel.EnroledTrialStudents)
                {
                    if (presentStudentsHS.Contains(student.Id.ToString()))
                    {
                        if (!studentsInClass.Contains(student.Id))
                        {
                            presencemodel.PresentTrialStudents.Add(student);
                        }
                    }
                    else
                    {
                        if (studentsInClass.Contains(student.Id))
                        {
                            presencemodel.PresentTrialStudents.Remove(student);
                        }
                    }
                }
            }
            else
            {
                presencemodel.PresentWeeklyStudents.Clear(); new List<StudentModel>();
                presencemodel.PresentReplacementStudents.Clear();// = new List<StudentModel>();
                presencemodel.PresentTrialStudents.Clear();// = new List<StudentModel>();
            }

            db.Entry(presencemodel).State = EntityState.Modified;
            db.SaveChanges();

            PopulatePresentStudents(presencemodel);

            ClassOverview co = new ClassOverview(presencemodel.ClassId, presencemodel.ConvertToMilliseconds);

            return View("Overview", co);
        }

        public bool GetClassDate(string date = "")
        {
            if (date != "")
            {
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0); //Set default date 1/1/1970
                dt = dt.AddMilliseconds(Convert.ToDouble(date));
                TempData["ClassDate"] = dt;
            }
            else
                TempData.Remove("ClassDate");

            return true;
        }

        private void PopulatePresentStudents(PresenceModel presencemodel)
        {
            if (presencemodel == null)
                return;
            StudentModel sm = new StudentModel();

            var weeklyStudents = presencemodel.EnroledWeeklyStudents;
            var presentWeeklyStudents = new HashSet<long>(presencemodel.PresentWeeklyStudents.Select(c => c.Id));
            var viewModel = new List<StudentsAttendance>();
            foreach (var student in weeklyStudents)
            {
                sm = db.StudentModels.Find(student.Id);
                viewModel.Add(new StudentsAttendance
                {
                    StudentId = sm.Id,
                    Name = sm.FirstName + " " + sm.LastName,
                    Presence = presentWeeklyStudents.Contains(sm.Id)
                });
            }

            var trialStudents = presencemodel.EnroledTrialStudents;
            var presentTrialStudents = new HashSet<long>(presencemodel.PresentTrialStudents.Select(c => c.Id));
            foreach (var student in trialStudents)
            {
                sm = db.StudentModels.Find(student.Id);
                viewModel.Add(new StudentsAttendance
                {
                    StudentId = sm.Id,
                    Name = sm.FirstName + " " + sm.LastName,
                    Presence = presentTrialStudents.Contains(sm.Id)
                });
            }

            var replacementStudents = presencemodel.EnroledReplacementStudents;
            var presentReplacementStudents = new HashSet<long>(presencemodel.PresentReplacementStudents.Select(c => c.Id));
            foreach (var student in replacementStudents)
            {
                sm = db.StudentModels.Find(student.Id);
                viewModel.Add(new StudentsAttendance
                {
                    StudentId = sm.Id,
                    Name = sm.FirstName + " " + sm.LastName,
                    Presence = presentReplacementStudents.Contains(sm.Id)
                });
            }

            ViewBag.Students = viewModel;
        }

        public ActionResult GetInformation(long classId = 0, string date = "")
        {
            ClassOverview co = new ClassOverview(classId, date);

            return PartialView("_ClassInformation", co);
        }

        [HttpPost]
        public string GetClasses(List<ClassJson> lcj)
        {
            try
            {
                foreach (ClassJson cj in lcj)
                {
                    ClassModel cm = db.ClassModels.Find(cj.ClassId);

                    DateTime dt = TimeZoneInfo.ConvertTimeToUtc(cj.Start);

                    List<StudentModel> lsmw = cm.WeeklyStudents(dt, db);
                    cj.AmountWeeklyStudents = lsmw.Count;

                    List<StudentModel> lsmt = cm.TrialStudents(dt, db);
                    cj.AmountTrialStudents = lsmt.Count;

                    List<StudentModel> lsmr = cm.ReplacementStudents(dt, db);
                    cj.AmountReplacementStudents = lsmr.Count;

                    PresenceModel pm = cm.PresenceList(dt);
                    if (pm != null)
                    {
                        foreach (StudentModel sm in pm.EnroledWeeklyStudents)
                        {
                            // if the user in the presence list is not enroled in that class
                            if (lsmw.Find(c => c.Id == sm.Id) == null)
                                cj.AmountWeeklyStudents++;
                        }

                        foreach (StudentModel sm in pm.EnroledTrialStudents)
                        {
                            // if the user in the presence list is not enroled in that class
                            if (lsmt.Find(c => c.Id == sm.Id) == null)
                                cj.AmountTrialStudents++;

                            // The user also appear in the weekly students list
                            if (lsmw.Find(c => c.Id == sm.Id) != null)
                                cj.AmountWeeklyStudents--;
                        }

                        foreach (StudentModel sm in pm.EnroledReplacementStudents)
                        {
                            // if the user in the presence list is not enroled in that class
                            if (lsmr.Find(c => c.Id == sm.Id) == null)
                                cj.AmountReplacementStudents++;
                        }

                    }

                }
            }
            catch (Exception err)
            {

            }
            return JsonConvert.SerializeObject(lcj, Formatting.Indented);
        }

        public ActionResult DeletePresence(long classId = 0, string date = "")
        {
            DateTime classDate = this.ConvertMillisecondsToDate(date);
            PresenceModel presencemodel = null;

            List<PresenceModel> lpm = db.PresenceModels.Include(c => c.EnroledWeeklyStudents).Include(c => c.PresentWeeklyStudents)
                .Include(c => c.EnroledTrialStudents).Include(c => c.PresentTrialStudents)
                .Include(c => c.EnroledReplacementStudents).Include(c => c.PresentTrialStudents)
                .Where(c => c.ClassId == classId).Where(c => c.Date == classDate).ToList();

            ClassOverview co = new ClassOverview(classId, date);

            if (lpm.Count > 0)
            {
                presencemodel = lpm[0];

                try
                {
                    db.PresenceModels.Remove(presencemodel);
                    db.SaveChanges();
                    ViewBag.ErrorContent = null;
                    return View("Overview", co);
                }
                catch
                {
                    TempData["ErrorContent"] = "Houve um problema ao apagar este registro";
                    return View("Overview", co);
                }
            }

            return View("Overview", co);
        }
    }
}