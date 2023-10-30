using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order : Entity
    {
        public DateTime DateTime { get; set; }
        public ICollection<Product> Procuts { get; set; } = new List<Product>();
    }
}
