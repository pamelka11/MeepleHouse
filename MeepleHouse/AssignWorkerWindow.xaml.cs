using System;
using System.Linq;
using System.Windows;

namespace MeepleHouse
{
    public partial class AssignWorkerWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public AssignWorkerWindow()
        {
            InitializeComponent();

            LoadWorkers();
            LoadGames();
        }

        private void LoadWorkers()
        {
            var workers = db.Workers
                .Where(w => w.IsDeleted == false)
                .Select(w => new ComboItem
                {
                    Id = w.Id,
                    Name = w.FullName
                })
                .ToList();

            WorkerComboBox.ItemsSource = workers;
        }

        private void LoadGames()
        {
            var games = db.BoardGames
                .Select(g => new ComboItem
                {
                    Id = g.Id,
                    Name = g.Title
                })
                .ToList();

            GameComboBox.ItemsSource = games;
        }

        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            if (WorkerComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите сотрудника");
                return;
            }

            if (GameComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите игру");
                return;
            }

            if (Session.CurrentAdmin == null)
            {
                MessageBox.Show("Администратор не авторизован");
                return;
            }

            int workerId = (int)WorkerComboBox.SelectedValue;
            int gameId = (int)GameComboBox.SelectedValue;
            int adminId = Session.CurrentAdmin.Id;

            bool alreadyAssigned = db.WorkerGameAssignments.Any(a =>
                a.WorkerId == workerId &&
                a.GameId == gameId &&
                a.IsActive == true);

            if (alreadyAssigned)
            {
                MessageBox.Show("Этот сотрудник уже назначен на выбранную игру");
                return;
            }

            WorkerGameAssignments assignment = new WorkerGameAssignments
            {
                WorkerId = workerId,
                GameId = gameId,
                AssignedByAdminId = adminId,
                AssignedDate = DateTime.Now,
                IsActive = true
            };

            try
            {
                db.WorkerGameAssignments.Add(assignment);
                db.SaveChanges();

                MessageBox.Show("Сотрудник успешно назначен на игру");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при назначении: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ComboItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}