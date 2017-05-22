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
    public class PlanController : Controller
    {
        private KaiutYogaContext db = new KaiutYogaContext();

        //
        // GET: /Plan/

        public ActionResult Index()
        {
            return View(db.PlanModels.ToList().OrderBy(c => c.Name));
        }

        //
        // GET: /Plan/Details/5

        public ActionResult Details(long id = 0)
        {
            PlanModel planmodel = db.PlanModels.Find(id);
            if (planmodel == null)
            {
                return HttpNotFound();
            }
            return View(planmodel);
        }

        //
        // GET: /Plan/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Plan/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlanModel planmodel)
        {
            if (ModelState.IsValid)
            {
                db.PlanModels.Add(planmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(planmodel);
        }

        //
        // GET: /Plan/Edit/5

        public ActionResult Edit(long id = 0)
        {
            PlanModel planmodel = db.PlanModels.Find(id);
            if (planmodel == null)
            {
                return HttpNotFound();
            }
            return View(planmodel);
        }

        //
        // POST: /Plan/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PlanModel planmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(planmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(planmodel);
        }

        //
        // GET: /Plan/Delete/5

        public ActionResult Delete(long id = 0)
        {
            PlanModel planmodel = db.PlanModels.Find(id);
            if (planmodel == null)
            {
                return HttpNotFound();
            }
            return View(planmodel);
        }

        //
        // POST: /Plan/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                PlanModel planmodel = db.PlanModels.Find(id);
                db.PlanModels.Remove(planmodel);
                db.SaveChanges();
                ViewBag.ErrorContent = null;
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorContent"] = "Houve um problema ao apagar este registro, primeiro selecione outro planos para os alunos que estao matriculados nessa frequencia.";
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