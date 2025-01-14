namespace MathGame
{
    public partial class MainPage : ContentPage
    {
        public int SelectedRounds { get; set; } = 5; // Default rounds
        public int SelectedTime { get; set; } = 30; // Default time limit

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnRoundsChanged(object sender, ValueChangedEventArgs e)
        {
            SelectedRounds = (int)e.NewValue; // Round to integer
            RoundsLabel.Text = $"Rounds: {SelectedRounds}";
        }

        private void OnTimeChanged(object sender, ValueChangedEventArgs e)
        {
            SelectedTime = (int)e.NewValue; // Round to integer
            TimeLabel.Text = SelectedTime == 120 ? "Time: Unlimited" : $"Time: {SelectedTime} seconds";
        }


        private void OnGameChosen(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var gameType = button.Text;

            // Navigate to GamePage with selected game type and rounds
            Navigation.PushAsync(new GamePage(gameType, SelectedRounds, SelectedTime));
        }

        private void OnViewPreviousGameChosen(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PreviousGames());
        }
    }
}
