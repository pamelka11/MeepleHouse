using System.Linq;
using System.Windows;

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
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (login == "" || password == "")
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            var user = db.Users.FirstOrDefault(u =>
                u.Username == login &&
                u.Password == password &&
                u.IsDeleted == false);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            if (user.IsBlocked == true)
            {
                MessageBox.Show("Ваш аккаунт заблокирован администратором");
                return;
            }

            Session.CurrentUser = user;
            Session.CurrentAdmin = null;
            Session.CurrentWorker = null;
            Session.CurrentRole = "User";

            MainWindow1 mainWindow = new MainWindow1();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            CloseOtherWindows(mainWindow);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            Application.Current.MainWindow = registerWindow;
            registerWindow.Show();

            this.Close();
        }

        private void WorkerLogin_Click(object sender, RoutedEventArgs e)
        {
            WorkerLoginWindow workerLoginWindow = new WorkerLoginWindow();
            Application.Current.MainWindow = workerLoginWindow;
            workerLoginWindow.Show();

            this.Close();
        }

        private void CloseOtherWindows(Window activeWindow)
        {
            foreach (Window window in Application.Current.Windows.Cast<Window>().ToList())
            {
                if (window != activeWindow)
                {
                    window.Close();
                }
            }
        }
    }
}