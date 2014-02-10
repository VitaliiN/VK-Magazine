using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.Response
{
    public class GetuserInfoResponse
    {
        public List<UserResponse> response { get; set; }

        public GetuserInfoResponse()
        {
            response = new List<UserResponse>();
        }
    }

    public class UserResponse
    {
        public long uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string photo_100 { get; set; }

    }
}
