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
    /// Логика взаимодействия для Избранные_Конфигурации.xaml
    /// </summary>
    public partial class Избранные_Конфигурации : Window
    {
        int userId;

        public Избранные_Конфигурации(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            loadBuilds();
            loadFio(userId);


        }
        private void loadBuilds()
        {
            try
            {
                // Получаем данные из БД
                var buildsData = DB.Context.Избранное.Where(x => x.Код_пользователя == userId).Select(p => p.Конфигурация).ToList();

                // Используем класс BuildLoader для загрузки
                var buildsWithData = Load.LoadBuilds(buildsData);
                BuildsListBox.ItemsSource = buildsWithData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки конфигураций: {ex.Message}");
            }
        }
        public void loadFio(int userId)
        {
            var userData = DB.Context.Пользователь.FirstOrDefault(x => x.Код_пользователя == userId);
            fioLabel.Content = userData.Фамилия + " " + userData.Имя + " " + userData.Отчество;
            userIdLabel.Content = "Ваш ID: " + userId;
        }
        private void Button_Click(object sender, RoutedEventArgs e) //отправка
        {
            if (BuildsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите конфигурацию для отправки!");
                return;
            }

            // Получаем выбранный элемент
            var selectedItem = BuildsListBox.SelectedItem;

            // Используем reflection для безопасного получения свойства
            var property = selectedItem.GetType().GetProperty("Код_конфигурации");
            if (property == null)
            {
                MessageBox.Show("Ошибка: не найдено свойство Код_конфигурации");
                return;
            }

            int buildId = (int)property.GetValue(selectedItem);
            new ПромежуточнаяФормаОтправки(buildId, userId).ShowDialog();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Мои_Конфигурации(userId);
            newForm.Show();
            this.Close();
        }
    }
}
