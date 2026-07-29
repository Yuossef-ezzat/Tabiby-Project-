using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Consultion
{
    public class ConsultationMessageDto
    {
        public int Id { get; set; }
        public int ConsultationId { get; set; }
        public int SenderUserId { get; set; }
        public string SenderName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class SendMessageDto
    {
        public string Content { get; set; } = null!;
    }
}
