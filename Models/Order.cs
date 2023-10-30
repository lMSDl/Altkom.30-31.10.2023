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
        public /*virtual*/ string? Name { get; set; }
        public /*virtual*/ ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();
    }
}
