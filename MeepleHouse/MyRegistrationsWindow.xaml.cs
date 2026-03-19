using System;
using System.Linq;
using System.Windows;

namespace MeepleHouse
{
    public partial class MyRegistrationsWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public MyRegistrationsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            var list = db.Registrations
                .Where(r => r.UserId == Session.CurrentUser.Id)
                .ToList()
                .Select(r => new
                {
                    r.Id,
                    GameName = r.BoardGames.Title,

                    // 🔥 ВЫЧИСЛЯЕМ ДАТУ ТУТ ЖЕ
                    GameDate = new DateTime(2026, 7, 1, 10, 25, 0)
                                .AddHours((double)(r.GameId - 1))
                                .ToString("d MMMM HH:mm", new System.Globalization.CultureInfo("ru-RU"))
                })
                .ToList();

            RegistrationsList.ItemsSource = list;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (RegistrationsList.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись");
                return;
            }

            var selected = RegistrationsList.SelectedItem;
            var id = (int)selected.GetType().GetProperty("Id").GetValue(selected);

            var reg = db.Registrations.FirstOrDefault(r => r.Id == id);

            if (reg != null)
            {
                db.Registrations.Remove(reg);
                db.SaveChanges();

                MessageBox.Show("Запись отменена");

                LoadData();

            }
        }
    }
}