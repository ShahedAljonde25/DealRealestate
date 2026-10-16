using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealRealState.Models
{
    public class PropertyImage
    {
        public int ImageID { get; set; }

        public int PropertyID { get; set; }

        public string ImageURL { get; set; }

        public string Description { get; set; }

        public int OrderNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
