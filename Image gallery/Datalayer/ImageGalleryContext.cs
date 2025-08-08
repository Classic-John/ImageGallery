using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Core
{
    public class ImageGalleryContext : DbContext
    {
        public DbSet<TheImage> TheImages { get; set; }
        public DbSet<User> Users { get; set; }
        public ImageGalleryContext (DbContextOptions<ImageGalleryContext> options) : base(options) { }
    }
    public static class DbSetExtensions
    {
        public static DbContext GetDbContext<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
        {
            var infrastructure = dbSet as IInfrastructure<IServiceProvider>;
            var serviceProvider = infrastructure.Instance;
            var currentDbContext = serviceProvider.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            return currentDbContext.Context;
        }
    }
}
