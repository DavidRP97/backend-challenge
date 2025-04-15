namespace Domain.Entities
{
    public class MotorcycleNotification : BaseMongoId
    {
        public string Plate { get; set; } = "";
        public int Year { get; set; }
        public string Message { get; set; } = "Is 2024!";
    }
}
