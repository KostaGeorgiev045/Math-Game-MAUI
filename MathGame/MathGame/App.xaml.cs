using MathGame.Data;

namespace MathGame
{
    public partial class App : Application
    {
        public static GameRepository GameRepository { get; private set; }

        public App(GameRepository gameRepository)
        {
            InitializeComponent();

            //MainPage = new AppShell();

            GameRepository = gameRepository;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}