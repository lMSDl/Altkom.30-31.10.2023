using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

            /*builder.Property(x => x.OrderType).HasConversion(
                x => x.ToString(),
                x => Enum.Parse<OrderTypes>(x)
                );*/

            //builder.Property(x => x.OrderType).HasConversion(new EnumToStringConverter<OrderTypes>());
            builder.Property(x => x.OrderType).HasConversion<string>();
            /*builder.Property(x => x.Parameters).HasConversion(
                x => string.Join(',',Enum.GetValues<Parameters>().Where(xx => x.HasFlag(xx)).Select(xx => xx.ToString())),
                x => x.Split(',', StringSplitOptions.None).Select(xx => Enum.Parse<Parameters>(xx)).Aggregate((a, b) => a | b)
                );*/
        }
    }
}
