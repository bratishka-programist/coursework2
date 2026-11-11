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
    /// Логика взаимодействия для РедактированиеПользователей.xaml
    /// </summary>
    public partial class РедактированиеПользователей : Window
    {
        public РедактированиеПользователей()
        {
            InitializeComponent();
            loadUsersData();
        }

        private void loadUsersData()
        {
            var usersData = DB.Context.Пользователь.Where(z=>z.Код_роли == 1).ToList();
            UsersDG.ItemsSource = usersData;
        }

        private void UsersDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)// кнопка редактирования
        {
            try
            {
                DB.Context.SaveChanges();
                loadUsersData(); // обновление
                MessageBox.Show("Изменения сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
