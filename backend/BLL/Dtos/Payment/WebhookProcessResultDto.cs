using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment
{
    public class WebhookProcessResultDto
    {
        public int? PaymentId { get; set; }
        public bool WasAlreadyProcessed { get; set; }
    }
}
