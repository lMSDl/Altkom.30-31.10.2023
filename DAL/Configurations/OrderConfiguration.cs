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
    internal class OrderConfiguration : EntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.DateTime).IsConcurrencyToken();

            //builder.Property(x => x.Description).HasComputedColumnSql("[Name] + ': ' + Cast([DateTime] as varchar(250))");
            builder.Property<string>("Timer").HasComputedColumnSql("Cast(getdate() as varchar(250))");
            builder.Property(x => x.Description).HasComputedColumnSql("[Name] + ': ala ma kota'", stored: true);

            builder.Property(x => x.Name).HasField("zuzia");
            builder.Property(x => x.Number).HasDefaultValueSql("NEXT VALUE FOR OrderNumber");
        }
    }
}
