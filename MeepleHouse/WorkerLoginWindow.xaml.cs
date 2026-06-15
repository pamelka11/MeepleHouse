using System.Linq;
using System.Windows;

namespace MeepleHouse
{
    public partial class WorkerLoginWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public WorkerLoginWindow()
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

            // Проверка администратора
            var admin = db.Admins.FirstOrDefault(a =>
                a.Username == login &&
                a.Password == password &&
                a.IsDeleted == false);

            if (admin != null)
            {
                Session.CurrentAdmin = admin;
                Session.CurrentWorker = null;
                Session.CurrentUser = null;
                Session.CurrentRole = "Admin";

                AdminWindow adminWindow = new AdminWindow();
                Application.Current.MainWindow = adminWindow;
                adminWindow.Show();

                CloseOtherWindows(adminWindow);
                return;
            }

            // Проверка работника
            var worker = db.Workers.FirstOrDefault(w =>
                w.Username == login &&
                w.Password == password &&
                w.IsDeleted == false);

            if (worker != null)
            {
                Session.CurrentWorker = worker;
                Session.CurrentAdmin = null;
                Session.CurrentUser = null;
                Session.CurrentRole = "Worker";

                WorkerWindow workerWindow = new WorkerWindow();
                Application.Current.MainWindow = workerWindow;
                workerWindow.Show();

                CloseOtherWindows(workerWindow);
                return;
            }

            MessageBox.Show("Неверный логин или пароль");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();

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