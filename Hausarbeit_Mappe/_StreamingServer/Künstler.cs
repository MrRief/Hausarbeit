using System;
using System.Collections.Generic;

namespace _StreamingServer;

public partial class Künstler
{
    public int KünstlerId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Lieder> Lieders { get; set; } = new List<Lieder>();
}
