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
    /// Логика взаимодействия для ОтправленныеКонфигурации.xaml
    /// </summary>
    public partial class ОтправленныеКонфигурации : Window
    {
        int userId;
        public ОтправленныеКонфигурации(int userId)
        {
            InitializeComponent();
            loadFio(userId);
            loadBuilds(userId);
            this.userId = userId;
        }

        public void loadFio(int userId)
        {
            var userData = DB.Context.Пользователь.FirstOrDefault(x => x.Код_пользователя == userId);
            fioLabel.Content = userData.Фамилия + " " + userData.Имя + " " + userData.Отчество;
            userIdLabel.Content = "Ваш ID: " + userId;
        }

        public void loadBuilds(int userId)
        {
            try
            {
                // Получаем данные из БД
                var buildsData = DB.Context.Почта
                    .Where(p => p.Код_получающего == userId) // Конфигурации, отправленные этим пользователем
                    .Select(p => p.Конфигурация) // Получаем связанные конфигурации
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

        private void send_Click(object sender, RoutedEventArgs e)
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

        private void Button_Click(object sender, RoutedEventArgs e) //назад 
        {
            var newForm = new Мои_Конфигурации(userId);
            newForm.Show();
            this.Close();
        }
    }
}