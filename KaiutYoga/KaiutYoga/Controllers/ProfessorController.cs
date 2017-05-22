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
    [Authorize]
    public class ProfessorController : Controller
    {
        private KaiutYogaContext db = new KaiutYogaContext();

        //
        // GET: /Professor/

        public ActionResult Index()
        {
            return View(db.ProfessorModels.ToList().OrderBy(c=>c.Name));
        }

        //
        // GET: /Professor/Details/5

        public ActionResult Details(long id = 0)
        {
            ProfessorModel professormodel = db.ProfessorModels.Find(id);
            if (professormodel == null)
            {
                return HttpNotFound();
            }
            return View(professormodel);
        }

        //
        // GET: /Professor/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Professor/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProfessorModel professormodel)
        {
            if (ModelState.IsValid)
            {
                db.ProfessorModels.Add(professormodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(professormodel);
        }

        //
        // GET: /Professor/Edit/5

        public ActionResult Edit(long id = 0)
        {
            ProfessorModel professormodel = db.ProfessorModels.Find(id);
            if (professormodel == null)
            {
                return HttpNotFound();
            }
            return View(professormodel);
        }

        //
        // POST: /Professor/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProfessorModel professormodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(professormodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(professormodel);
        }

        //
        // GET: /Professor/Delete/5

        public ActionResult Delete(long id = 0)
        {
            ProfessorModel professormodel = db.ProfessorModels.Find(id);
            if (professormodel == null)
            {
                return HttpNotFound();
            }
            return View(professormodel);
        }

        //
        // POST: /Professor/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                ProfessorModel professormodel = db.ProfessorModels.Find(id);
                db.ProfessorModels.Remove(professormodel);
                db.SaveChanges();
                ViewBag.ErrorContent = null;
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorContent"]="Houve um problema ao apagar este registro, primeiro selecione outro professor para as aulas que este professor está encarregado";
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}