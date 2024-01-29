using System;
using Identity.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<User>()
                .Property(u => u.Gender)
                .HasConversion<string>();

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            var manager = new Role { Id = new Guid("796138b4-cc1c-4660-a6c9-18250cb20672"), Name = "Business manager" };
            var customerService = new Role { Id = new Guid("ae25cb3d-b983-44eb-a233-430684f37330"), Name = "Customer services" };
            var complianceOfiicer = new Role { Id = new Guid("92053e15-bb56-4170-9198-6f944d3b008a"), Name = "Compliance officer" };

            builder.Entity<Role>()
                .HasData(new Role { Id = new Guid("fb52ee2d-0e69-47c5-856d-3a367c66378d"), Name = "Admin", }, new Role { Id = new Guid("b4b063e8-3dd1-4c46-815c-d2a0ea9cac0c"), Name = "User" }, manager, customerService, complianceOfiicer);

            builder.Entity<Permission>().HasData(
                new Permission { Id = new Guid("b8ff867b-1979-44fb-86da-7ccfce4d052d"), Name = "Transaction View" },
                new Permission { Id = new Guid("22d17c90-7658-4120-a087-c1f30797f9f5"), Name = "Transaction Edit" },
                new Permission { Id = new Guid("b1f5a953-a3c0-40b7-9842-3ed29084aa27"), Name = "Transaction Delete" },
                new Permission { Id = new Guid("65ef9fb5-0a0f-4805-a712-66983b9ca33c"), Name = "Staff View" },
                new Permission { Id = new Guid("6989d9bc-b5df-4088-8e8e-e452bba3c098"), Name = "Staff Edit" },
                new Permission { Id = new Guid("d8fa6d21-4a0b-40c3-a4e7-0ab7a4559c49"), Name = "Staff Delete" },
                new Permission { Id = new Guid("f7ddd8d8-a96e-42f9-92f7-b92a8061aca0"), Name = "Staff Create" },
                new Permission { Id = new Guid("a2c9fcaa-560b-4bd4-88d7-727ec722e47e"), Name = "Customer View" },
                new Permission { Id = new Guid("0a31d551-2a1c-4941-aeec-a698b6678747"), Name = "Customer Edit" },
                new Permission { Id = new Guid("43eac5fd-7509-4cd5-83aa-17083ccbc577"), Name = "Customer Delete" },
                new Permission { Id = new Guid("b129d9aa-f6a9-4ed4-a50e-f3651a8633dc"), Name = "Location View" },
                new Permission { Id = new Guid("345011ad-de0a-492d-8706-06f877a6e202"), Name = "Location Edit" },
                new Permission { Id = new Guid("7b476fc8-e1cd-49bb-986d-db9bab72e003"), Name = "Location Create" },
                new Permission { Id = new Guid("04eb9f6c-570c-4f75-a1c3-505beb7583ff"), Name = "Location Delete" },
                new Permission { Id = new Guid("7de1e463-3e67-4ad0-94b8-09873030dc95"), Name = "ExchangeRate View" },
                new Permission { Id = new Guid("aad5c740-0b0b-485e-a488-5128a95d0a55"), Name = "ExchangeRate Edit" },
                new Permission { Id = new Guid("4e5e641b-d54d-4c8f-a0d1-23bb23bbb6d9"), Name = "ExchangeRate Create" },
                new Permission
                {
                    Id = new Guid("c15dcb6f-a335-4518-afb4-33c83e94bc2f"),
                    Name = "ExchangeRate Delete"
                }
            );
        }
    }
}