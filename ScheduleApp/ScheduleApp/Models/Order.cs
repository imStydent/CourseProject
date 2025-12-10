using System;
using System.Collections.Generic;

namespace ScheduleApp.Models;

public partial class Order
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public TimeSpan LoadTime { get; set; }

    public TimeSpan UnloadTime { get; set; }

    public virtual ICollection<ProductsHasOrder> ProductsHasOrders { get; set; } = new List<ProductsHasOrder>();

    public virtual ICollection<LoadUnloadOperation> LoadUnloadOpertations { get; set; } = new List<LoadUnloadOperation>();
}
