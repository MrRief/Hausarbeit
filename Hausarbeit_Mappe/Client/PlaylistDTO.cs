using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class PlaylistDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<LiedDTO> Lieder { get; set; }
    }
}
