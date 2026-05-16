using EMD.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMD.EF
{
    public class EMDDbContext : DbContext
    {
        public EMDDbContext(DbContextOptions<EMDDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //-----------------------------------------------------
            // Departments
            //-----------------------------------------------------
            modelBuilder.Entity<Department>(entity =>
            {

                entity.Property(e => e.DepartmentName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .IsRequired();

                entity.HasIndex(e => e.DepartmentName)
                      .IsUnique()
                      .HasDatabaseName("UQ_Departments_DepartmentName");
            });

            //-----------------------------------------------------
            // Designations
            //-----------------------------------------------------
            modelBuilder.Entity<Designation>(entity =>
            {
                entity.Property(e => e.DesignationName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.DepartmentId)
                      .IsRequired();

                entity.HasIndex(e => e.DesignationName)
                      .IsUnique()
                      .HasDatabaseName("UQ_Designations_Name");

                entity.HasIndex(e => e.DepartmentId)
                      .HasDatabaseName("IX_Designations_DepartmentId");

                //-------------------------------------------------
                // Relationships
                //-------------------------------------------------
                entity.HasOne(d => d.Department)
                      .WithMany(p => p.Designations)
                      .HasForeignKey(d => d.DepartmentId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_Designations_Departments");
            });

            //-----------------------------------------------------
            // Employees
            //-----------------------------------------------------
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.EmployeeName)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.PhoneNo)
                      .HasMaxLength(10)
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.PasswordHash)
                      .HasMaxLength(500)
                      .IsRequired();

                entity.Property(e => e.Role)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.City)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.State)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Pincode)
                      .HasMaxLength(6)
                      .IsRequired();

                entity.Property(e => e.Address)
                      .HasMaxLength(2000)
                      .IsRequired();

                entity.Property(e => e.AltPhoneNo)
                      .HasMaxLength(10);

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime");

                //-------------------------------------------------
                // Unique Indexes
                //-------------------------------------------------
                entity.HasIndex(e => e.Email)
                      .IsUnique()
                      .HasDatabaseName("UQ_Employees_Email");

                entity.HasIndex(e => e.PhoneNo)
                      .IsUnique()
                      .HasDatabaseName("UQ_Employees_PhoneNo");

                //-------------------------------------------------
                // Normal Indexes
                //-------------------------------------------------
                entity.HasIndex(e => e.DesignationId)
                      .HasDatabaseName("IX_Employees_DesignationId");

                entity.HasIndex(e => e.Email)
                      .HasDatabaseName("IX_Employees_Email");

                entity.HasIndex(e => e.PhoneNo)
                      .HasDatabaseName("IX_Employees_PhoneNo");

                //-------------------------------------------------
                // Relationships
                //-------------------------------------------------
                entity.HasOne(d => d.Designation)
                      .WithMany(p => p.Employees)
                      .HasForeignKey(d => d.DesignationId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_Employees_Designations");
            });

            //-----------------------------------------------------
            // Sessions
            //-----------------------------------------------------
            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Sessions");

                entity.HasKey(e => e.SessionId)
                      .HasName("PK_Sessions");

                entity.Property(e => e.SessionId)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                      .IsRequired();

                entity.Property(e => e.RefreshTokenHash)
                      .HasMaxLength(500)
                      .IsRequired();

                entity.Property(e => e.RefreshTokenExpiresAt)
                      .HasColumnType("datetime")
                      .IsRequired();

                entity.Property(e => e.RefreshTokenRevokedAt)
                      .HasColumnType("datetime");

                //-------------------------------------------------
                // Indexes
                //-------------------------------------------------
                entity.HasIndex(e => e.UserId)
                      .HasDatabaseName("IX_Sessions_UserId");

                entity.HasIndex(e => e.RefreshTokenExpiresAt)
                      .HasDatabaseName("IX_Sessions_RefreshTokenExpiresAt");

                //-------------------------------------------------
                // Relationships
                //-------------------------------------------------
                entity.HasOne(d => d.Employee)
                      .WithMany(p => p.Sessions)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_Sessions_Employees");
            });
        }
    }
}
