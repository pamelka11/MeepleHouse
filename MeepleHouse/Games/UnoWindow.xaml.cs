using System;
using System.Linq;
using System.Windows;

namespace MeepleHouse.Games
{
    public partial class UnoWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        int gameId = 9;

        public UnoWindow()
        {
            InitializeComponent();
            LoadPlayers();
            LoadDate();
        }

        void LoadPlayers()
        {
            int registered = db.Registrations.Count(r => r.GameId == gameId);

            int maxPlayers = db.BoardGames
                .Where(g => g.Id == gameId)
                .Select(g => g.MaxPlayers)
                .FirstOrDefault() ?? 0;

            PlayersText.Text = "Записано: " + registered + " из " + maxPlayers;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // проверка авторизации
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Вы не авторизованы");
                return;
            }

            // проверка: уже записан
            bool exists = db.Registrations.Any(r =>
                r.GameId == gameId &&
                r.UserId == Session.CurrentUser.Id);

            if (exists)
            {
                MessageBox.Show("Вы уже записаны на этот кружок");
                return;
            }

            // сколько уже записано
            int registered = db.Registrations.Count(r => r.GameId == gameId);

            // максимум игроков
            int maxPlayers = db.BoardGames
                .Where(g => g.Id == gameId)
                .Select(g => g.MaxPlayers)
                .FirstOrDefault() ?? 0;

            // проверка заполненности
            if (registered >= maxPlayers)
            {
                MessageBox.Show("Группа уже заполнена");
                return;
            }

            // создание записи
            Registrations reg = new Registrations
            {
                GameId = gameId,
                UserId = Session.CurrentUser.Id,
                RegistrationDate = DateTime.Now
            };

            try
            {
                db.Registrations.Add(reg);   // 🔥 ВАЖНО
                db.SaveChanges();            // 🔥 ВАЖНО

                MessageBox.Show("Вы успешно записались");

                LoadPlayers(); // обновление
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.InnerException?.Message);
            }
        }
        void LoadDate()
        {
            DateTime baseTime = new DateTime(2026, 7, 1, 10, 25, 0);

            DateTime gameTime = baseTime.AddHours(gameId - 1);

            DateText.Text = "Ближайшая игра: " + gameTime.ToString("d MMMM HH:mm");
        }
    }
}