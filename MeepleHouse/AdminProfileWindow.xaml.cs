using System.Linq;
using System.Windows;

namespace MeepleHouse
{
    public partial class AdminProfileWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public AdminProfileWindow()
        {
            InitializeComponent();
            LoadAdminData();
        }

        private void LoadAdminData()
        {
            if (Session.CurrentAdmin == null)
            {
                MessageBox.Show("Администратор не авторизован");
                Close();
                return;
            }

            int adminId = Session.CurrentAdmin.Id;

            var admin = db.Admins.FirstOrDefault(a => a.Id == adminId);

            if (admin == null)
            {
                MessageBox.Show("Администратор не найден");
                Close();
                return;
            }

            FullNameBox.Text = admin.FullName;
            EmailBox.Text = admin.Email;
            PhoneBox.Text = admin.Phone;
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editProfileWindow = new EditProfileWindow();
            editProfileWindow.Owner = this;
            editProfileWindow.ShowDialog();

            LoadAdminData();
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