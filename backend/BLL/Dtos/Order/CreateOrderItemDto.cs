using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Order
{
    public class CreateOrderItemDto
    {
        public int MedicationId { get; set; }
        public int Quantity { get; set; }
    }
}
