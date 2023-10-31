using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ProductDetails : Entity
    {
        public float Weight { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
