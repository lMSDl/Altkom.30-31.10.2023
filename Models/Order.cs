using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order : Entity
    {
        public virtual DateTime DateTime { get; set; }
        public virtual string? Name { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();
    }
}
