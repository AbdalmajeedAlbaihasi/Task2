using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
    {
        //DTOs
        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public ICollection<Product> Products { get; set; } = new List<Product>();
        }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }

            public int CategoryId { get; set; }
            public Category? Category { get; set; }

            public bool IsDeleted { get; set; }

            [Timestamp]
            public byte[] RowVersion { get; set; } = [];
        }



        //كود الEF  اضطريت  اخذه من الذكاء الاصطناعي
        public class AppDbContext : DbContext
        {
            public DbSet<Product> Products => Set<Product>();
            public DbSet<Category> Categories => Set<Category>();

            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);


                modelBuilder.Entity<Product>()
                    .HasIndex(p => p.Name);


                modelBuilder.Entity<Product>()
                    .HasQueryFilter(p => !p.IsDeleted);
            }
        }

        public class ProductDataAccess
        {
            private readonly AppDbContext _context;

            public ProductDataAccess(AppDbContext context)
            {
                _context = context;
            }


            public async Task<List<Product>> GetProductsAsync(int pageNumber, int pageSize)
            {
                return await _context.Products
                    .AsNoTracking() 
                    .Include(p => p.Category) 
                    .OrderBy(p => p.Id)
                    .ToListAsync();
            }




            public async Task<Product?> GetByIdAsync(int id)
            {
                return await _context.Products.FindAsync(id);
            }

            public async Task UpdateProductAsync(Product product, byte[] originalRowVersion)
            {
                _context.Entry(product).Property(p => p.RowVersion).OriginalValue = originalRowVersion;

                await _context.SaveChangesAsync();
            }
        }
    
}