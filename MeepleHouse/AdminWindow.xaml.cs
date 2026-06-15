using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MeepleHouse
{
    public partial class AdminWindow : Window
    {
        MeepleHouseDB2Entities db = new MeepleHouseDB2Entities();

        public AdminWindow()
        {
            InitializeComponent();

            LoadUsers();
            LoadWorkers();
            LoadAdmins();
            LoadComplaints();
            LoadBlockedUsers();
            LoadGames();
            LoadAssignments();
        }

        private void LoadUsers()
        {
            var users = db.Users
                .Where(u => u.IsDeleted == false && u.IsBlocked == false)
                .ToList()
                .Select(u => new UserRow
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,

                    RegisteredGames = string.Join(", ",
                        db.Registrations
                            .Where(r => r.UserId == u.Id)
                            .Join(db.BoardGames,
                                r => r.GameId,
                                g => g.Id,
                                (r, g) => g.Title)
                            .ToList())
                })
                .ToList();

            UsersGrid.ItemsSource = users;
        }

        private void LoadWorkers()
        {
            var workers = db.Workers
                .Where(w => w.IsDeleted == false)
                .Select(w => new WorkerRow
                {
                    Id = w.Id,
                    Username = w.Username,
                    FullName = w.FullName,
                    Email = w.Email,
                    Phone = w.Phone
                })
                .ToList();

            WorkersGrid.ItemsSource = workers;
        }

        private void LoadAdmins()
        {
            var admins = db.Admins
                .Where(a => a.IsDeleted == false)
                .ToList()
                .Select(a => new AdminRow
                {
                    Id = a.Id,
                    Username = a.Username,
                    FullName = a.FullName,
                    Email = a.Email,
                    Phone = a.Phone,

                    CreatedBy = a.CreatedByAdminId == null
                        ? ""
                        : db.Admins
                            .Where(ad => ad.Id == a.CreatedByAdminId)
                            .Select(ad => ad.FullName)
                            .FirstOrDefault()
                })
                .ToList();

            AdminsGrid.ItemsSource = admins;
        }

        private void LoadComplaints()
        {
            var complaints = db.WorkerComplaints
                .ToList()
                .Select(c => new ComplaintRow
                {
                    Id = c.Id,
                    WorkerId = c.WorkerId,
                    UserId = c.UserId,
                    ComplaintId = c.ComplaintId,

                    WorkerName = db.Workers
                        .Where(w => w.Id == c.WorkerId)
                        .Select(w => w.FullName)
                        .FirstOrDefault(),

                    UserName = db.Users
                        .Where(u => u.Id == c.UserId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),

                    UserEmail = db.Users
                        .Where(u => u.Id == c.UserId)
                        .Select(u => u.Email)
                        .FirstOrDefault(),

                    ComplaintTitle = db.Complaints
                        .Where(cp => cp.Id == c.ComplaintId)
                        .Select(cp => cp.Title)
                        .FirstOrDefault(),

                    ComplaintText = c.ComplaintText,
                    ComplaintDate = c.ComplaintDate
                })
                .ToList();

            ComplaintsGrid.ItemsSource = complaints;
        }

        private void LoadBlockedUsers()
        {
            var blockedUsers = db.BlockedUsers
                .Where(b => b.IsActive == true)
                .ToList()
                .Select(b => new BlockedUserRow
                {
                    Id = b.Id,
                    UserId = b.UserId,

                    UserName = db.Users
                        .Where(u => u.Id == b.UserId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),

                    Email = db.Users
                        .Where(u => u.Id == b.UserId)
                        .Select(u => u.Email)
                        .FirstOrDefault(),

                    AdminName = db.Admins
                        .Where(a => a.Id == b.BlockedByAdminId)
                        .Select(a => a.FullName)
                        .FirstOrDefault(),

                    ComplaintTitle = b.ComplaintTitle,
                    ComplaintText = b.ComplaintText,
                    BlockedAt = b.BlockedAt
                })
                .ToList();

            BlockedUsersGrid.ItemsSource = blockedUsers;
        }

        private void LoadGames()
        {
            GamesGrid.ItemsSource = db.BoardGames.ToList();
        }

        private void LoadAssignments()
        {
            var assignments = db.WorkerGameAssignments
                .ToList()
                .Select(a => new AssignmentRow
                {
                    Id = a.Id,

                    WorkerName = db.Workers
                        .Where(w => w.Id == a.WorkerId)
                        .Select(w => w.FullName)
                        .FirstOrDefault(),

                    GameTitle = db.BoardGames
                        .Where(g => g.Id == a.GameId)
                        .Select(g => g.Title)
                        .FirstOrDefault(),

                    AdminName = db.Admins
                        .Where(ad => ad.Id == a.AssignedByAdminId)
                        .Select(ad => ad.FullName)
                        .FirstOrDefault(),

                    AssignedDate = a.AssignedDate,
                    IsActive = a.IsActive
                })
                .ToList();

            AssignmentsGrid.ItemsSource = assignments;
        }

        private void RefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void RefreshComplaints_Click(object sender, RoutedEventArgs e)
        {
            LoadComplaints();
        }

        private void RefreshBlockedUsers_Click(object sender, RoutedEventArgs e)
        {
            LoadBlockedUsers();
        }

        private void RefreshGames_Click(object sender, RoutedEventArgs e)
        {
            LoadGames();
            LoadAssignments();
        }

        private void SaveGames_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GamesGrid.CommitEdit();
                GamesGrid.CommitEdit(DataGridEditingUnit.Row, true);

                db.SaveChanges();

                MessageBox.Show("Изменения игр сохранены");

                LoadGames();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void AcceptComplaint_Click(object sender, RoutedEventArgs e)
        {
            ComplaintRow selectedComplaint = ComplaintsGrid.SelectedItem as ComplaintRow;

            if (selectedComplaint == null)
            {
                MessageBox.Show("Выберите жалобу");
                return;
            }

            if (Session.CurrentAdmin == null)
            {
                MessageBox.Show("Администратор не авторизован");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Принять жалобу, заблокировать пользователя и удалить все его записи на игры?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            var user = db.Users.FirstOrDefault(u => u.Id == selectedComplaint.UserId);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден");
                return;
            }

            var complaint = db.WorkerComplaints.FirstOrDefault(c => c.Id == selectedComplaint.Id);

            if (complaint == null)
            {
                MessageBox.Show("Жалоба не найдена");
                return;
            }

            try
            {
                user.IsBlocked = true;

                var userRegistrations = db.Registrations
                    .Where(r => r.UserId == selectedComplaint.UserId)
                    .ToList();

                foreach (var registration in userRegistrations)
                {
                    db.Registrations.Remove(registration);
                }

                bool alreadyBlocked = db.BlockedUsers.Any(b =>
                    b.UserId == selectedComplaint.UserId &&
                    b.IsActive == true);

                if (!alreadyBlocked)
                {
                    BlockedUsers blockedUser = new BlockedUsers
                    {
                        UserId = selectedComplaint.UserId,
                        WorkerId = selectedComplaint.WorkerId,
                        ComplaintTitle = selectedComplaint.ComplaintTitle,
                        ComplaintText = selectedComplaint.ComplaintText,
                        BlockedByAdminId = Session.CurrentAdmin.Id,
                        BlockedAt = DateTime.Now,
                        IsActive = true
                    };

                    db.BlockedUsers.Add(blockedUser);
                }

                db.WorkerComplaints.Remove(complaint);

                db.SaveChanges();

                LoadComplaints();
                LoadBlockedUsers();
                LoadUsers();
                LoadAssignments();

                MessageBox.Show("Жалоба принята. Пользователь заблокирован, все его записи на игры удалены");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при принятии жалобы: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void RejectComplaint_Click(object sender, RoutedEventArgs e)
        {
            ComplaintRow selectedComplaint = ComplaintsGrid.SelectedItem as ComplaintRow;

            if (selectedComplaint == null)
            {
                MessageBox.Show("Выберите жалобу");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Отклонить выбранную жалобу?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            var complaint = db.WorkerComplaints.FirstOrDefault(c => c.Id == selectedComplaint.Id);

            if (complaint == null)
            {
                MessageBox.Show("Жалоба не найдена");
                return;
            }

            try
            {
                db.WorkerComplaints.Remove(complaint);
                db.SaveChanges();

                LoadComplaints();

                MessageBox.Show("Жалоба отклонена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отклонении жалобы: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void UnblockUser_Click(object sender, RoutedEventArgs e)
        {
            BlockedUserRow selectedBlockedUser = BlockedUsersGrid.SelectedItem as BlockedUserRow;

            if (selectedBlockedUser == null)
            {
                MessageBox.Show("Выберите пользователя для разблокировки");
                return;
            }

            if (Session.CurrentAdmin == null)
            {
                MessageBox.Show("Администратор не авторизован");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Разблокировать выбранного пользователя?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            var user = db.Users.FirstOrDefault(u => u.Id == selectedBlockedUser.UserId);
            var blockedUser = db.BlockedUsers.FirstOrDefault(b => b.Id == selectedBlockedUser.Id);

            if (user == null || blockedUser == null)
            {
                MessageBox.Show("Запись не найдена");
                return;
            }

            try
            {
                user.IsBlocked = false;

                blockedUser.IsActive = false;
                blockedUser.UnblockedByAdminId = Session.CurrentAdmin.Id;
                blockedUser.UnblockedAt = DateTime.Now;

                db.SaveChanges();

                LoadBlockedUsers();
                LoadUsers();

                MessageBox.Show("Пользователь разблокирован");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при разблокировке: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void AddStaff_Click(object sender, RoutedEventArgs e)
        {
            AddStaffWindow addStaffWindow = new AddStaffWindow();
            addStaffWindow.Owner = this;

            bool? result = addStaffWindow.ShowDialog();

            if (result == true)
            {
                LoadWorkers();
                LoadAdmins();
            }
        }

        private void AssignWorker_Click(object sender, RoutedEventArgs e)
        {
            AssignWorkerWindow assignWorkerWindow = new AssignWorkerWindow();
            assignWorkerWindow.Owner = this;

            bool? result = assignWorkerWindow.ShowDialog();

            if (result == true)
            {
                LoadAssignments();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            UserRow selectedUser = UsersGrid.SelectedItem as UserRow;

            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для удаления");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Удалить выбранного пользователя?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            var user = db.Users.FirstOrDefault(u => u.Id == selectedUser.Id);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден");
                return;
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.Now;

            db.SaveChanges();

            LoadUsers();

            MessageBox.Show("Пользователь удалён");
        }

        private void DeleteWorker_Click(object sender, RoutedEventArgs e)
        {
            WorkerRow selectedWorker = WorkersGrid.SelectedItem as WorkerRow;

            if (selectedWorker == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Удалить выбранного сотрудника?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            var worker = db.Workers.FirstOrDefault(w => w.Id == selectedWorker.Id);

            if (worker == null)
            {
                MessageBox.Show("Сотрудник не найден");
                return;
            }

            var workerAssignments = db.WorkerGameAssignments
                .Where(a => a.WorkerId == selectedWorker.Id)
                .ToList();

            foreach (var assignment in workerAssignments)
            {
                db.WorkerGameAssignments.Remove(assignment);
            }

            worker.IsDeleted = true;
            worker.DeletedAt = DateTime.Now;

            db.SaveChanges();

            LoadWorkers();
            LoadAssignments();

            MessageBox.Show("Сотрудник удалён, его назначения на игры также удалены");
        }

        private void DeleteAdmin_Click(object sender, RoutedEventArgs e)
        {
            AdminRow selectedAdmin = AdminsGrid.SelectedItem as AdminRow;

            if (selectedAdmin == null)
            {
                MessageBox.Show("Выберите администратора для удаления");
                return;
            }

            if (Session.CurrentAdmin != null && selectedAdmin.Id == Session.CurrentAdmin.Id)
            {
                MessageBox.Show("Нельзя удалить администратора, под которым выполнен вход");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Удалить выбранного администратора?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            var admin = db.Admins.FirstOrDefault(a => a.Id == selectedAdmin.Id);

            if (admin == null)
            {
                MessageBox.Show("Администратор не найден");
                return;
            }

            admin.IsDeleted = true;
            admin.DeletedAt = DateTime.Now;

            db.SaveChanges();

            LoadAdmins();

            MessageBox.Show("Администратор удалён");
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            AdminProfileWindow adminProfileWindow = new AdminProfileWindow();
            adminProfileWindow.Show();

            this.Close();
        }
    }

    public class UserRow
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string RegisteredGames { get; set; }
    }

    public class WorkerRow
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class AdminRow
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CreatedBy { get; set; }
    }

    public class ComplaintRow
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }
        public int UserId { get; set; }
        public int ComplaintId { get; set; }

        public string WorkerName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string ComplaintTitle { get; set; }
        public string ComplaintText { get; set; }
        public DateTime ComplaintDate { get; set; }
    }

    public class BlockedUserRow
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string AdminName { get; set; }
        public string ComplaintTitle { get; set; }
        public string ComplaintText { get; set; }
        public DateTime BlockedAt { get; set; }
    }

    public class AssignmentRow
    {
        public int Id { get; set; }
        public string WorkerName { get; set; }
        public string GameTitle { get; set; }
        public string AdminName { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }
    }
}