namespace MathGame
{
    public partial class MainPage : ContentPage
    {
        public int SelectedRounds { get; set; } = 5; // Default rounds

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnRoundsChanged(object sender, ValueChangedEventArgs e)
        {
            SelectedRounds = (int)e.NewValue; // Round to integer
            RoundsLabel.Text = $"Rounds: {SelectedRounds}";
        }

        private void OnGameChosen(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var gameType = button.Text;

            // Navigate to GamePage with selected game type and rounds
            Navigation.PushAsync(new GamePage(gameType, SelectedRounds));
        }

        private void OnViewPreviousGameChosen(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PreviousGames());
        }
    }
}
