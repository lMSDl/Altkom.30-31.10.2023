using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Flags]
    public enum Parameters
    {
        Cash = 1 << 1,
        Card = 1 << 2,
        Shipping = 1 << 3,
        Gift = 1 << 4
    }
}
