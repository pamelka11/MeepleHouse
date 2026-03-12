using System.Windows;

namespace MeepleHouse
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();

            LoadUser();
        }

        void LoadUser()
        {
            NameText.Text = Session.CurrentUser.FullName;
            EmailText.Text = Session.CurrentUser.Email;
            PhoneText.Text = Session.CurrentUser.Phone;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow window = new EditProfileWindow();
            window.ShowDialog();

            // обновляем данные после редактирования
            LoadUser();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;

            MainWindow window = new MainWindow();
            window.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w is MainWindow1)
                {
                    w.Close();
                    break;
                }
            }

            this.Close();
        }
    }
}