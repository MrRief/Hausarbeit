using System;
using System.Collections.Generic;

namespace _StreamingServer;

public partial class Playlist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? NutzerId { get; set; }

    public virtual Nutzer? Nutzer { get; set; }

    public virtual ICollection<Lieder> Lieds { get; set; } = new List<Lieder>();
}
