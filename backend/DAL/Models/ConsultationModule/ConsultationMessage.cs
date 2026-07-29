using DAL.Models.Consultation;
using DAL.Models.Users;
using DAL.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ConsultationMessage : BaseEntity
{

    public int ConsultationId { get; set; }

    public int SenderUserId { get; set; }

    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    
    [ForeignKey(nameof(ConsultationId))]
        
    public virtual Consultation Consultation { get; set; } = null!;

    [ForeignKey(nameof(SenderUserId))]
    public virtual ApplicationUser Sender { get; set; } = null!;
}