using Microsoft.EntityFrameworkCore;

namespace Lab2.DataAccess
{
    public class BasketDbContext : DbContext
    {
        public DbSet<Delivery> Delivery { get; set; }
        public DbSet<Bread> Bread { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public BasketDbContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Git_hub\LB2_DataAccess\Lab2.DataAccess\BasketDb.mdf;Integrated Security=True");
            }
        }

    }
}
