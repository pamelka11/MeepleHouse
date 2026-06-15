using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MeepleHouse
{
    public partial class WorkerGameDetailsWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        private int currentGameId;

        public WorkerGameDetailsWindow(int gameId)
        {
            InitializeComponent();

            currentGameId = gameId;
            LoadGameData();
        }

        private void LoadGameData()
        {
            var game = db.BoardGames.FirstOrDefault(g => g.Id == currentGameId);

            if (game == null)
            {
                MessageBox.Show("Игра не найдена");
                Close();
                return;
            }

            Title = game.Title;

            TitleText.Text = game.Title;
            SubtitleText.Text = GetGameGenre(game.Title);

            DescriptionText.Text = game.Description;
            RulesText.Text = GetGameRules(game.Title);

            DateText.Text = GetGameDateText(game.Title);

            int registeredCount = db.Registrations
                .Count(r => r.GameId == currentGameId);

            PlayersText.Text = "Записано: " + registeredCount + " из " + game.MaxPlayers;

            GameImage.Source = new BitmapImage(
                new Uri(GetGameImage(game.Title), UriKind.Absolute));
        }

        private string GetGameDateText(string title)
        {
            switch (title)
            {
                case "Каркассон":
                    return "Время записи: 1 июля 10:25";

                case "Манчкин":
                    return "Время записи: 2 июля 11:30";

                case "Колонизаторы":
                    return "Время записи: 3 июля 12:00";

                case "Мафия":
                    return "Время записи: 4 июля 18:30";

                case "Диксит":
                    return "Время записи: 5 июля 14:20";

                case "Билет на поезд":
                    return "Время записи: 6 июля 15:10";

                case "7 чудес":
                    return "Время записи: 7 июля 16:40";

                case "Имаджинариум":
                    return "Время записи: 8 июля 17:15";

                case "Уно":
                    return "Время записи: 9 июля 13:00";

                case "Пандемия":
                    return "Время записи: 10 июля 19:00";

                default:
                    return "Время записи: 1 июля 10:25";
            }
        }

        private string GetGameImage(string title)
        {
            switch (title)
            {
                case "Каркассон":
                    return "pack://application:,,,/Games/Image/carcassonne.png";

                case "Манчкин":
                    return "pack://application:,,,/Games/Image/munchkin.png";

                case "Колонизаторы":
                    return "pack://application:,,,/Games/Image/catan.png";

                case "Мафия":
                    return "pack://application:,,,/Games/Image/mafia.png";

                case "Диксит":
                    return "pack://application:,,,/Games/Image/dixit.png";

                case "Билет на поезд":
                    return "pack://application:,,,/Games/Image/ticket_to_ride.png";

                case "7 чудес":
                    return "pack://application:,,,/Games/Image/7wonders.png";

                case "Имаджинариум":
                    return "pack://application:,,,/Games/Image/imaginarium.png";

                case "Уно":
                    return "pack://application:,,,/Games/Image/uno.png";

                case "Пандемия":
                    return "pack://application:,,,/Games/Image/pandemic.png";

                default:
                    return "pack://application:,,,/Games/Image/carcassonne.png";
            }
        }

        private string GetGameGenre(string title)
        {
            switch (title)
            {
                case "Каркассон":
                    return "Стратегическая настольная игра";

                case "Манчкин":
                    return "Карточная настольная игра";

                case "Колонизаторы":
                    return "Экономическая стратегия";

                case "Мафия":
                    return "Психологическая игра";

                case "Диксит":
                    return "Ассоциативная настольная игра";

                case "Билет на поезд":
                    return "Семейная стратегия";

                case "7 чудес":
                    return "Цивилизационная стратегия";

                case "Имаджинариум":
                    return "Игра на воображение";

                case "Уно":
                    return "Карточная игра";

                case "Пандемия":
                    return "Кооперативная игра";

                default:
                    return "Настольная игра";
            }
        }

        private string GetGameRules(string title)
        {
            switch (title)
            {
                case "Каркассон":
                    return "Игроки по очереди выкладывают тайлы местности, строят города, дороги и монастыри. Очки начисляются за завершённые объекты и размещённых подданных.";

                case "Манчкин":
                    return "Игроки открывают двери, сражаются с монстрами, получают сокровища и стараются первыми достичь 10 уровня.";

                case "Колонизаторы":
                    return "Игроки добывают ресурсы, строят дороги, поселения и города. Побеждает участник, первым набравший 10 победных очков.";

                case "Мафия":
                    return "Игроки получают скрытые роли. Мирные жители пытаются вычислить мафию, а мафия устраняет игроков ночью.";

                case "Диксит":
                    return "Игроки придумывают ассоциации к картам, голосуют за подходящие изображения и получают очки за удачные подсказки.";

                case "Билет на поезд":
                    return "Игроки собирают карты вагонов, строят железнодорожные маршруты и выполняют билеты направлений.";

                case "7 чудес":
                    return "Игроки развивают города, строят здания, развивают науку, торговлю и армию, набирая победные очки.";

                case "Имаджинариум":
                    return "Игроки используют ассоциации к необычным изображениям, угадывают карты других участников и получают очки.";

                case "Уно":
                    return "Игроки выкладывают карты по цвету или значению и стараются первыми избавиться от всех карт на руках.";

                case "Пандемия":
                    return "Игроки действуют совместно, лечат болезни, перемещаются между городами и стараются спасти мир от эпидемий.";

                default:
                    return "Игроки выполняют действия по правилам настольной игры и набирают победные очки.";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}