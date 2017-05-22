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
    public class RoomController : Controller
    {
        private KaiutYogaContext db = new KaiutYogaContext();

        //
        // GET: /Room/

        public ActionResult Index()
        {
            return View(db.RoomModels.ToList().OrderBy(c=>c.Name));
        }

        //
        // GET: /Room/Details/5

        public ActionResult Details(long id = 0)
        {
            RoomModel roommodel = db.RoomModels.Find(id);
            if (roommodel == null)
            {
                return HttpNotFound();
            }
            return View(roommodel);
        }

        //
        // GET: /Room/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Room/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomModel roommodel)
        {
            if (ModelState.IsValid)
            {
                db.RoomModels.Add(roommodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roommodel);
        }

        //
        // GET: /Room/Edit/5

        public ActionResult Edit(long id = 0)
        {
            RoomModel roommodel = db.RoomModels.Find(id);
            if (roommodel == null)
            {
                return HttpNotFound();
            }
            return View(roommodel);
        }

        //
        // POST: /Room/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoomModel roommodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roommodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roommodel);
        }

        //
        // GET: /Room/Delete/5

        public ActionResult Delete(long id = 0)
        {
            RoomModel roommodel = db.RoomModels.Find(id);
            if (roommodel == null)
            {
                return HttpNotFound();
            }
            return View(roommodel);
        }

        //
        // POST: /Room/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                RoomModel roommodel = db.RoomModels.Find(id);
                db.RoomModels.Remove(roommodel);
                db.SaveChanges();
                ViewBag.ErrorContent = null;
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorContent"]="Houve um problema ao apagar este registro, primeiro selecione outra sala para as aulas que são feitas nesta sala";
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