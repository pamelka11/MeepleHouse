using System.Linq;
using System.Windows;

namespace MeepleHouse
{
    public partial class EditProfileWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public EditProfileWindow()
        {
            InitializeComponent();

            LoadProfileData();
        }

        private void LoadProfileData()
        {
            if (Session.CurrentAdmin != null)
            {
                NameBox.Text = Session.CurrentAdmin.FullName ?? "";
                EmailBox.Text = Session.CurrentAdmin.Email ?? "";
                PhoneBox.Text = Session.CurrentAdmin.Phone ?? "";
                return;
            }

            if (Session.CurrentWorker != null)
            {
                NameBox.Text = Session.CurrentWorker.FullName ?? "";
                EmailBox.Text = Session.CurrentWorker.Email ?? "";
                PhoneBox.Text = Session.CurrentWorker.Phone ?? "";
                return;
            }

            if (Session.CurrentUser != null)
            {
                NameBox.Text = Session.CurrentUser.FullName ?? "";
                EmailBox.Text = Session.CurrentUser.Email ?? "";
                PhoneBox.Text = Session.CurrentUser.Phone ?? "";
                return;
            }

            MessageBox.Show("Пользователь не авторизован");
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Session.CurrentAdmin != null)
            {
                var admin = db.Admins.FirstOrDefault(a => a.Id == Session.CurrentAdmin.Id);

                if (admin != null)
                {
                    admin.FullName = NameBox.Text;
                    admin.Email = EmailBox.Text;
                    admin.Phone = PhoneBox.Text;

                    db.SaveChanges();

                    Session.CurrentAdmin = admin;

                    MessageBox.Show("Профиль администратора обновлён");
                }

                Close();
                return;
            }

            if (Session.CurrentWorker != null)
            {
                var worker = db.Workers.FirstOrDefault(w => w.Id == Session.CurrentWorker.Id);

                if (worker != null)
                {
                    worker.FullName = NameBox.Text;
                    worker.Email = EmailBox.Text;
                    worker.Phone = PhoneBox.Text;

                    db.SaveChanges();

                    Session.CurrentWorker = worker;

                    MessageBox.Show("Профиль работника обновлён");
                }

                Close();
                return;
            }

            if (Session.CurrentUser != null)
            {
                var user = db.Users.FirstOrDefault(u => u.Id == Session.CurrentUser.Id);

                if (user != null)
                {
                    user.FullName = NameBox.Text;
                    user.Email = EmailBox.Text;
                    user.Phone = PhoneBox.Text;

                    db.SaveChanges();

                    Session.CurrentUser = user;

                    MessageBox.Show("Профиль обновлён");
                }

                Close();
                return;
            }

            MessageBox.Show("Пользователь не авторизован");
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}