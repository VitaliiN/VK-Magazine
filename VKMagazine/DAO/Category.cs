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
    public class Category
    {
        private EntitySet<Group> _groups;
        private int _id;
        [Column(IsPrimaryKey = true,
           IsDbGenerated = true,
           DbType = "INT NOT NULL IDENTITY",
           CanBeNull = false,
           AutoSync = AutoSync.OnInsert)]
        public int CategoryId
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
        public string VkUserId { get; set; }
        [Column]
        public string CategoryName { get; set; }
        [Column]
        public string IconPath { get; set; }
        [Column]
        public bool isCollection { get; set; }
        [Column]
        public bool isUserGroups { get; set; }
        [Column]
        public bool IsSelected { get; set; }

        [Association(Storage = "_groups",
     ThisKey = "CategoryId", OtherKey = "CategoryId")]
        public EntitySet<Group> Groups
        {
            set
            {
                _groups.Assign(value);
            }
            get
            {
                return _groups;
            }
        }

        public Category()
        {
            _groups = new EntitySet<Group>(AttachToDo, DetachToDo);
        }
        private void AttachToDo(Group toDo)
        {
            //NotifyPropertyChanging("ToDoItem");
            toDo.Category = this;
        }
        private void DetachToDo(Group toDo)
        {
            //NotifyPropertyChanging("ToDoItem");
            toDo.Category = null;
        }


    }
}
