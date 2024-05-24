using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class UpdateNutzerDTO
    {
        public int NutzerId { get; set; }
        public string Name { get; set; }
        public string Vorname { get; set; }
        public string Email { get; set; }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
