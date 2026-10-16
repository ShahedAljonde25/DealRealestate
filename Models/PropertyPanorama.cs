using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealRealState.Models
{
    public class PropertyPanorama
    {
        public int PanoramaID { get; set; }

        public int PropertyID { get; set; }

        public string RoomName { get; set; }

        public string PanoramaURL { get; set; }

        public string Description { get; set; }

        public int OrderNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
