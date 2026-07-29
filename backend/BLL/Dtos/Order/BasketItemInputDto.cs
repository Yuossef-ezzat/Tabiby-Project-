using System.ComponentModel.DataAnnotations;


namespace BLL.Dtos.Order
{
    public class BasketItemInputDto
    {
        [Required]
        public int MedicationId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
