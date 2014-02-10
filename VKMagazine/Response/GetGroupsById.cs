using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.Response
{
    public class GetGroupsById
    {
        public List<GroupResponse> response { get; set; }
    }
    public class GroupResponse
    {
        public string name { get; set; }
        public string photo { get; set; }
        public int gid { get; set; }
        public bool is_closed { get; set; }
        public bool is_member { get; set; }
     
        
    }
}
