using System.Linq;
using System.Windows;

namespace MeepleHouse.Games
{
    public partial class MunchkinWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        private int gameId;

        public MunchkinWindow()
        {
            InitializeComponent();

            LoadGameData();
        }

        private void LoadGameData()
        {
            var game = db.BoardGames.FirstOrDefault(g => g.Title == "Манчкин");

            if (game == null)
            {
                MessageBox.Show("Игра не найдена в базе данных");
                return;
            }

            gameId = game.Id;

            DateText.Text = "Ближайшая игра: 2 июля 11:30";

            int registeredCount = db.Registrations
                .Count(r => r.GameId == gameId);

            PlayersText.Text = "Записано: " + registeredCount + " из " + game.MaxPlayers;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Для записи необходимо авторизоваться");

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                this.Close();
                return;
            }

            bool alreadyRegistered = db.Registrations.Any(r =>
                r.UserId == Session.CurrentUser.Id &&
                r.GameId == gameId);

            if (alreadyRegistered)
            {
                MessageBox.Show("Вы уже записаны на эту игру");
                return;
            }

            var game = db.BoardGames.FirstOrDefault(g => g.Id == gameId);

            if (game == null)
            {
                MessageBox.Show("Игра не найдена");
                return;
            }

            int registeredCount = db.Registrations
                .Count(r => r.GameId == gameId);

            if (registeredCount >= game.MaxPlayers)
            {
                MessageBox.Show("На эту игру уже нет свободных мест");
                return;
            }

            Registrations registration = new Registrations
            {
                UserId = Session.CurrentUser.Id,
                GameId = gameId,
                RegistrationDate = System.DateTime.Now
            };

            db.Registrations.Add(registration);
            db.SaveChanges();

            MessageBox.Show("Вы успешно записались на игру");

            LoadGameData();
        }
    }
}