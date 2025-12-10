using System;
using System.Collections.Generic;

namespace ScheduleApp.Models;

public partial class ProductsHasOrder
{
    public int ProductsId { get; set; }

    public int OrdersId { get; set; }

    public short Amount { get; set; }

    public virtual Order Orders { get; set; } = null!;

    public virtual Product Products { get; set; } = null!;
}
