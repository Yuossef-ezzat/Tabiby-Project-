using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment.paymob
{
    public class CreateIntentionResponse
    {
        public string Id { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}
