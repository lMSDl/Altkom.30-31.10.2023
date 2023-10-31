using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Conventions
{
    internal class PluralizeTableNames : IModelFinalizingConvention

    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            modelBuilder.Metadata.GetEntityTypes()
                .ToList()
                .ForEach(x =>
                {
                    x.SetTableName(new Pluralizer().Pluralize(x.GetDefaultTableName()));
                });

        }
    }
}
