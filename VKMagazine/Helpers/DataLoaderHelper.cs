using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKMagazine.DAO;

namespace VKMagazine.Helpers
{
    public static class DataLoaderHelper
    {
        public const int CURRENT_DATABASE_VERSION = 4;

        private static string[] Categories = { "СМИ", "Hi-Tech", "Образ Жизни", "", "", "" };
        private static int[] smiIds = { 15755094, 20035339, 11982368, 29534144, 24199209, 11546691, 17842936, 20169232, 25106701, 24136539, 15548215, 23482909, 25232578, 28261334 };
        private static int[] hiTechIds = { 33393308, 38362552, 57526182, 20629724, 42025435, 52558528, 12310748, 34300760, 17733403, 40546932, 23553134 };
        private static int[] lifeStileIds = { 27797026, 34685858, 30576005, 23228214, 34350462, 54423481, 49439086, 33134232, 40567146, 24098496 };
        private static int[] manIds = { 25397178, 32573577, 32194500, 34137285, 25251165, 24419507, 44652965, 37739473, 38630769, 36188379, 47765919, 35034571, 37862023, 34529402, 39144813 };
        private static int[] womanIds = { 29219144, 56369320, 33477779, 32477579, 28627911, 45064245, 26970810, 34486229, 34308709, 55074079, 51579638, 34757875, 34981365, 39747283, 30145618, 35484478, 41682596, 42379103, 38328058, 31051063 };
        private static int[] musicIds = { 24154167, 54879485, 21292404, 22991532, 19552394, 164810, 41109472, 53997646, 58406727, 35257924, 35707452, 49583292, 38233726, 41445500, 41437811, 30022666, 30532220, 31076033, 26217154, 47544817 };
        private static int[] movieIds = { 108468, 51252121, 22554373, 48125645, 29716454, 35595350, 21257898, 57138590, 5285618, 51708436 };
        private static int[] smileIds = { 12382740, 29246653, 30277672, 31836774, 30179569, 45608667, 460389, 23537466, 36775802, 26858816, 23148107, 31480508 };
        //categoryList.add(new Category(9,new Integer[]{31545235,38290762,35748704,35143797,46066853,23951686,34203973,40973193,21090314,49680000,10639516},R.drawable.plus18));
        private static int[] sportIds = { 23471538, 30360552, 35486195, 16054118, 27922438, 31428243, 12637219, 38909088, 25539801, 35555559, 755816, 39445664, 46813166 };
        private static int[] auto_and_motoIds = { 35807044, 27744747, 23783750, 30665521, 23400027, 36635754, 29058619, 42996405, 39236729, 44256610, 31128331, 11766299 };
        private static int[] iskIds ={29302425,32131490,25813425,23626127,31963425,31920990,34580489,38666219,36184135,31461843,35807199,25817269,45770903,56118322,26307864,43688579,25421850,
                43772432,31805219,35915449,27060808,2002,37608158};
        private static int[] goroskopIds ={23830580,30318830,30783475,27243668,26170853,28075993,26609852,29874238,30891977,26567649,23485910,
                26813657,27842893,30860272};
        private static int[] animalsIds = { 28301494, 33621085, 32015300, 34273778, 35406511, 33709813, 29357458, 23300841, 36202478 };
        private static int[] kulinarIds = { 32194285, 52026163, 48946342, 51068271, 46117626, 42092461, 43879004, 32509740, 28565318, 41554252, 39009769, 32231484, 34451036, 33521375 };
        private static int[] modaIds = { 38976531, 32127188, 28129783, 31041444, 28623496, 30207899, 24396213, 48634303, 23319307, 24483645, 34010064, 43606005, 36859750 };
        private static int[] travelIdsIds = { 27246052, 35238813, 47161293, 33440105, 21314891, 41053835, 34964358, 31516466, 33383239, 39034715, 26127512, 38044058, 28277850, 40062539 };
        private static int[] businesIdsIds = { 23616160, 43251403, 42706552, 34483558, 28556858, 6386, 50243804, 30559917, 34168005, 40020304, 29348258, 29233217, 46603834, 44053412 };
       // private static Dictionary<string, int[]> Cat = new Dictionary<string, int[]>().Add("СМИ", smiIds);
        public static void LoadDataToDabase()
        {
            MagazineDatabaseContext db = DbSingleton.Instance;
            if (false == db.DatabaseExists())
            {
                CreateDataBase(db);
            }
            else 
            {
                VersionTable version = db.Version.FirstOrDefault();
                if (version == null || version.Version < CURRENT_DATABASE_VERSION)
                {
                  
                    db.DeleteDatabase();
                    DbSingleton.Refresher();
                    db = DbSingleton.Instance;
                    //bool res = DbSingleton.Instance.DatabaseExists();
                    //db.SubmitChanges();
                    CreateDataBase(db);
                }
            }

        }

        private static void CreateDataBase(MagazineDatabaseContext db)
        {
            db.CreateDatabase();
            db.Version.InsertOnSubmit(new VersionTable() { Version = CURRENT_DATABASE_VERSION });
            Category smiCat = new Category()
            {
                CategoryName = "СМИ",
                IconPath = "Icons/smi.png",
            };
            Category hitech = new Category()
            {
                CategoryName = "Hi-Tech",
                IconPath = "/Icons/tech.png"
            };
            Category lifestyle = new Category()
            {
                CategoryName = "Образ Жизни",
                IconPath = "/Icons/lifestyle.png"
            };
            Category men = new Category()
            {
                CategoryName = "Мужчинам",
                IconPath = "/Icons/man.png"
            };
            Category women = new Category()
            {
                CategoryName = "Женщинам",
                IconPath = "/Icons/woman.png"
            };
            Category music = new Category()
            {
                CategoryName = "Музыка",
                IconPath = "/Icons/music.png"
            };
            Category kino = new Category()
            {
                CategoryName = "Кино/ТВ",
                IconPath = "/Icons/movie.png"
            };
            Category smile = new Category()
            {
                CategoryName = "Юмор",
                IconPath = "/Icons/smile.png"
            };
            Category sport = new Category()
            {
                CategoryName = "Спорт",
                IconPath = "/Icons/sport.png"
            };

            Category auto = new Category()
            {
                CategoryName = "Авто и мото",
                IconPath = "/Icons/auto_and_moto.png"
            };
            Category art = new Category()
            {
                CategoryName = "Искусство",
                IconPath = "/Icons/isk.png"
            };
            Category goroskop = new Category()
            {
                CategoryName = "Гороскоп",
                IconPath = "/Icons/goroskop.png"
            };

            Category Animals = new Category()
            {
                CategoryName = "Животные",
                IconPath = "/Icons/animals.png"
            };
            Category coocking = new Category()
            {
                CategoryName = "Кулинария",
                IconPath = "/Icons/kulinar.png"
            };
            Category fashion = new Category()
            {
                CategoryName = "Мода",
                IconPath = "/Icons/moda.png"
            };
            Category travel = new Category()
            {
                CategoryName = "Птушествия",
                IconPath = "/Icons/travel.png"
            };
            Category bussines = new Category()
            {
                CategoryName = "Бизнес и финансы",
                IconPath = "/Icons/busines.png"
            };
            db.Categories.InsertOnSubmit(smiCat);
            db.Categories.InsertOnSubmit(hitech);
            db.Categories.InsertOnSubmit(lifestyle);
            db.Categories.InsertOnSubmit(men);
            db.Categories.InsertOnSubmit(women);
            db.Categories.InsertOnSubmit(music);
            db.Categories.InsertOnSubmit(kino);
            db.Categories.InsertOnSubmit(smile);
            db.Categories.InsertOnSubmit(sport);
            db.Categories.InsertOnSubmit(auto);
            db.Categories.InsertOnSubmit(art);
            db.Categories.InsertOnSubmit(goroskop);
            db.Categories.InsertOnSubmit(Animals);
            db.Categories.InsertOnSubmit(coocking);
            db.Categories.InsertOnSubmit(fashion);
            db.Categories.InsertOnSubmit(travel);
            db.Categories.InsertOnSubmit(bussines);
            db.SubmitChanges();
            AddGroupsToCategory(smiCat, smiIds);
            AddGroupsToCategory(hitech, hiTechIds);
            AddGroupsToCategory(lifestyle, lifeStileIds);
            AddGroupsToCategory(men, manIds);
            AddGroupsToCategory(women, womanIds);
            AddGroupsToCategory(music, musicIds);
            AddGroupsToCategory(kino, movieIds);
            AddGroupsToCategory(smile, smileIds);
            AddGroupsToCategory(sport, sportIds);
            AddGroupsToCategory(auto, auto_and_motoIds);
            AddGroupsToCategory(art, iskIds);
            AddGroupsToCategory(goroskop, goroskopIds);
            AddGroupsToCategory(Animals, animalsIds);
            AddGroupsToCategory(coocking, kulinarIds);
            AddGroupsToCategory(fashion, modaIds);
            AddGroupsToCategory(travel, travelIdsIds);
            AddGroupsToCategory(bussines, businesIdsIds);
            //foreach (int id in smiIds.ToList())
            //{
            //    Group g = new Group() { VkId = id,Category=smiCat };
            //    smiCat.Groups.Add(g);
            //}
            //foreach (int id in hiTechIds.ToList())
            //{
            //    Group g = new Group() { VkId = id, Category = hitech };
            //    hitech.Groups.Add(g);
            //}

            //db.Categories.InsertOnSubmit(smiCat);
            db.SubmitChanges();
        }

        private static void AddGroupsToCategory(Category category, int[] ids)
        {
            foreach (int id in ids.ToList())
            {
                Group g = new Group() { VkId = id, Category = category };
                category.Groups.Add(g);
            }
        }
    }
}
