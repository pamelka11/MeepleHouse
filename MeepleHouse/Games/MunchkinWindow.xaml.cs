using System;
using System.Linq;
using System.Windows;

namespace MeepleHouse.Games
{
    public partial class MunchkinWindow : Window
    {
        MeepleHouseDBEntities db = new MeepleHouseDBEntities();

        int gameId = 2; // МАНЧКИН

        public MunchkinWindow()
        {
            InitializeComponent();
            LoadPlayers();
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
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Вы не авторизованы");
                return;
            }

            var existing = db.Registrations.FirstOrDefault(r =>
                r.GameId == gameId &&
                r.UserId == Session.CurrentUser.Id);

            if (existing != null)
            {
                MessageBox.Show("Вы уже записаны на этот кружок");
                return;
            }

            int registered = db.Registrations.Count(r => r.GameId == gameId);

            int maxPlayers = db.BoardGames
                .Where(g => g.Id == gameId)
                .Select(g => g.MaxPlayers)
                .FirstOrDefault() ?? 0;

            if (registered >= maxPlayers)
            {
                MessageBox.Show("Группа уже заполнена");
                return;
            }

            Registrations reg = new Registrations();
            reg.GameId = gameId;
            reg.UserId = Session.CurrentUser.Id;

            db.Registrations.Add(reg);
            db.SaveChanges();

            MessageBox.Show("Вы успешно записались");

            LoadPlayers();
        }

        void LoadDate()
        {
            DateTime baseTime = new DateTime(2026, 7, 1, 10, 25, 0);

            DateTime gameTime = baseTime.AddHours(gameId - 1);

            DateText.Text = "Ближайшая игра: " + gameTime.ToString("d MMMM HH:mm");
        }
    }
    }