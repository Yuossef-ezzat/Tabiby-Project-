using DAL.Models.Consultation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.ConsultationSpecs
{
    public class UnReadMessagesSpecs : BaseSpecification<ConsultationMessage>
    {
        public UnReadMessagesSpecs(int consultationId,int requesterId) :
                base( c=> c.ConsultationId == consultationId && c.SenderUserId != requesterId &&!c.IsRead)
        {
            
        }
    }
}
