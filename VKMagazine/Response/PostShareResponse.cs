using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.Response
{
    public  class PostShareResponse
    {
        public ShareResponse response { get; set; }
    }

    public class ShareResponse 
    {
        public int success {get;set;}
    }
}
