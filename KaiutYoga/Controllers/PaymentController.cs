using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KaiutYoga.Models;
using PagedList;

namespace KaiutYoga.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private KaiutYogaContext db = new KaiutYogaContext();

        // GET: PaymentModels
        public ActionResult Index(string sortOrder, string currentFilter, string searchName, string searchMonth, string searchYear, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortPam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PlanSortParm = sortOrder == "plan" ? "plan_desc" : "plan";
            ViewBag.NotifiedSortParm = sortOrder == "notified" ? "notified_desc" : "notified";
            ViewBag.PaidSortParm = sortOrder == "paid" ? "paid_desc" : "paid";
            ViewBag.PeriodSortParm = sortOrder == "period" ? "period_desc" : "period";
            ViewBag.DueDateSortPam = sortOrder == "due" ? "due_desc" : "due";

            if (searchName != null)
            {
                page = 1;
            }
            else
            {
                searchName = currentFilter;
            }

            if (String.IsNullOrEmpty(searchMonth))
            {
                searchMonth = DateTime.Now.Month.ToString();
            }

            if (String.IsNullOrEmpty(searchYear))
            {
                searchYear = DateTime.Now.Year.ToString();
            }

            ViewBag.searchName = searchName;
            ViewBag.searchMonth = searchMonth;
            ViewBag.searchYear = searchYear;
            ViewBag.CurrentFilter = searchName;

            var paymentmodel = db.PaymentModels.Include(c => c.Student).Include(c => c.Plan).ToList();
            if (!String.IsNullOrEmpty(searchName))
            {
                paymentmodel = paymentmodel.Where(c => c.Student.Name.ToUpper().Contains(searchName.ToUpper())).ToList();
            }

            if (!String.IsNullOrEmpty(searchMonth))
            {
                paymentmodel = paymentmodel.Where(c => c.PeriodStart.Month == Convert.ToInt16(searchMonth) || c.PeriodEnd.Month == Convert.ToInt16(searchMonth)).ToList();
            }

            if (!String.IsNullOrEmpty(searchYear))
            {
                paymentmodel = paymentmodel.Where(c => c.PeriodStart.Year == Convert.ToInt16(searchYear) || c.PeriodEnd.Year == Convert.ToInt16(searchYear)).ToList();
            }
            
            switch (sortOrder)
            {
                case "due":
                    paymentmodel = paymentmodel.OrderBy(c => c.DueDate).ToList();
                    break;
                case "due_desc":
                    paymentmodel = paymentmodel.OrderByDescending(c => c.DueDate).ToList();
                    break;
                case "period":
                    paymentmodel = paymentmodel.OrderBy(c => c.PeriodStart).ToList();
                    break;
                case "period_desc":
                    paymentmodel = paymentmodel.OrderByDescending(c => c.PeriodEnd).ToList();
                    break;
                case "paid":
                    paymentmodel = paymentmodel.OrderBy(c => c.DatePaid).ToList();
                    break;
                case "paid_desc":
                    paymentmodel = paymentmodel.OrderByDescending(c => c.DatePaid).ToList();
                    break;
                case "notified":
                    paymentmodel = paymentmodel.OrderBy(c => c.Notified).ToList();
                    break;
                case "notified_desc":
                    paymentmodel = paymentmodel.OrderByDescending(c => c.Notified).ToList();
                    break;
                case "plan":
                    paymentmodel = paymentmodel.OrderBy(c => c.Plan.Name).ToList();
                    break;
                case "plan_desc":
                    paymentmodel = paymentmodel.OrderByDescending(c => c.Plan.Name).ToList();
                    break;
                case "name_desc":
                    paymentmodel = paymentmodel.OrderByDescending(c => c.Student.Name).ToList();
                    break;
                default:
                    paymentmodel = paymentmodel.OrderBy(c => c.Student.Name).ToList();
                    break;

            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(paymentmodel.ToPagedList(pageNumber, pageSize)); 
        }

        
        public ActionResult CreateMissing()
        {
            this.CreateMissingPayments(DateTime.Now);
            return RedirectToAction("Index");//,"Payment", new { ViewBag.sortOrder, ViewBag.currentFilter, ViewBag.searchName, ViewBag.searchMonth, ViewBag.searchYear, ViewBag.page});

        }
        private void CreateMissingPayments(DateTime date)
        {
            List<StudentModel> studentModel = db.StudentModels.Include(c => c.Payments).Where(c=>c.Active).ToList();
            foreach (StudentModel student in studentModel)
            {
                List<PaymentModel> payments = student.CreateOrGetPayment(date);

                foreach (PaymentModel pm in payments)
                {
                    if (pm.Id == 0)
                    {
                        db.PaymentModels.Add(pm);
                        db.SaveChanges();
                    }
                }
            }
        }

        // GET: PaymentModels/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentModel paymentModel = db.PaymentModels.Find(id);
            if (paymentModel == null)
            {
                return HttpNotFound();
            }
            return View(paymentModel);
        }

        // GET: PaymentModels/Create
        public ActionResult Create()
        {
            ViewBag.PlanId = new SelectList(db.PlanModels, "Id", "Name");
            ViewBag.StudentId = new SelectList(db.StudentModels, "Id", "FirstName");
            return View();
        }

        // POST: PaymentModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PeriodStart,PeriodEnd,Period,AmountPaid,DatePaid,Notified,AmountToPay,PlanId,StudentId,DueDate")] PaymentModel paymentModel)
        {
            if (ModelState.IsValid)
            {
                db.PaymentModels.Add(paymentModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PlanId = new SelectList(db.PlanModels, "Id", "Name", paymentModel.PlanId);
            ViewBag.StudentId = new SelectList(db.StudentModels, "Id", "FirstName", paymentModel.StudentId);
            return View(paymentModel);
        }

        // GET: PaymentModels/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentModel paymentModel = db.PaymentModels.Find(id);
            if (paymentModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlanId = new SelectList(db.PlanModels, "Id", "Name", paymentModel.PlanId);
            ViewBag.StudentId = new SelectList(db.StudentModels, "Id", "FirstName", paymentModel.StudentId);
            return View(paymentModel);
        }

        // POST: PaymentModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PeriodStart,PeriodEnd,Period,DueDate,AmountPaid,DatePaid,Notified,AmountToPay,PlanId,StudentId")] PaymentModel paymentModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paymentModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlanId = new SelectList(db.PlanModels, "Id", "Name", paymentModel.PlanId);
            ViewBag.StudentId = new SelectList(db.StudentModels, "Id", "FirstName", paymentModel.StudentId);
            return View(paymentModel);
        }

        // GET: PaymentModels/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentModel paymentModel = db.PaymentModels.Find(id);
            if (paymentModel == null)
            {
                return HttpNotFound();
            }
            paymentModel.Plan = db.PlanModels.Find(paymentModel.PlanId);
            paymentModel.Student = db.StudentModels.Find(paymentModel.StudentId);
            
            return View(paymentModel);
        }

        // POST: PaymentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            PaymentModel paymentModel = db.PaymentModels.Find(id);
            db.PaymentModels.Remove(paymentModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GetAmountToPay(string from="",string to="",string due="",int planId = 0)
        {
            if (from == "" || to == "" || due == "" || planId == 0)
                return "";
            DateTime start = new DateTime(Convert.ToInt16(from.Split('-')[0]), Convert.ToInt16(from.Split('-')[1]), Convert.ToInt16(from.Split('-')[2]));
            DateTime end = new DateTime(Convert.ToInt16(to.Split('-')[0]), Convert.ToInt16(to.Split('-')[1]), Convert.ToInt16(to.Split('-')[2]));
            DateTime duedate = new DateTime(Convert.ToInt16(due.Split('-')[0]), Convert.ToInt16(due.Split('-')[1]), Convert.ToInt16(due.Split('-')[2]));
            PaymentModel pm = new PaymentModel(0, planId, start, end);
            pm.DueDate = duedate;
            return pm.AmountToPay.ToString("0.00");
        }

    }
}
