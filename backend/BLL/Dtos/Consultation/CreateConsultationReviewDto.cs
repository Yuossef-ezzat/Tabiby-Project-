using DAL.Models.Consultation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Consultion
{
    public class CreateConsultationReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
