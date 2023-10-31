using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configurations
{
    internal class ProductConfiguration : EntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Order).WithMany(x => x.Products);

            builder/*.Property(x => x.Timestamp)*/
                //konfiguracja shadow property    
                .Property<byte[]>("Timestamp")
                    .IsRowVersion();

            builder.HasQueryFilter(x => !EF.Property<bool>(x.Order, "IsDeleted"));

            builder.HasOne(x => x.Detail).WithOne().HasForeignKey<ProductDetails>(x => x.Id);
        }
    }
}
