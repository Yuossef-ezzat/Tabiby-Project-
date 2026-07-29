namespace PL.Models.MedicationModels
{
    public class UpdateMedicationModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsAvailable { get; set; }
    }
}
