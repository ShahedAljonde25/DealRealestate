using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealRealState.Models
{
    public class Favorite
    {
        public int FavoriteID { get; set; }
        public int UserID { get; set; }
        public int PropertyID { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PropertyType { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal AreaSQM { get; set; }
    }
}