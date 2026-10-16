using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string Name { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string role { get; set; }

        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? VerificationTokenExpires { get; set; }
    }
}