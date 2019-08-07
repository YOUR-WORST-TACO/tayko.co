using Microsoft.EntityFrameworkCore;
using Tayko.co.Models;

namespace Tayko.co.Data
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions options) : base(options)
        {
        }

        // Define Comments dbSet
        public DbSet<CommentModel> Comments { get; set; }
    }
}