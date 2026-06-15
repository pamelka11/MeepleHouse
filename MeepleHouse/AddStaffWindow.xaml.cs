using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MeepleHouse
{
    public partial class AddStaffWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public AddStaffWindow()
        {
            InitializeComponent();

            RoleComboBox.SelectedIndex = 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string fullName = FullNameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();

            ComboBoxItem selectedRoleItem = RoleComboBox.SelectedItem as ComboBoxItem;

            if (selectedRoleItem == null)
            {
                MessageBox.Show("Выберите роль");
                return;
            }

            string role = selectedRoleItem.Content.ToString();

            if (username == "" || password == "" || fullName == "" || email == "")
            {
                MessageBox.Show("Заполните логин, пароль, ФИО и почту");
                return;
            }

            if (Session.CurrentAdmin == null)
            {
                MessageBox.Show("Администратор не авторизован");
                return;
            }

            bool loginExists =
                db.Admins.Any(a => a.Username == username && a.IsDeleted == false) ||
                db.Workers.Any(w => w.Username == username && w.IsDeleted == false);

            if (loginExists)
            {
                MessageBox.Show("Учётная запись с таким логином уже существует");
                return;
            }

            bool emailExists =
                db.Admins.Any(a => a.Email == email && a.IsDeleted == false) ||
                db.Workers.Any(w => w.Email == email && w.IsDeleted == false);

            if (emailExists)
            {
                MessageBox.Show("Учётная запись с такой почтой уже существует");
                return;
            }

            try
            {
                if (role == "Администратор")
                {
                    Admins admin = new Admins
                    {
                        Username = username,
                        Password = password,
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        CreatedByAdminId = Session.CurrentAdmin.Id,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };

                    db.Admins.Add(admin);
                }
                else
                {
                    Workers worker = new Workers
                    {
                        Username = username,
                        Password = password,
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        CreatedByAdminId = Session.CurrentAdmin.Id,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };

                    db.Workers.Add(worker);
                }

                db.SaveChanges();

                MessageBox.Show("Учётная запись успешно добавлена");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}