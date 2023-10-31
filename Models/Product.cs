using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Models
{
    public class Product : Entity
    {
        private ILazyLoader _lazyLoader;
        private Order? _order;

        public Product()
        {
        }

        public Product(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }



        public /*virtual*/ string Name { get; set; } = string.Empty;
        public /*virtual*/ float Price { get; set; }
        //ShadowProperty dla referencji
        //public int OrderId { get; set; }
        //public virtual Order? Order { get; set; }
        public virtual Order? Order
        {
            get
            {
                if (_order == null)
                {
                    try
                    {
                        _lazyLoader.Load(this, ref _order);
                    }
                    catch
                    {
                        _order = null;
                    }
                }

                return  _order;
            }
            set => _order = value;
        }

        public ProductDetails? Detail { get; set; }

        //odpowiednik IsRowVersion w konfiguracji
        //[Timestamp]
        //public byte[] Timestamp { get; set; } // zastąpione przez shadow property
    }
}