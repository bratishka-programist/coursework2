using System.Linq;
using System.Windows;
using System.Windows.Input;
using Mail_LIB;


namespace FINALS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GOIN(object sender, RoutedEventArgs e)
        {
            string log = login.Text;
            string pas = paroltext.Text;
            Mail_LIB.ClassLIB classLIB = new ClassLIB();
            // Проверка логина
            var loginCheck = classLIB.check_login(log);
            if (!loginCheck.isValid)
            {
                MessageBox.Show(loginCheck.message);
                return;
            }
            // Проверка пароля
            var passwordCheck = classLIB.check_password(pas);
            if (!passwordCheck.isValid)
            {
                MessageBox.Show(passwordCheck.message);
                return;
            }
            //Если логин и пароль прошли проверку, продолжаем
            var accessRights = DB.Context.Пользователь.FirstOrDefault(x => x.Логин == log);
            if (accessRights == null)
            {
                MessageBox.Show("Не правильный логин!");
                return;
            }
            if (accessRights.Пароль != pas)
            {
                MessageBox.Show("Неуспешная авторизация!");
                return;
            }
            var user = DB.Context.Пользователь.FirstOrDefault(x => x.Логин == log);
            if (user.Код_роли == 1)
            {
                new ГлавнаоеОкно_СписокКонфигураций(user.Код_пользователя).Show();
                this.Close(); 
            }
            else
            {
                new Промежуточная_Форма_Админа().Show();
                this.Close();
            }
        }
        private void CREATEACC(object sender, RoutedEventArgs e)
        {
            Mail_LIB.ClassLIB classLIB = new ClassLIB();
            string fio1 = fio.Text;
            string login = txtlogin.Text;
            string password = pas1.Password;
            string password1 = pas2.Password;
            if (password != password1)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }
            // Проверка логина
            var loginCheck = classLIB.check_login(login);
            if (!loginCheck.isValid)
            {
                MessageBox.Show(loginCheck.message);
                return;
            }
            // Проверка пароля
            var passwordCheck = classLIB.check_password(password);
            if (!passwordCheck.isValid)
            {
                MessageBox.Show(passwordCheck.message);
                return;
            }
            string[] nameParts = fio1.Split(' ');
            string lastName = nameParts[0];   // Фамилия (Иванов)
            string firstName = nameParts[1];  // Имя (Иван)
            string middleName = nameParts[2];
            Пользователь пользователь = new Пользователь
            {
                Фамилия = lastName,
                Имя = firstName,
                Отчество = middleName,
                Логин = login,
                Пароль = password,
                Код_роли = 1
            };
            DB.Context.Пользователь.Add(пользователь);
            DB.Context.SaveChanges();
            MessageBox.Show("Вы успешно зарегестрировались!");
            fio.Text = "";
            txtlogin.Text = "";
            pas1.Password = "";
            pas2.Password = "";
        }
        private void parolavto_PasswordChanged(object sender, RoutedEventArgs e)
        {
            paroltext.Text = parolavto.Password;
        }
        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            parolavto.Visibility = Visibility.Hidden;
        }
        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            parolavto.Visibility = Visibility.Visible;
        }
        private void onlyInspection_Click(object sender, RoutedEventArgs e)
        {
            new ГлавнаоеОкно_СписокКонфигураций().Show();
            this.Close();
        }
    }
}