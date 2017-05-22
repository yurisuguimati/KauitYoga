using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaiutYoga.Models;

namespace KaiutYoga.Controllers
{
    public class ClassControllerOld : Controller
    {
        private KaiutYogaContext db = new KaiutYogaContext();

        //
        // GET: /Class/

        public ActionResult Index()
        {
            var classmodels = db.ClassModels.Include(c => c.Professor).Include(c => c.Room);
            
            return View(classmodels.ToList().OrderBy(c => c.SelectedTime));
        }

        public ActionResult FilterDayOfWeek(DayOfWeek dayOfWeek)
        {
            var classmodels = db.ClassModels.Include(c => c.Professor).Include(c => c.Room);
            classmodels = classmodels.Where(c => c.Day == (DayOfWeek)dayOfWeek);
            return View("Index",classmodels.ToList().OrderBy(c => c.SelectedTime));
        }

        //
        // GET: /Class/Details/5

        public ActionResult Details(long id = 0)
        {
            ClassModel classmodel = db.ClassModels.Find(id);
            if (classmodel == null)
            {
                return HttpNotFound();
            }
            return View(classmodel);
        }

        //
        // GET: /Class/Create

        public ActionResult Create()
        {
            ViewBag.ProfessorId = new SelectList(db.ProfessorModels, "Id", "Name");
            ViewBag.RoomId = new SelectList(db.RoomModels, "Id", "Name");
            
            ClassModel classmodel = new ClassModel();
            classmodel.Time = this.PopulateTime().ToList().Select(time => new SelectListItem { Value = time, Text = time });

            return View(classmodel);
            //return View();
        }

        //
        // POST: /Class/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassModel classmodel)
        {
            //var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                db.ClassModels.Add(classmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProfessorId = new SelectList(db.ProfessorModels, "Id", "Name", classmodel.ProfessorId);
            ViewBag.RoomId = new SelectList(db.RoomModels, "Id", "Name", classmodel.RoomId);
            classmodel.Time = this.PopulateTime().ToList().Select(time => new SelectListItem { Value = time, Text = time });
            return View(classmodel);
        }

        //
        // GET: /Class/Edit/5

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

        //
        // POST: /Class/Edit/5

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

            classmodel.Time = this.PopulateTime().ToList().Select(time => new SelectListItem { Value = time, Text = time });

            return View(classmodel);
        }

        //
        // GET: /Class/Delete/5

        public ActionResult Delete(long id = 0)
        {
            ClassModel classmodel = db.ClassModels.Find(id);
            if (classmodel == null)
            {
                return HttpNotFound();
            }
            return View(classmodel);
        }

        //
        // POST: /Class/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClassModel classmodel = db.ClassModels.Find(id);
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
            DateTime end = DateTime.ParseExact("19:00", "HH:mm", null);
            //Set the interval time 
            int interval = 30;
            //List to hold the values of intervals
            List<string> lstTimeIntervals = new List<string>();
            //Populate the list with 30 minutes interval values
            for (DateTime i = start; i <= end; i = i.AddMinutes(interval))
                lstTimeIntervals.Add(i.ToString("HH:mm"));
            return lstTimeIntervals;
        }

        public ActionResult Overview(long id = 0)
        {
            ClassModel classmodel = db.ClassModels.Find(id);
            if (classmodel == null)
            {
                return HttpNotFound();
            }
            return View(classmodel);
        }
    }
}