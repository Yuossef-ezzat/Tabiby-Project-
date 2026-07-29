using DAL.Models.Users;
using DAL.Shared;
using DomainLayer.Models.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.OrderModule
{
    public class Medication : BaseEntity
    {

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? PictureUrl { get; set; }

        public bool IsAvailable { get; set; }
        

                public virtual List<OrderItem> Order_Item { get; set; }
        public ICollection<BasketItem> BasketItems { get; set; }= new List<BasketItem>();
        public int PharmacistId { get; set; }

        public virtual Pharmacist Pharmacist { get; set; }
    }
}
