using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MeepleHouse
{
    public partial class RegisterWindow : Window
    {
        MeepleHouseDBEntities db = new MeepleHouseDBEntities();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == "" ||
                EmailBox.Text == "" ||
                PhoneBox.Text == "" ||
                LoginBox.Text == "" ||
                PasswordBox.Password == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            var existUser = db.Users.FirstOrDefault(u => u.Username == LoginBox.Text);

            if (existUser != null)
            {
                MessageBox.Show("Такой логин уже существует");
                return;
            }

            Users newUser = new Users();

            newUser.FullName = NameBox.Text;
            newUser.Email = EmailBox.Text;
            newUser.Phone = PhoneBox.Text;
            newUser.Username = LoginBox.Text;
            newUser.Password = PasswordBox.Password;

            db.Users.Add(newUser);
            db.SaveChanges();

            MessageBox.Show("Регистрация успешна");

            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}