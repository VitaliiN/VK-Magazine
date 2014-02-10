using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.DAO
{
    [Table]
    public class VersionTable
    {
        private int _id;
        [Column(IsPrimaryKey = true,
           IsDbGenerated = true,
           DbType = "INT NOT NULL IDENTITY",
           CanBeNull = false,
           AutoSync = AutoSync.OnInsert)]
        public int VersionId
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [Column]
        public int Version { get; set; }
    }
}
