using System.Linq;
using System.Windows;

namespace MeepleHouse.Games
{
    public partial class TicketToRideWindow : Window
    {

        MeepleHouseDBEntities db = new MeepleHouseDBEntities();
        int gameId = 3;

        public TicketToRideWindow()
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

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Чтобы записаться, необходимо авторизоваться");
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

            reg.UserId = Session.CurrentUser.Id;
            reg.GameId = gameId;
            reg.RegistrationDate = System.DateTime.Now;

            db.Registrations.Add(reg);
            db.SaveChanges();

            LoadPlayers();
        }
    }
}