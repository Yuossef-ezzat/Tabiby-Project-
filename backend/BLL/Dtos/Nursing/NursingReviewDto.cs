using DAL.Models.NursingModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Nursing
{
    public class NursingReviewDto
    {
        public int NursingRequestId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

    }
}
