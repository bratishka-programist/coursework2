using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FINALS
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return null;

            string imagePath = value.ToString();

            // Если путь уже относительный
            if (imagePath.StartsWith("img\\") || imagePath.StartsWith("/img/") || imagePath.StartsWith("img/"))
                return imagePath;

            // Извлекаем только имя файла из абсолютного пути
            string fileName = Path.GetFileName(imagePath);

            // Определяем папку на основе типа изображения
            string folder = "configs";
            if (imagePath.Contains("weapon")) folder = "weapon";
            else if (imagePath.Contains("gadget")) folder = "gadget";
            else if (imagePath.Contains("spec")) folder = "spec";
            else if (imagePath.Contains("class")) folder = "class";

            return $"/img/{folder}/{fileName}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
