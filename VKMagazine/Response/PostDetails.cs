using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.Response
{
    public  class PostDetails
    {
        public long date { get; set; }
        public string text { get; set; }
        public long from_id { get; set; }
        public long id { get; set; }
        public List<Attachment> attachments { get; set; }
        public PostDetails()
        {
            attachments = new List<Attachment>();
        }
    }
    public class Attachment
    {
        public string type { get; set; }
        public Photo photo { get; set; }
    }
    public class Photo
    {
        public string src_big { get; set; }
        public string src_small { get; set; }
        public double width { get; set; }
        public double height { get; set; }
    }
}
