using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Converters
{
    internal class ObfuscationConverter : ValueConverter<string, string>
    {
        public ObfuscationConverter() : base(
            x => Convert.ToBase64String(Encoding.Default.GetBytes(x)), 
            x => Encoding.Default.GetString(Convert.FromBase64String(x)))
        {

        }
    }
}
