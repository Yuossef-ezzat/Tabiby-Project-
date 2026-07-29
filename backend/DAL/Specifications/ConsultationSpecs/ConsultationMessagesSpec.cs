using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.ConsultationSpecs
{
    public class ConsultationMessagesSpecByConsultationId : BaseSpecification<ConsultationMessage>
    {
        public ConsultationMessagesSpecByConsultationId(int consultationId) : base(m => m.ConsultationId == consultationId)
        {
            AddInclude(m => m.Sender);
            ApplyOrderBy(m => m.SentAt);
            
        }

    }
    public class ConsultationMessageByIdSpec : BaseSpecification<ConsultationMessage>
    {
        public ConsultationMessageByIdSpec(int messageId) : base(m => m.Id == messageId)
        {
            AddInclude(m => m.Sender);
        }
    }
}
