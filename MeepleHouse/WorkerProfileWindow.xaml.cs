using System.Linq;
using System.Windows;

namespace MeepleHouse
{
    public partial class WorkerProfileWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public WorkerProfileWindow()
        {
            InitializeComponent();
            LoadWorkerData();
        }

        private void LoadWorkerData()
        {
            if (Session.CurrentWorker == null)
            {
                MessageBox.Show("Сотрудник не авторизован");
                Close();
                return;
            }

            int workerId = Session.CurrentWorker.Id;

            var worker = db.Workers.FirstOrDefault(w => w.Id == workerId);

            if (worker == null)
            {
                MessageBox.Show("Сотрудник не найден");
                Close();
                return;
            }

            FullNameBox.Text = worker.FullName;
            EmailBox.Text = worker.Email;
            PhoneBox.Text = worker.Phone;
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editProfileWindow = new EditProfileWindow();
            editProfileWindow.Owner = this;
            editProfileWindow.ShowDialog();

            LoadWorkerData();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentAdmin = null;
            Session.CurrentWorker = null;
            Session.CurrentUser = null;
            Session.CurrentRole = null;

            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            CloseOtherWindows(mainWindow);
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