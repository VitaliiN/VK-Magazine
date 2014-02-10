using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.DAO
{
    [Table]
    public class FavouritedPost
    {

        [Column(IsPrimaryKey = true,
        IsDbGenerated = true,
        DbType = "INT NOT NULL Identity",
        CanBeNull = false,
        AutoSync = AutoSync.OnInsert)]
        public long PostId { get; set; }

        [Column]
        public string GroupName { get; set; }

        [Column]
        public string GroupIcon { get; set; }

        [Column]
        public string Text { get; set; }

        [Column]
        public DateTime Created { get; set; }

        [Column]
        public string Photos { get; set; }
        [Column]
        public long VkPostId { get; set; }
        [Column]
        public long VKGroupId { get; set; }
    }
}
