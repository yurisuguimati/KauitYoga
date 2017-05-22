using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KaiutYoga.Models
{
    public class PlanModel
    {
        [Key]
        public long Id { get; set; }
        [DisplayName("Nome do plano")]
        public string Name { get; set; }
        
        [Display(Name="Frequencia")]
        public int Frequency { get; set; }
        [Display(Name = "Valor (R$)")]
        public decimal Price { get; set; }

        [DisplayName("Último dia para pagamento com desconto")]
        public int? LastDayToPayWithDiscount { get; set; }
        [DisplayName("Desconto(%)")]
        public decimal Discount { get; set; }

        public float Interest { get; set; }
        public float Fine { get; set; }

        public bool IsTrial { get; set; }

        public List<PaymentModel> Payments { get; set; }
    }
}