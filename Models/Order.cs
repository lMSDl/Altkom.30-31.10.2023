using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order : Entity
    {
        private DateTime dateTime;

        //nazwy backfielda, które zgodnie z konwencją pasują do nazwy kolumny
        //private string? name;
        //private string? _name;
        //private string? m_name;

        private string? zuzia;

        //odpowiednik IsConcurencyToken w konfiguracji
        //[ConcurrencyCheck]
        public /*virtual*/ DateTime DateTime
        {
            get => dateTime;
            set
            {
                dateTime = value;
                OnPropertyChanged();
            }
        }
        public /*virtual*/ string? Name
        {
            get => zuzia;
            set
            {
                zuzia = value;
            }
        }
        public int Number { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        //public string Description => $"{Name}: {DateTime}";
        public string? Description { get; }

        public OrderTypes OrderType { get; set; }

        public Parameters Parameters { get; set; }

        public Point? DeliveryPoint { get; set; }
    }
}
