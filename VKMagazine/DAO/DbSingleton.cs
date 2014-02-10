using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.DAO
{
    public class DbSingleton
    {
        private static MagazineDatabaseContext instance;
        private static object syncRoot = new Object();
        private DbSingleton() { }

        public static MagazineDatabaseContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MagazineDatabaseContext();
                        }
                    }
                }
                return instance;
            }
        }


       
        public static void Refresher()
        {
            instance.Dispose();
            instance = new MagazineDatabaseContext();
        }

    }
}
