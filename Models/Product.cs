using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product : Entity
    {
        public /*virtual*/ string Name { get; set; } = string.Empty;
        public /*virtual*/ float Price { get; set; }
        public /*virtual*/ Order? Order { get; set; }

        //odpowiednik IsRowVersion w konfiguracji
        //[Timestamp]
        public byte[] Timestamp { get; set; }
    }
}