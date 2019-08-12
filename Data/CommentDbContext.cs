using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tayko.co.Models;

namespace Tayko.co.Data
{
    public class CommentDbContext : DbContext, IDataProtectionKeyContext
    {
        public CommentDbContext(DbContextOptions options) : base(options)
        {
        }

        // Define Comments dbSet
        public DbSet<CommentModel> Comments { get; set; }
        
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}