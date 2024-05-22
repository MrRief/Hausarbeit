﻿using System;
using System.Collections.Generic;

namespace _StreamingServer;

public partial class Lieder
{
    public int Id { get; set; }

    public string Titel { get; set; } = null!;

    public string? Genre { get; set; }

    public int KünstlerId { get; set; }

    public virtual Künstler Künstler { get; set; } = null!;

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
