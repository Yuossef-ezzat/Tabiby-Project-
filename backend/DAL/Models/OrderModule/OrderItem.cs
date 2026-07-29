using DAL.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.OrderModule
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }


        
        public int MedicationId { get; set; }
        public virtual Medication Medication { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

    }
}
