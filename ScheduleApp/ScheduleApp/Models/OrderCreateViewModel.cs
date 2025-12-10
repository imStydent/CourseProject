using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Models
{
    public class OrderCreateViewModel
    {
        public string Name { get; set; } = null!;

        public TimeSpan LoadTime { get; set; }

        public TimeSpan UnloadTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today.AddDays(1);

        public List<Product> Products { get; set; }

        public List<AddProduct> AddProducts { get; set; }

        public OrderCreateViewModel()
        {
            AddProducts = new List<AddProduct>(); // Инициализация коллекции
            Products = new List<Product>(); // Инициализация коллекции
        }
    }
}
