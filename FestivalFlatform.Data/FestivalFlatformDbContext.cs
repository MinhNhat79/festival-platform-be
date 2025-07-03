using System.Collections.Generic;
using System.Reflection.Emit;
using FestivalFlatform.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FestivalFlatform.Data
{
    using FestivalModel = FestivalFlatform.Data.Models.Festival;
    public class FestivalFlatformDbContext : DbContext
    {
        public FestivalFlatformDbContext(DbContextOptions<FestivalFlatformDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountPoints> AccountPoints { get; set; }
        public DbSet<Booth> Booths { get; set; }
        public DbSet<ChatAttachment> ChatAttachments { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<FestivalModel> Festivals { get; set; }
        public DbSet<FestivalIngredient> FestivalIngredients { get; set; }
        public DbSet<FestivalMap> FestivalMaps { get; set; }
        public DbSet<FestivalSchool> FestivalSchools { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<MapLocation> MapLocations { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemIngredient> MenuItemIngredients { get; set; }
        public DbSet<Minigame> Minigames { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PointsTransaction> PointsTransactions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<School> Schools { get; set; }

        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<FestivalMenu> FestivalMenus { get; set; }
        public DbSet<BoothMenuItem> BoothMenuItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DBLocalShipper"));
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==== Account & Role ====
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==== Account & AccountPoints (1-1) ====

            modelBuilder.Entity<Account>()
                .HasOne(a => a.AccountPoints)
                .WithOne(ap => ap.Account)
                .HasForeignKey<AccountPoints>(ap => ap.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            // ==== Account & SchoolAccounts (1-n) ====
          

            // ==== School & SchoolAccounts (1-n) ====
           

            // ==== Booth - Group (n-1) ====
            modelBuilder.Entity<Booth>()
                .HasOne(b => b.StudentGroup)
                .WithMany(g => g.Booths)
                .HasForeignKey(b => b.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== Booth - Festival (n-1) ====
            modelBuilder.Entity<Booth>()
                .HasOne(b => b.Festival)
                .WithMany(f => f.Booths)
                .HasForeignKey(b => b.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== Booth - MapLocation (n-1) ====
            modelBuilder.Entity<Booth>()
                .HasOne(b => b.Location)
                .WithMany()
                .HasForeignKey(b => b.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==== ChatMessage - ChatSession ====
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ChatSession)
                .WithMany(cs => cs.Messages)
                .HasForeignKey(cm => cm.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== ChatSession - Account ====
            modelBuilder.Entity<ChatSession>()
                .HasOne(cs => cs.Account)
                .WithMany(a => a.ChatSessions)
                .HasForeignKey(cs => cs.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== FestivalIngredient - Festival ====
            modelBuilder.Entity<FestivalIngredient>()
                .HasOne(fi => fi.Festival)
                .WithMany(f => f.FestivalIngredients)
                .HasForeignKey(fi => fi.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== FestivalIngredient - Ingredient ====
            modelBuilder.Entity<FestivalIngredient>()
                .HasOne(fi => fi.Ingredient)
                .WithMany()
                .HasForeignKey(fi => fi.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==== FestivalMap - Festival ====
            modelBuilder.Entity<FestivalMap>()
                .HasOne(fm => fm.Festival)
                .WithMany(f => f.FestivalMaps)
                .HasForeignKey(fm => fm.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== MapLocation - FestivalMap ====
            modelBuilder.Entity<MapLocation>()
                .HasOne(ml => ml.FestivalMap)
                .WithMany(fm => fm.Locations)
                .HasForeignKey(ml => ml.MapId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== FestivalSchool - Festival ====
            modelBuilder.Entity<FestivalSchool>()
                .HasOne(fs => fs.Festival)
                .WithMany(f => f.FestivalSchools)
                .HasForeignKey(fs => fs.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== FestivalSchool - School ====
            modelBuilder.Entity<FestivalSchool>()
                .HasOne(fs => fs.School)
                .WithMany(s => s.FestivalSchools)
                .HasForeignKey(fs => fs.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== GroupMember - Group & Account ====
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.StudentGroup)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Account)
                .WithMany(a => a.GroupMemberships)
                .HasForeignKey(gm => gm.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== Ingredient - Supplier ====
            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Supplier)
                .WithMany(s => s.Ingredients)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== MenuItem - Booth ====
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.FestivalMenu)
                .WithMany(b => b.MenuItems)
                .HasForeignKey(mi => mi.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== MenuItemIngredient - MenuItem & Ingredient ====
            modelBuilder.Entity<MenuItemIngredient>()
                .HasOne(mii => mii.MenuItem)
                .WithMany(mi => mi.MenuItemIngredients)
                .HasForeignKey(mii => mii.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItemIngredient>()
                .HasOne(mii => mii.Ingredient)
                .WithMany()
                .HasForeignKey(mii => mii.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==== Minigame - Booth ====
            modelBuilder.Entity<Minigame>()
                .HasOne(mg => mg.Booth)
                .WithMany(b => b.Minigames)
                .HasForeignKey(mg => mg.BoothId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== Question - Minigame ====
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Game)
                .WithMany(mg => mg.Questions)
                .HasForeignKey(q => q.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== Order - Account & Booth ====
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Account)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Booth)
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BoothId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==== OrderItem - Order & MenuItem ====
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==== PointsTransaction - Account & Minigame ====
            modelBuilder.Entity<PointsTransaction>()
                .HasOne(pt => pt.Account)
                .WithMany(a => a.PointsTransactions)
                .HasForeignKey(pt => pt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PointsTransaction>()
                .HasOne(pt => pt.Minigame)
                .WithMany()
                .HasForeignKey(pt => pt.GameId)
                .OnDelete(DeleteBehavior.SetNull);

            // ==== Image - MenuItem (optional only) ====
            modelBuilder.Entity<Image>()
                .HasOne(i => i.MenuItem)
                .WithMany(mi => mi.Images)
                .HasForeignKey(i => i.MenuItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // ==== BoothMenuItem - Unique Constraint ====
            modelBuilder.Entity<BoothMenuItem>()
                 .HasIndex(b => new { b.BoothId, b.MenuItemId })
                    .IsUnique(); // nếu muốn mỗi Booth chỉ bán 1 loại MenuItem một lần

            modelBuilder.Entity<BoothMenuItem>()
                .HasOne(bmi => bmi.Booth)
                .WithMany(b => b.BoothMenuItems)
                .HasForeignKey(bmi => bmi.BoothId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BoothMenuItem>()
                .HasOne(bmi => bmi.MenuItem)
                .WithMany(mi => mi.BoothMenuItems)
                .HasForeignKey(bmi => bmi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==== Festival - School ====
            modelBuilder.Entity<FestivalModel>()
                .HasOne(f => f.School)
                .WithMany(s => s.Festivals)
                .HasForeignKey(f => f.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==== StudentGroup - Teacher (Account) ====
            modelBuilder.Entity<StudentGroup>()
                .HasOne(sg => sg.Account)
                .WithMany() // hoặc WithMany(a => a.AdvisedGroups) nếu có
                .HasForeignKey(sg => sg.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<School>()
            .HasOne(s => s.Account)
            .WithMany(a => a.Schools)
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Restrict); // hoặc Cascade nếu bạn muốn xoá account xoá luôn school

            modelBuilder.Entity<Image>()
            .HasOne(i => i.MenuItem)
            .WithMany(mi => mi.Images)
            .HasForeignKey(i => i.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Booth)
                .WithMany(b => b.Images)
                .HasForeignKey(i => i.BoothId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Festival)
                .WithMany(f => f.Images)
                .HasForeignKey(i => i.FestivalId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==== Composite or Unique Keys (if needed) ====
            // Ví dụ: GroupMember => composite key
        }
    }
}
