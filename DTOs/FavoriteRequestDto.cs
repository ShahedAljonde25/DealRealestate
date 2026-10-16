using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public enum FavoriteMode
    {
        Remove = 0,
        Add = 1
    }
     public class FavoriteRequestDto
        {
            public int PropertyID { get; set; }
            public FavoriteMode Mode { get; set; }
        }
    }

