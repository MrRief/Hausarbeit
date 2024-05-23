using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _StreamingServer;

public partial class MusicStreamDbContext : DbContext
{
    public MusicStreamDbContext()
    {
    }

    public MusicStreamDbContext(DbContextOptions<MusicStreamDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Künstler> Künstlers { get; set; }

    public virtual DbSet<Lieder> Lieders { get; set; }

    public virtual DbSet<Nutzer> Nutzers { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MusicStreamDB;Integrated Security=SSPI");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Künstler>(entity =>
        {
            entity.HasKey(e => e.KünstlerId).HasName("PK__Künstler__73E3ABBAA45297DE");

            entity.ToTable("Künstler");

            entity.Property(e => e.KünstlerId).HasColumnName("KünstlerID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Lieder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lieder__3214EC07D6723C80");

            entity.ToTable("Lieder");

            entity.Property(e => e.Genre).HasMaxLength(20);
            entity.Property(e => e.KünstlerId).HasColumnName("KünstlerID");
            entity.Property(e => e.Titel).HasMaxLength(40);

            entity.HasOne(d => d.Künstler).WithMany(p => p.Lieders)
                .HasForeignKey(d => d.KünstlerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KünstlerID");
        });

        modelBuilder.Entity<Nutzer>(entity =>
        {
            entity.HasKey(e => e.NutzerId).HasName("PK__tmp_ms_x__C0768D15A6BF5E78");

            entity.ToTable("Nutzer");

            entity.Property(e => e.Email).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Passwort).HasMaxLength(20);
            entity.Property(e => e.Vorname).HasMaxLength(20);

            entity.HasMany(d => d.Lieds).WithMany(p => p.Nutzers)
                .UsingEntity<Dictionary<string, object>>(
                    "NutzerFavoriten",
                    r => r.HasOne<Lieder>().WithMany()
                        .HasForeignKey("LiedId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NutzerFavoriten_LiedID"),
                    l => l.HasOne<Nutzer>().WithMany()
                        .HasForeignKey("NutzerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NutzerFavoriten_NutzerID"),
                    j =>
                    {
                        j.HasKey("NutzerId", "LiedId");
                        j.ToTable("NutzerFavoriten");
                        j.IndexerProperty<int>("NutzerId").HasColumnName("NutzerID");
                        j.IndexerProperty<int>("LiedId").HasColumnName("LiedID");
                    });
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Playlist__3214EC0761F2969E");

            entity.ToTable("Playlist");

            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.NutzerId).HasColumnName("NutzerID");

            entity.HasOne(d => d.Nutzer).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.NutzerId)
                .HasConstraintName("FK_NutzerID");

            entity.HasMany(d => d.Lieds).WithMany(p => p.Playlists)
                .UsingEntity<Dictionary<string, object>>(
                    "LiederInPlaylist",
                    r => r.HasOne<Lieder>().WithMany()
                        .HasForeignKey("LiedId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_LiederInPlaylists_LiedID"),
                    l => l.HasOne<Playlist>().WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_LiederInPlaylists_PlaylistID"),
                    j =>
                    {
                        j.HasKey("PlaylistId", "LiedId");
                        j.ToTable("LiederInPlaylists");
                        j.IndexerProperty<int>("PlaylistId").HasColumnName("PlaylistID");
                        j.IndexerProperty<int>("LiedId").HasColumnName("LiedID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
