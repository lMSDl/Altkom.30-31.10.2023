using DAL.Conventions;
using DAL.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Models;
using Pluralize.NET.Core;
using System.Security.Cryptography.X509Certificates;

namespace DAL
{
    public class Context : DbContext
    {

        public Context(DbContextOptions options) : base(options) {
        }


        public static Func<Context, DateTime, DateTime, IEnumerable<Order>> GetOrdersByDateRange { get; } =
            EF.CompileQuery((Context context, DateTime from, DateTime to) => 
                context.Set<Order>()
                .AsNoTracking()
            .Include(x => x.Products)
            .Where(x => x.DateTime >= from)
            .Where(x => x.DateTime <= to));


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);


            modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetProperties())
                //.Where(x => x.ClrType == typeof(int))
                .Where(x => x.Name == "Key")
                .ToList()
                .ForEach(x =>
                {
                    x.IsNullable = false;
                    x.DeclaringEntityType.SetPrimaryKey(x);
                });

            /*modelBuilder.Model.GetEntityTypes()
                .ToList()
                .ForEach(x =>
                {
                    x.SetTableName(new Pluralizer().Pluralize(x.GetDefaultTableName()));
                });*/

            modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetProperties())
                .Where(x => x.ClrType == typeof(string))
                .Where(x => x.PropertyInfo?.CanWrite ?? false)
                .ToList()
                .ForEach(x => x.SetValueConverter(new ObfuscationConverter()));

            modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferProperty); //domyślna wartość: PropertyAccessMode.PreferField


            modelBuilder.HasSequence<int>("OrderNumber")
                .StartsAt(100)
                .HasMin(0)
                .HasMax(900)
                .IncrementsBy(333)
                .IsCyclic();
        }


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            //configurationBuilder.Properties<DateTime>().HavePrecision(5);
            configurationBuilder.Conventions.Add(_ => new DateTimePrecision());
            //configurationBuilder.Conventions.Add(_ => new PluralizeTableNames());

            //configurationBuilder.Conventions.Remove(typeof(KeyDiscoveryConvention));

            configurationBuilder.Properties<string>().HaveMaxLength(128);
        }


        public bool RandomFail { get; set; }
        public override int SaveChanges()
        {
            if (RandomFail && new Random((int)DateTime.Now.Ticks).Next(1, 50) == 1)
                throw new Exception();


            return base.SaveChanges();
        }

    }
}