using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MeepleHouse
{
    public partial class LoginWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;

            if (login == "" || password == "")
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            var user = db.Users.FirstOrDefault(u =>
                u.Username == login &&
                u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            Session.CurrentUser = user;

            MessageBox.Show("Вы успешно авторизовались");

            MainWindow1 window = new MainWindow1();
            window.Show();

            // закрываем главное окно (неавторизованное)
            foreach (Window w in Application.Current.Windows)
            {
                if (w is MainWindow)
                {
                    w.Close();
                    break;
                }
            }

            this.Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow window = new RegisterWindow();
            window.ShowDialog();
        }
    }
}