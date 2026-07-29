using DAL.Models.OrderModule;
using DAL.Models.Users;
using DAL.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.BasketModule
{
    public class CustomerBasket : BaseEntity
    {

        public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
        public int PatientId { get; set; }

        public Patient Patient { get; set; }
        public bool IsCheckedOut { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
