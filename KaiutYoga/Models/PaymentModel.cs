using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KaiutYoga.Models
{
    public class PaymentModel
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PeriodStart { get; set; }

        [DisplayName("Final")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PeriodEnd { get; set; }

        public int Month {
            get
            {
                return PeriodStart.Month;
            }
        }
        public int Year
        {
            get
            {
                return PeriodStart.Year;
            }
        }

        [Display(Name = "Período")]
        public string Period
        {
            get
            {
                //return PeriodStart.ToString("MM/yyyy") + " - " + PeriodEnd.ToString("MM/yyyy");
                return PeriodStart.ToString("MM/yyyy");
            }
            set { }
        }

        [Display(Name = "Valor pago")]
        public decimal? AmountPaid { get; set; }

        [DisplayName("Data de pagamento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DatePaid { get; set; }

        [DisplayName("Data de vencimento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }


        [Display(Name = "Notificado")]
        public bool Notified { get; set; } = false;

        [Display(Name ="Valor a pagar")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal AmountToPay {
            get
            {

                PlanModel planmodel = this.Plan;
                if (this.Plan == null)
                {
                    KaiutYogaContext db = new KaiutYogaContext();
                    planmodel = db.PlanModels.Find(this.PlanId);
                }

                decimal proportionalPrice = (decimal)(PeriodEnd.Day - PeriodStart.Day + 1) / (decimal)DateTime.DaysInMonth(PeriodEnd.Year, PeriodEnd.Month);
                proportionalPrice = planmodel.Price * proportionalPrice;

                // Ja foi pago
                if (AmountPaid != 0 && DatePaid != null)
                {
                    return (decimal)AmountPaid;
                }
                // não foi pago
                else
                {
                    // se estiver dentro do vencimento
                    if (DateTime.Now < this.DueDate)
                    {
                        return proportionalPrice * (decimal)(1 - planmodel.Discount / 10);
                    }
                    // valor com juros/multa
                    return proportionalPrice;
                }
            }
        }

        public long? PlanId { get; set; }
        
        [ForeignKey("PlanId")]
        public PlanModel Plan { get; set; }

        public long? StudentId { get; set; }
        [Column("Name")]
        [ForeignKey("StudentId")]
        public StudentModel Student { get; set; }


        public PaymentModel() { }

        public PaymentModel(long studentId, long planId, DateTime start, DateTime end)
        {
            KaiutYogaContext db = new KaiutYogaContext();
            PaymentModel payment = new PaymentModel();
            List<PaymentModel> lpm = new List<PaymentModel>();
            PlanModel planmodel = db.PlanModels.Find(planId);
            

            this.StudentId = studentId;
            this.PlanId = planId;

            this.PeriodStart = start;
            this.PeriodEnd = end;

            DateTime dueDate = this.PeriodEnd;
            int lastDay = planmodel.LastDayToPayWithDiscount == null ? 10 : (int)planmodel.LastDayToPayWithDiscount;
            this.DueDate = new DateTime(dueDate.Year, dueDate.Month, lastDay);
        }


        public PaymentModel(StudentModel student, PlanModel plan, DateTime date)
        {
            KaiutYogaContext db = new KaiutYogaContext();
            PaymentModel payment = new PaymentModel();
            List<PaymentModel> lpm = new List<PaymentModel>();
            PlanModel planmodel = db.PlanModels.Find(plan.Id);
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            DateTime start = new DateTime(date.Year, date.Month, 1);
            DateTime end = new DateTime(date.Year, date.Month, daysInMonth);

            this.StudentId = student.Id;
            this.PlanId = plan.Id;

            this.PeriodStart = start;
            this.PeriodEnd = end;

            DateTime dueDate = this.PeriodEnd;
            int lastDay = planmodel.LastDayToPayWithDiscount == null ? 10 : (int)planmodel.LastDayToPayWithDiscount;
            this.DueDate = new DateTime(dueDate.Year, dueDate.Month, lastDay);

        }

        public PaymentModel (ref PaymentModel paymentToChange, StudentModel student, long planId, DateTime start)
        {
            KaiutYogaContext db = new KaiutYogaContext();
            PlanModel planmodel = db.PlanModels.Find(planId);

            this.PeriodStart = start;
            this.PeriodEnd = paymentToChange.PeriodEnd;
            this.StudentId = student.Id;
            this.PlanId = planmodel.Id;

            int diff = ((this.PeriodEnd.Day - this.PeriodStart.Day) + 1);
            
            paymentToChange.PeriodEnd = start.AddDays(-1);

            DateTime dueDate = this.PeriodEnd;
            int lastDay = planmodel.LastDayToPayWithDiscount == null ? 10 : (int)planmodel.LastDayToPayWithDiscount;
            this.DueDate = new DateTime(dueDate.Year, dueDate.Month, lastDay);
        }
    }
}