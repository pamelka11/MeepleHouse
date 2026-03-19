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

            // заполняем поля данными пользователя
            NameBox.Text = Session.CurrentUser.FullName;
            EmailBox.Text = Session.CurrentUser.Email;
            PhoneBox.Text = Session.CurrentUser.Phone;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
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

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}