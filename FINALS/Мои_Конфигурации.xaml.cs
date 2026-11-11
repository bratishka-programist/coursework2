using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
using static FINALS.ГлавнаоеОкно_СписокКонфигураций;

namespace FINALS
{
    /// <summary>
    /// Логика взаимодействия для Мои_Конфигурации.xaml
    /// </summary>
    public partial class Мои_Конфигурации : Window
    {
        int userId;
        public Мои_Конфигурации(int userId)
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
                var buildsData = DB.Context.Конфигурация.Where(x => x.Код_автора == userId).ToList();

                // Используем класс BuildLoader для загрузки
                var buildsWithData = Load.LoadBuilds(buildsData);
                BuildsListBox.ItemsSource = buildsWithData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки конфигураций: {ex.Message}");
            }
        }
        private void CREATE_Click(object sender, RoutedEventArgs e)
        {
            new Создание_Конфигурации(userId).Show();
            this.Close();
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
        private void publishButton_Click(object sender, RoutedEventArgs e)
        {
            if (BuildsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите конфигурацию для публикации!");
                return;
            }

            var selectedItem = BuildsListBox.SelectedItem;
            var buildIdProperty = selectedItem.GetType().GetProperty("Код_конфигурации");
            int buildId = (int)buildIdProperty.GetValue(selectedItem);
            var build = DB.Context.Конфигурация.FirstOrDefault(z => z.Код_конфигурации == buildId);

            if (build.Публикация == true)
            {
                MessageBox.Show("Конфигурация уже опубликована");
            }
            else
            {
                build.Публикация = true;
                try
                {
                    DB.Context.SaveChanges();
                    MessageBox.Show("Конфигурация успешно опубликована!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) //почта
        {
            new ОтправленныеКонфигурации(userId).Show();
            this.Close();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) //главное окно конфигураций
        {
            new ГлавнаоеОкно_СписокКонфигураций(userId).Show();
            this.Close();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e) //избранное
        {
            new Избранные_Конфигурации(userId).Show();
            this.Close();
        }
        private void Button_Click_3(object sender, RoutedEventArgs e) //редактирование
        {
            if (BuildsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите конфигурацию для редактирования!");
                return;
            }

            var selectedItem = BuildsListBox.SelectedItem;
            var buildIdProperty = selectedItem.GetType().GetProperty("Код_конфигурации");
            int buildId = (int)buildIdProperty.GetValue(selectedItem);
            Конфигурация build = DB.Context.Конфигурация.FirstOrDefault(z => z.Код_конфигурации == buildId);



            MessageBoxResult result = MessageBox.Show(
                            "Вы уверены, что хотите отредактировать выбранную конфигурацию?",
                            "Подтверждение",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;
            new Редактирование_Конфигурации(build).ShowDialog();
            loadBuilds(userId);
        }
    }
}