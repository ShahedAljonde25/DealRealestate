using DealRealState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Property
    {
        public int PropertyID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PropertyType { get; set; }

        public string Location { get; set; }

        public string City { get; set; }

        public int Bedrooms { get; set; }

        public int Bathrooms { get; set; }

        public decimal AreaSQM { get; set; }

        public int Floor { get; set; }

        public int TotalFloors { get; set; }

        public int YearBuilt { get; set; }
        public string ImageURL { get; set; }

        public bool HasGarage { get; set; }

        public bool HasPool { get; set; }

        public bool HasGarden { get; set; }

        public bool IsFurnished { get; set; }


        public List<PropertyPanorama> Panoramas { get; set; } = new List<PropertyPanorama>();


    }
}
