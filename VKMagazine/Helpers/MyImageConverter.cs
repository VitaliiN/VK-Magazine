using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VKMagazine.Helpers
{
    public class MyImageConverter : IValueConverter 
    {
        private static IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //try
            //{
                //ImageSource image = (ImageSource)value;
            if (value == null)
                return value;
            if (value is ImageSource)
                    return value;
            //}
           // catch (InvalidCastException)
            else
            {
                string path = (string)value;
                if ((path.Length > 9) && (path.ToLower().Substring(0, 9).Equals("isostore:")))
                {
                    if (!isoStorage.FileExists(path.Substring(9)))
                        return new BitmapImage(new Uri("/VKMagazine;component/Icons/spash_logo.png", UriKind.Relative));
                    using (var sourceFile = isoStorage.OpenFile(path.Substring(9), FileMode.Open, FileAccess.Read))
                    {
                        BitmapImage image = new BitmapImage();
                        image.CreateOptions = BitmapCreateOptions.DelayCreation;
                        try
                        {
                            image.SetSource(sourceFile);
                        }
                        catch
                        {
                            return new BitmapImage(new Uri("/VKMagazine;component/Icons/spash_logo.png", UriKind.Relative));
                        }
                        return image;

                    }
                }
                else
                {
                    BitmapImage image = new BitmapImage();
                    //image.CreateOptions = BitmapCreateOptions.None;
                    image.UriSource = new Uri(path);
                    //image.BeginInit();
                    //BitmapImage image = new BitmapImage(new Uri(path));
                    return image;
                }
            }



            //string path = value as string;

            //if (string.IsNullOrEmpty(path))
            //    return null;

            //if ((path.Length > 9) && (path.ToLower().Substring(0, 9).Equals("isostore:")))
            //{
            //    if (!isoStorage.FileExists(path.Substring(9)))
            //        return new BitmapImage(new Uri("/VKMagazine;component/Icons/spash_logo.png", UriKind.Relative));
            //    using (var sourceFile = isoStorage.OpenFile(path.Substring(9), FileMode.Open, FileAccess.Read))
            //    {
            //        BitmapImage image = new BitmapImage();
            //        try
            //        {
            //            image.SetSource(sourceFile);
            //        }
            //        catch
            //        {
            //            return new BitmapImage(new Uri("/VKMagazine;component/Icons/spash_logo.png", UriKind.Relative));
            //        }
            //        return image;
            //    }
            //}
            //else
            //{
            //    BitmapImage image = new BitmapImage();
            //    //image.CreateOptions = BitmapCreateOptions.None;
            //    image.UriSource = new Uri(path);
            //    //image.BeginInit();
            //    //BitmapImage image = new BitmapImage(new Uri(path));
            //    return image;
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
