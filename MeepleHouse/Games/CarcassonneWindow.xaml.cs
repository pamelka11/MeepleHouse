using System;
using System.Linq;
using System.Windows;

namespace MeepleHouse.Games
{
    public partial class CarcassonneWindow : Window
    {

        MeepleHouseDBEntities db = new MeepleHouseDBEntities();

        // ИСПРАВЛЕНО: добавлен gameId для конкретной игры
        int gameId = 1;

        public CarcassonneWindow()
        {
            InitializeComponent();

            // ИСПРАВЛЕНО: при открытии окна загружается количество игроков
            LoadPlayers();
            LoadDate();
        }

        // ИСПРАВЛЕНО: метод загрузки количества записанных
        void LoadPlayers()
        {
            int registered = db.Registrations.Count(r => r.GameId == gameId);

            int maxPlayers = db.BoardGames
                .Where(g => g.Id == gameId)
                .Select(g => g.MaxPlayers)
                .FirstOrDefault() ?? 0;

            PlayersText.Text = "Записано: " + registered + " из " + maxPlayers;
        }

        // ИСПРАВЛЕНО: полностью переписана логика записи
        private void Register_Click(object sender, RoutedEventArgs e)
        {

            // проверка авторизации
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Вы не авторизованы");
                return;
            }

            // проверка: уже записан
            var existing = db.Registrations.FirstOrDefault(r =>
                r.GameId == gameId &&
                r.UserId == Session.CurrentUser.Id);

            if (existing != null)
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

            // ИСПРАВЛЕНО: создание новой записи
            Registrations reg = new Registrations();
            reg.GameId = gameId;
            reg.UserId = Session.CurrentUser.Id;

            db.Registrations.Add(reg);
            db.SaveChanges();

            MessageBox.Show("Вы успешно записались");

            // ИСПРАВЛЕНО: обновление количества игроков
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