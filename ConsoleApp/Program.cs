

using DAL;
using Microsoft.EntityFrameworkCore;

var contextOptions = new DbContextOptionsBuilder<Context>()
                        .UseSqlServer(@"Server=(local)\SQLEXPRESS;Database=EFCore;Integrated Security=true;TrustServerCertificate=True") //Encrypt=True
                        .Options;

using var context = new Context(contextOptions);

context.Database.EnsureDeleted();
context.Database.EnsureCreated();