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
    /// Логика взаимодействия для ПромежуточнаяФормаОтправки.xaml
    /// </summary>
    public partial class ПромежуточнаяФормаОтправки : Window
    {
        int buildIdPublic;
        int userIdPublic;

        public ПромежуточнаяФормаОтправки(int buildId, int userId)
        {
            InitializeComponent();
            buildIdPublic = buildId;
            userIdPublic = userId;
        }

        private void inputTextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)//отправка
        {
            int inputUserId = Convert.ToInt32(inputTextBlock.Text);

            // Проверка: нельзя отправлять себе
            if (inputUserId == userIdPublic)
            {
                MessageBox.Show("Нельзя отправлять конфигурации себе!");
                return;
            }

            var checkUser = DB.Context.Пользователь.FirstOrDefault(z => z.Код_пользователя == inputUserId);
            if (checkUser != null)
            {
                // Получаем информацию о конфигурации одним запросом
                var configuration = DB.Context.Конфигурация
                    .FirstOrDefault(z => z.Код_конфигурации == buildIdPublic);

                if (configuration == null)
                {
                    MessageBox.Show("Конфигурация не найдена!");
                    return;
                }

                // Проверка: нельзя отправлять автору его же конфигурацию
                if (configuration.Код_автора == inputUserId)
                {
                    MessageBox.Show("Нельзя отправлять автору его же конфигурацию!");
                    return;
                }

                // Создаем и сохраняем письмо
                Почта почта = new Почта
                {
                    Код_отправляющего = userIdPublic,
                    Код_получающего = inputUserId,
                    Код_конфигурации = buildIdPublic,
                };

                DB.Context.Почта.Add(почта);
                DB.Context.SaveChanges();
                MessageBox.Show("Конфигурация отправлена");
                this.Close();
            }
            else
            {
                MessageBox.Show("Такого пользователя не существует!");
            }
        }
    }
}
