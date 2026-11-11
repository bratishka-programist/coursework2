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
    /// Логика взаимодействия для ВсеКонфигурации.xaml
    /// </summary>
    public partial class ВсеКонфигурации : Window
    {
        public ВсеКонфигурации()
        {
            InitializeComponent();
            var buildsData = DB.Context.Конфигурация.ToList();
            LoadPublicBuilds();

        }
        private void LoadPublicBuilds()
        {
            try
            {
                // Получаем данные из БД
                var buildsData = DB.Context.Конфигурация.ToList();

                // Используем класс BuildLoader для загрузки
                var buildsWithData = Load.LoadBuilds(buildsData);
                BuildsListBox.ItemsSource = buildsWithData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки конфигураций: {ex.Message}");
            }
        }
        

        private void redactButton_Click(object sender, RoutedEventArgs e)
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
            new Редактирование_Конфигурации(build).Show();
            this.Close();
        }

        private void SafeDeleteConfiguration(int buildId)
        {
            
                    try
                    {
                        var резервы = DB.Context.Конфигурация_резерв
                            .Where(cr => cr.Код_конфигурации == buildId)
                            .ToList();

                        foreach (var резерв in резервы)
                        {
                            DB.Context.Конфигурация_резерв.Remove(резерв);
                        }
                        DB.Context.SaveChanges();

                        var почта = DB.Context.Почта
                        .Where(cr => cr.Код_конфигурации == buildId)
                        .ToList();

                        foreach (var почт in почта)
                        {
                            DB.Context.Почта.Remove(почт);
                        }
                        DB.Context.SaveChanges();

                        var избранное = DB.Context.Избранное
                            .Where(i => i.Код_конфигурации == buildId)
                            .ToList();
                        
                        foreach (var избр in избранное)
                        {
                            DB.Context.Избранное.Remove(избр);
                        }
                        DB.Context.SaveChanges();

                        var configurationToDelete = DB.Context.Конфигурация
                            .FirstOrDefault(z => z.Код_конфигурации == buildId);

                        if (configurationToDelete != null)
                        {
                            DB.Context.Конфигурация.Remove(configurationToDelete);
                        }

                        // Сохраняем все изменения
                        DB.Context.SaveChanges();

                        // Подтверждаем транзакцию
                        

                        MessageBox.Show("Конфигурация успешно удалена!");
                    }
                    catch (Exception ex)
                    {
                        // Откатываем транзакцию при ошибке
                        
                        throw new Exception($"Ошибка при удалении: {ex.Message}");
                    }
              
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (BuildsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите конфигурацию для удаления!");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                            "Вы уверены, что хотите удалить выбранную конфигурацию?",
                            "Подтверждение удаления",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

            if (result != MessageBoxResult.No)
            {
                try
                {
                    var selectedItem = BuildsListBox.SelectedItem;
                    var buildIdProperty = selectedItem.GetType().GetProperty("Код_конфигурации");
                    int buildId = (int)buildIdProperty.GetValue(selectedItem);

                    SafeDeleteConfiguration(buildId);

                    // Перезагружаем данные
                    LoadPublicBuilds();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            new Промежуточная_Форма_Админа().Show();
            this.Close();
        }
    }
}