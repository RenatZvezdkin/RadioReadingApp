using System;
using System.Collections.Generic;
using CrossplatformRadioApp.MainDatabase;
using Microsoft.EntityFrameworkCore;

namespace CrossplatformRadioApp.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<Recordediqdatum> RecordedIQData { get; set; }

    public virtual DbSet<SavedFile> SavedFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("database=RadioRecords;server=localhost;port=3306;user=zvezdkinrenat;password=120570", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.33-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Record>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("records");

            entity.Property(e => e.DateOfRecord).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(45);
        });

        modelBuilder.Entity<Recordediqdatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("recordediqdata");

            entity.HasIndex(e => e.RecordId, "Record_idx");

            entity.Property(e => e.DatetimeOfRecord).HasMaxLength(6);

            entity.HasOne(d => d.Record).WithMany(p => p.Recordediqdata)
                .HasForeignKey(d => d.RecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Record");
        });

        modelBuilder.Entity<SavedFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("savedfiles");

            entity.Property(e => e.DateOfSaving).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(100);
            entity.Property(e => e.Format).HasMaxLength(45);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
