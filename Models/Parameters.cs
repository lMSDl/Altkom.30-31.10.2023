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
        Cash = 1 << 0,
        Card = 1 << 1,
        Shipping = 1 << 2,
        Gift = 1 << 3
    }
}
