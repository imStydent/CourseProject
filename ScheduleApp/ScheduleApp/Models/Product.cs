using System;
using System.Collections.Generic;

namespace ScheduleApp.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Kind { get; set; } = null!;

    public sbyte PiecesPerBox { get; set; }

    public virtual ICollection<ProductsHasOrder> ProductsHasOrders { get; set; } = new List<ProductsHasOrder>();
}
