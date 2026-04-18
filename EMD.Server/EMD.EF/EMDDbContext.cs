using EMD.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMD.EF
{
    public class EMDDbContext : DbContext
    {
        public EMDDbContext(DbContextOptions<EMDDbContext> options):base(options)
        {
            
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
