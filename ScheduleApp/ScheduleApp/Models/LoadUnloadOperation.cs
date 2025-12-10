using System;
using System.Collections.Generic;

namespace ScheduleApp.Models;

public partial class LoadUnloadOperation
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
