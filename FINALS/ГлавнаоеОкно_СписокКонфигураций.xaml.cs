using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ГлавнаоеОкно_СписокКонфигураций.xaml
    /// </summary>
    public partial class ГлавнаоеОкно_СписокКонфигураций : Window
    {     
        int userId;
        public ГлавнаоеОкно_СписокКонфигураций(int userId)
        {
            InitializeComponent();
            LoadPublicBuilds();
            this.userId = userId;
        }
        public ГлавнаоеОкно_СписокКонфигураций()
        {
            InitializeComponent();
            LoadPublicBuilds();

            send.Visibility = Visibility.Hidden;
            myBuild.Visibility = Visibility.Hidden;
            favorite.Visibility = Visibility.Hidden;
        }
        private void LoadPublicBuilds()
        {
            try
            {
                // Получаем данные из БД
                var buildsData = DB.Context.Конфигурация
                    .Where(x => x.Публикация
                    == true)
                    .ToList();

                // Используем класс BuildLoader для загрузки
                var buildsWithData = Load.LoadBuilds(buildsData);
                BuildsListBox.ItemsSource = buildsWithData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки конфигураций: {ex.Message}");
            }
        }     
        private void MyBuild_Click(object sender, RoutedEventArgs e)
        {
            new Мои_Конфигурации(userId).Show();
            this.Close();
        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
        public void send_Click(object sender, RoutedEventArgs e)
        {
            if (BuildsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите конфигурацию для отправки!");
                return;
            }
            var selectedItem = BuildsListBox.SelectedItem;
            var property = selectedItem.GetType().GetProperty("Код_конфигурации");
            if (property == null)
            {
                MessageBox.Show("Ошибка: не найдено свойство Код_конфигурации");
                return;
            }
            int buildId = (int)property.GetValue(selectedItem);
            
            new ПромежуточнаяФормаОтправки(buildId, userId).Show();
        }
        private void favorite_Click(object sender, RoutedEventArgs e)
        {
            if (BuildsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите конфигурацию для добавления в избранное!");
                return;
            }
            var selectedItem = BuildsListBox.SelectedItem;
            var property = selectedItem.GetType().GetProperty("Код_конфигурации");
            if (property == null)
            {
                MessageBox.Show("Ошибка: не найдено свойство Код_конфигурации");
                return;
            }
            int buildId = (int)property.GetValue(selectedItem);
            var existsAvtor = DB.Context.Конфигурация.Where(z=>z.Код_конфигурации == buildId).Any(z => z.Код_автора == userId);
            if (existsAvtor)
            {
                MessageBox.Show("Нельзя добавлять свои конфигурации в избранное!");
                return;
            }
            var exists = DB.Context.Избранное.Any(z => z.Код_пользователя == userId && z.Код_конфигурации == buildId);
            if (exists)
            {
                MessageBox.Show("Ошибка: эта конфигурация уже в избранном!");
                return;
            }

            Избранное избранное = new Избранное
            {
                Код_пользователя = userId,
                Код_конфигурации = buildId
            };

            DB.Context.Избранное.Add(избранное);
            DB.Context.SaveChanges();
            MessageBox.Show($"Конфигурация добавлена в избранное!");
        }
    }
}