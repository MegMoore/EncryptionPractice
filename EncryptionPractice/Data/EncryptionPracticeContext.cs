using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EncryptionPractice.Models;

namespace EncryptionPractice.Data
{
    public class EncryptionPracticeContext : DbContext
    {
        public EncryptionPracticeContext (DbContextOptions<EncryptionPracticeContext> options)
            : base(options)
        {
        }

        public DbSet<EncryptionPractice.Models.Kid> Kids { get; set; } = default!;
    }
}
