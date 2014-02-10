using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.DAO
{
    public class MagazineDatabaseContext : DataContext 
    {
         public static string DBConnectionString = "Data Source=isostore:/magazinedb.sdf;";
        public MagazineDatabaseContext(string connectionString)
            : base(connectionString)
        { }

        public MagazineDatabaseContext()
            : base(DBConnectionString)
        { }

        public Table<Group> Groups;
        public Table<Category> Categories;
        public Table<FavouritedPost> FavouritedPosts;
        public Table<VersionTable> Version;

    }
}
