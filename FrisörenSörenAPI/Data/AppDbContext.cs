using FrisörenSörenModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FrisörenSörenAPI.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().HasData(new Company
            {
                CompanyId = 1,
                CompanyName = "Frisören Sören",
                Email = "Soren@hotmail.com",
                Password = "Password123"
                

            });

            modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                CustomerId = 1,
                CustomerName = "Sven Svensson",
                Email = "Sven.Svensson@hotmail.com",
                Phone = "1111111111",
                Password = "Password123"
            },
            new Customer
            {
                CustomerId = 2,
                CustomerName = "Anna Eriksson",
                Email = "Anna.Eriksson@hotmail.com",
                Phone = "2222222222",
                Password = "Password123"
            },
            new Customer
            {
                CustomerId = 3,
                CustomerName = "Lars Carlsson",
                Email = "Lars.Carlsson@hotmail.com",
                Phone = "3333333333",
                Password = "Password123"
            },
            new Customer
            {
                CustomerId = 4,
                CustomerName = "Sara Nilsson",
                Email = "Sara.Nilsson@hotmail.com",
                Phone = "4444444444",
                Password = "Password123"
            },
            new Customer
            {
                CustomerId = 5,
                CustomerName = "Thomas Andersson",
                Email = "Thomas.Andersson@hotmail.com",
                Phone = "5555555555",
                Password = "Password123"
            }
            );

            modelBuilder.Entity<Booking>().HasData(
            new Booking
            {
                StartTime = new DateTime(2024, 6, 1, 13, 0, 0),
                EndTime = new DateTime(2024, 6, 1, 14, 0, 0),
                CustomerId = 1,
                BookingId = 1,
                CompanyId = 1
            },
            new Booking
            {
                StartTime = new DateTime(2024, 6, 1, 14, 0, 0),
                EndTime = new DateTime(2024, 6, 1, 15, 0, 0),
                CustomerId = 2,
                BookingId = 2,
                CompanyId = 1
            },
            new Booking
            {
                StartTime = new DateTime(2024, 6, 1, 9, 0, 0),
                EndTime = new DateTime(2024, 6, 1, 10, 0, 0),
                CustomerId = 3,
                BookingId = 3,
                CompanyId = 1
            },
            new Booking
            {
                StartTime = new DateTime(2024, 6, 1, 10, 0, 0),
                EndTime = new DateTime(2024, 6, 1, 11, 0, 0),
                CustomerId = 4,
                BookingId = 4,
                CompanyId = 1
            },
            new Booking
            {
                StartTime = new DateTime(2024, 6, 1, 12, 0, 0),
                EndTime = new DateTime(2024, 6, 1, 13, 0, 0),
                CustomerId = 5,
                BookingId = 5,
                CompanyId = 1
            }
            );
        }
    }
}
