using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.DAO
{
    [Table]
    public class Group
    {
        private EntityRef<Category> _category;
        

        [Column(IsPrimaryKey = true,
          IsDbGenerated = true,
          DbType = "INT NOT NULL Identity",
          CanBeNull = false,
          AutoSync = AutoSync.OnInsert)]
        public int GroupId { get; set; }
        [Column]
        internal int CategoryId;
        [Column]
        public int VkId { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public string Image { get; set; }
        [Column]
        public bool isSelected { get; set; }
        [Column]
        public bool isUserGroups { get; set; }
        [Column]
        public bool isFinded { get; set; }
        [Column]
        public string vkUserId { get; set; }

        [Association(Storage = "_category",
    ThisKey = "CategoryId", OtherKey = "CategoryId",IsForeignKey=true)]
        public Category Category
        {
            set
            {
                _category.Entity = value;
                if (value != null)
                {
                    CategoryId = value.CategoryId;
                }
            }
            get
            {
                return _category.Entity;
            }
        }
    }
}
