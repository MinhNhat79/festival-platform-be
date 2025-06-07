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
        public DbSet<SchoolAccount> SchoolAccounts { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
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

            // Account - Role (Many-to-One)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Account - SchoolAccount (One-to-Many)
            modelBuilder.Entity<SchoolAccount>()
                .HasOne(sa => sa.Account)
                .WithMany(a => a.SchoolAccounts)
                .HasForeignKey(sa => sa.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // School - SchoolAccount (One-to-Many)
            modelBuilder.Entity<SchoolAccount>()
                .HasOne(sa => sa.School)
                .WithMany(s => s.SchoolAccounts)
                .HasForeignKey(sa => sa.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booth - Group (Many-to-One)
            modelBuilder.Entity<Booth>()
                .HasOne<StudentGroup>()
                .WithMany(g => g.Booths)
                .HasForeignKey(b => b.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booth - Festival (Many-to-One)
            modelBuilder.Entity<Booth>()
                .HasOne<FestivalModel>()
                .WithMany(f => f.Booths)
                .HasForeignKey(b => b.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booth - Location (Many-to-One) -- locationId refers to MapLocation
            modelBuilder.Entity<Booth>()
                .HasOne<MapLocation>()
                .WithMany()
                .HasForeignKey(b => b.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatMessage - ChatSession (Many-to-One)
            modelBuilder.Entity<ChatMessage>()
                .HasOne<ChatSession>()
                .WithMany(cs => cs.Messages)
                .HasForeignKey(cm => cm.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ChatSession - Account (Many-to-One)
            modelBuilder.Entity<ChatSession>()
                .HasOne<Account>()
                .WithMany(a => a.ChatSessions)
                .HasForeignKey(cs => cs.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // FestivalIngredient - Festival (Many-to-One)
            modelBuilder.Entity<FestivalIngredient>()
                .HasOne<FestivalModel>()
                .WithMany(f => f.FestivalIngredients)
                .HasForeignKey(fi => fi.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // FestivalIngredient - Ingredient (Many-to-One)
            modelBuilder.Entity<FestivalIngredient>()
                .HasOne<Ingredient>()
                .WithMany()
                .HasForeignKey(fi => fi.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            // FestivalMap - Festival (Many-to-One)
            modelBuilder.Entity<FestivalMap>()
                .HasOne<FestivalModel>()
                .WithMany(f => f.FestivalMaps)
                .HasForeignKey(fm => fm.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // MapLocation - FestivalMap (Many-to-One)
            modelBuilder.Entity<MapLocation>()
                .HasOne<FestivalMap>()
                .WithMany(fm => fm.Locations)
                .HasForeignKey(ml => ml.MapId)
                .OnDelete(DeleteBehavior.Cascade);

            // FestivalSchool - Festival (Many-to-One)
            modelBuilder.Entity<FestivalSchool>()
                .HasOne<FestivalModel>()
                .WithMany(f => f.FestivalSchools)
                .HasForeignKey(fs => fs.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // FestivalSchool - School (Many-to-One)
            modelBuilder.Entity<FestivalSchool>()
                .HasOne<School>()
                .WithMany(s => s.FestivalSchools)
                .HasForeignKey(fs => fs.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            // GroupMember - StudentGroup (Many-to-One)
            modelBuilder.Entity<GroupMember>()
                .HasOne<StudentGroup>()
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // GroupMember - Account (Many-to-One)
            modelBuilder.Entity<GroupMember>()
                .HasOne<Account>()
                .WithMany(a => a.GroupMemberships)
                .HasForeignKey(gm => gm.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ingredient - Supplier (Many-to-One)
            modelBuilder.Entity<Ingredient>()
                .HasOne<Supplier>()
                .WithMany(s => s.Ingredients)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItem - Booth (Many-to-One)
            modelBuilder.Entity<MenuItem>()
                .HasOne<Booth>()
                .WithMany(b => b.MenuItems)
                .HasForeignKey(mi => mi.BoothId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItemIngredient - MenuItem (Many-to-One)
            modelBuilder.Entity<MenuItemIngredient>()
                .HasOne<MenuItem>()
                .WithMany(mi => mi.MenuItemIngredients)
                .HasForeignKey(mii => mii.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItemIngredient - Ingredient (Many-to-One)
            modelBuilder.Entity<MenuItemIngredient>()
                .HasOne<Ingredient>()
                .WithMany()
                .HasForeignKey(mii => mii.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Minigame - Booth (Many-to-One)
            modelBuilder.Entity<Minigame>()
                .HasOne<Booth>()
                .WithMany(b => b.Minigames)
                .HasForeignKey(mg => mg.BoothId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question - Minigame (Many-to-One)
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Game)
                .WithMany(mg => mg.Questions)
                .HasForeignKey(q => q.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order - Account (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne<Account>()
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order - Booth (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne<Booth>()
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BoothId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem - Order (Many-to-One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc .Cascade nếu thật sự cần

            // OrderItem - MenuItem (Many-to-One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
            // PointsTransaction - Account (Many-to-One)
            modelBuilder.Entity<PointsTransaction>()
                .HasOne<Account>()
                .WithMany(a => a.PointsTransactions)
                .HasForeignKey(pt => pt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);



            // PointsTransaction - Minigame (optional)
            modelBuilder.Entity<PointsTransaction>()
                .HasOne<Minigame>()
                .WithMany()
                .HasForeignKey(pt => pt.GameId)
                .OnDelete(DeleteBehavior.SetNull);

            // Image - Booth (optional)


            // Image - Festival (optional)


            // Image - Minigame (optional)


            // Image - MenuItem (optional)
            modelBuilder.Entity<Image>()
                .HasOne(i => i.MenuItem)
                .WithMany(mi => mi.Images)
                .HasForeignKey(i => i.MenuItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // Image - Supplier (optional)


            // Image - School (optional)


            // Image - Account (optional)


            // GroupMember - SchoolAccount (optional)        

            // AccountPoints - Account (One-to-One or One-to-Many)
            modelBuilder.Entity<Account>()
                 .HasOne(a => a.AccountPoints)
                 .WithOne(ap => ap.Account)
                 .HasForeignKey<AccountPoints>(ap => ap.AccountId)
                 .OnDelete(DeleteBehavior.Cascade); // hoặc Restrict tuỳ logic

            // Unique and composite keys or indexes can be added here if needed
            // e.g., modelBuilder.Entity<GroupMember>().HasKey(gm => new { gm.AccountId, gm.GroupId });

            // Configuring DateTime properties or enums can be done here if needed
        }
    }
}
