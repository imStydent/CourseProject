namespace ScheduleApp.Models
{
    public class AddProduct
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Kind { get; set; } = null!;

        public sbyte PiecesPerBox { get; set; }

        public short Amount { get; set; }
    }
}
