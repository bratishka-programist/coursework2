using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FINALS
{
    /// <summary>
    /// Логика взаимодействия для Промежуточная_Форма_Админа.xaml
    /// </summary>
    public partial class Промежуточная_Форма_Админа : Window
    {
        public Промежуточная_Форма_Админа()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)//редактирование пользователей
        {
            new РедактированиеПользователей().ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)//редактирование конфигураций
        {
            new ВсеКонфигурации().ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)//назад
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}