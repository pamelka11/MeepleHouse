using System.Windows;
using MeepleHouse.Games;

namespace MeepleHouse
{
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            InitializeComponent();
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profile = new ProfileWindow();
            profile.ShowDialog();
        }

        private void Carcassonne_Click(object sender, RoutedEventArgs e)
        {
            new CarcassonneWindow().ShowDialog();
        }

        private void Catan_Click(object sender, RoutedEventArgs e)
        {
            new CatanWindow().ShowDialog();
        }

        private void Ticket_Click(object sender, RoutedEventArgs e)
        {
            new TicketToRideWindow().ShowDialog();
        }

        private void Dixit_Click(object sender, RoutedEventArgs e)
        {
            new DixitWindow().ShowDialog();
        }

        private void Munchkin_Click(object sender, RoutedEventArgs e)
        {
            new MunchkinWindow().ShowDialog();
        }

        private void Uno_Click(object sender, RoutedEventArgs e)
        {
            new UnoWindow().ShowDialog();
        }

        private void Pandemic_Click(object sender, RoutedEventArgs e)
        {
            new PandemicWindow().ShowDialog();
        }

        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            new SevenWondersWindow().ShowDialog();
        }

        private void Imaginarium_Click(object sender, RoutedEventArgs e)
        {
            new ImaginariumWindow().ShowDialog();
        }

        private void Mafia_Click(object sender, RoutedEventArgs e)
        {
            new MafiaWindow().ShowDialog();
        }

       
    }
}