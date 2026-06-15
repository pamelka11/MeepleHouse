using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MeepleHouse
{
    public partial class WorkerWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public WorkerWindow()
        {
            InitializeComponent();

            LoadAssignedGames();
            LoadComplaintTypes();
        }

        private void LoadAssignedGames()
        {
            if (Session.CurrentWorker == null)
            {
                MessageBox.Show("Сотрудник не авторизован");
                Close();
                return;
            }

            int workerId = Session.CurrentWorker.Id;

            var assignedGames = db.WorkerGameAssignments
                .Where(a => a.WorkerId == workerId && a.IsActive == true)
                .Join(db.BoardGames,
                    a => a.GameId,
                    g => g.Id,
                    (a, g) => new
                    {
                        Id = g.Id,
                        Title = g.Title,
                        Description = g.Description
                    })
                .ToList();

            var games = assignedGames
                .Select(g => new AssignedGameRow
                {
                    Id = g.Id,
                    Title = g.Title,
                    Description = g.Description,
                    Genre = GetGameGenre(g.Title),
                    ImagePath = GetGameImage(g.Title)
                })
                .ToList();

            AssignedGamesList.ItemsSource = games;
            ParticipantsGameComboBox.ItemsSource = games;
            ComplaintGameComboBox.ItemsSource = games;

            EmptyGamesText.Visibility = games.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (games.Count > 0)
            {
                ParticipantsGameComboBox.SelectedIndex = 0;
                ComplaintGameComboBox.SelectedIndex = 0;
            }
            else
            {
                ParticipantsGrid.ItemsSource = null;
                ComplaintUserComboBox.ItemsSource = null;
            }
        }

        private void LoadParticipants(int gameId)
        {
            var participants = db.Registrations
                .Where(r => r.GameId == gameId)
                .Join(db.Users,
                    r => r.UserId,
                    u => u.Id,
                    (r, u) => new ParticipantRow
                    {
                        Id = u.Id,
                        Username = u.Username,
                        FullName = u.FullName,
                        Email = u.Email,
                        Phone = u.Phone
                    })
                .ToList();

            ParticipantsGrid.ItemsSource = participants;
        }

        private void LoadComplaintUsers(int gameId)
        {
            var users = db.Registrations
                .Where(r => r.GameId == gameId)
                .Join(db.Users,
                    r => r.UserId,
                    u => u.Id,
                    (r, u) => new ParticipantRow
                    {
                        Id = u.Id,
                        Username = u.Username,
                        FullName = u.FullName,
                        Email = u.Email,
                        Phone = u.Phone
                    })
                .ToList();

            ComplaintUserComboBox.ItemsSource = users;

            if (users.Count > 0)
            {
                ComplaintUserComboBox.SelectedIndex = 0;
            }
            else
            {
                ComplaintUserComboBox.SelectedIndex = -1;
            }
        }

        private void LoadComplaintTypes()
        {
            var complaints = db.Complaints
                .Select(c => new ComplaintTypeRow
                {
                    Id = c.Id,
                    Title = c.Title
                })
                .ToList();

            ComplaintTypeComboBox.ItemsSource = complaints;

            if (complaints.Count > 0)
            {
                ComplaintTypeComboBox.SelectedIndex = 0;
            }
        }

        private void ParticipantsGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssignedGameRow selectedGame = ParticipantsGameComboBox.SelectedItem as AssignedGameRow;

            if (selectedGame != null)
            {
                LoadParticipants(selectedGame.Id);
            }
        }

        private void ComplaintGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssignedGameRow selectedGame = ComplaintGameComboBox.SelectedItem as AssignedGameRow;

            if (selectedGame != null)
            {
                LoadComplaintUsers(selectedGame.Id);
            }
        }

        private void RefreshParticipants_Click(object sender, RoutedEventArgs e)
        {
            AssignedGameRow selectedGame = ParticipantsGameComboBox.SelectedItem as AssignedGameRow;

            if (selectedGame == null)
            {
                MessageBox.Show("Выберите игру");
                return;
            }

            LoadParticipants(selectedGame.Id);
        }

        private void AssignedGame_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
                return;

            int gameId = (int)button.Tag;

            WorkerGameDetailsWindow gameWindow = new WorkerGameDetailsWindow(gameId);
            gameWindow.Owner = this;
            gameWindow.ShowDialog();
        }

        private void SendComplaint_Click(object sender, RoutedEventArgs e)
        {
            if (Session.CurrentWorker == null)
            {
                MessageBox.Show("Сотрудник не авторизован");
                return;
            }

            AssignedGameRow selectedGame = ComplaintGameComboBox.SelectedItem as AssignedGameRow;
            ParticipantRow selectedUser = ComplaintUserComboBox.SelectedItem as ParticipantRow;
            ComplaintTypeRow selectedComplaint = ComplaintTypeComboBox.SelectedItem as ComplaintTypeRow;

            if (selectedGame == null)
            {
                MessageBox.Show("Выберите игру");
                return;
            }

            if (selectedUser == null)
            {
                MessageBox.Show("Выберите участника");
                return;
            }

            if (selectedComplaint == null)
            {
                MessageBox.Show("Выберите тип жалобы");
                return;
            }

            WorkerComplaints complaint = new WorkerComplaints
            {
                WorkerId = Session.CurrentWorker.Id,
                UserId = selectedUser.Id,
                ComplaintId = selectedComplaint.Id,
                ComplaintText = ComplaintTextBox.Text.Trim(),
                ComplaintDate = DateTime.Now,
                IsReviewed = false
            };

            try
            {
                db.WorkerComplaints.Add(complaint);
                db.SaveChanges();

                ComplaintTextBox.Clear();

                MessageBox.Show("Жалоба отправлена администратору");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке жалобы: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private string GetGameImage(string title)
        {
            switch (title)
            {
                case "Каркассон":
                    return "ButtonImage\\carcason.png";

                case "Манчкин":
                    return "ButtonImage\\Munchcin.png";

                case "Колонизаторы":
                    return "ButtonImage\\catan.png";

                case "Мафия":
                    return "ButtonImage\\mafia.png";

                case "Диксит":
                    return "ButtonImage\\dixit.png";

                case "Билет на поезд":
                    return "ButtonImage\\ticket.png";

                case "7 чудес":
                    return "ButtonImage\\7wonders.png";

                case "Имаджинариум":
                    return "ButtonImage\\imaginarium.png";

                case "Уно":
                    return "ButtonImage\\uno.png";

                case "Пандемия":
                    return "ButtonImage\\pandemic.png";

                default:
                    return "ButtonImage\\catan.png";
            }
        }

        private string GetGameGenre(string title)
        {
            switch (title)
            {
                case "Каркассон":
                    return "Стратегическая игра";

                case "Манчкин":
                    return "Карточная игра";

                case "Колонизаторы":
                    return "Экономическая стратегия";

                case "Мафия":
                    return "Психологическая игра";

                case "Диксит":
                    return "Ассоциативная игра";

                case "Билет на поезд":
                    return "Семейная стратегия";

                case "7 чудес":
                    return "Цивилизационная игра";

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

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            WorkerProfileWindow workerProfileWindow = new WorkerProfileWindow();
            workerProfileWindow.Show();

            
        }
    }

    public class AssignedGameRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string ImagePath { get; set; }
    }

    public class ParticipantRow
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class ComplaintTypeRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}