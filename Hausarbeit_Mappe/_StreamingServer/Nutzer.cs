using System;
using System.Collections.Generic;

namespace _StreamingServer;

public partial class Nutzer
{
    public int NutzerId { get; set; }

    public string Name { get; set; } = null!;

    public string Vorname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwort { get; set; } = null!;

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
