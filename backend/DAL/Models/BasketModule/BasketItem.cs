using DAL.Models.OrderModule;
using DAL.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.BasketModule
{
    public class BasketItem : BaseEntity
    {
        public Medication Medication { get; set; }

        public int MedicationId { get; set; }
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
