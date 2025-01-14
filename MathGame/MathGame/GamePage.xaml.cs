using MathGame.Models;
using Microsoft.UI.Composition.Interactions;

namespace MathGame;

public partial class GamePage : ContentPage
{
	private const int MIN_NUMBER = 1;
    private const int MAX_NUMBER_BASIC = 9;
    private const int MAX_NUMBER_DIVISION = 99;
    
    private readonly string gameType;
    private readonly int totalRounds;
    private readonly int timeLimit;
    private int currentRound;
    private int currentScore;
    private int remainingTime;
    private int firstNumber;
    private int secondNumber;
    private readonly Random random;
    private CancellationTokenSource timerCancellation;

    public GamePage(string gameType, int rounds, int timeLimit)
    {
        InitializeComponent();
        this.gameType = gameType;
        totalRounds = rounds;
        this.timeLimit = timeLimit == 120 ? int.MaxValue : timeLimit; // Treat max as unlimited
        this.remainingTime = timeLimit == 120 ? int.MaxValue : timeLimit; // Initialize remainingTime
        currentRound = 1;
        currentScore = 0;
        random = new Random();
        
        BindingContext = this;
        StartGame();
    }

    private string GetGameOperand() => gameType switch
    {
        "Addition" => "+",
        "Subtraction" => "-",
        "Multiplication" => "*",
        "Division" => "/",
        _ => string.Empty
    };

    private void GenerateNumbers()
    {
        int maxNumber = gameType != "Division" ? MAX_NUMBER_BASIC : MAX_NUMBER_DIVISION;
        
        firstNumber = random.Next(MIN_NUMBER, maxNumber);
        secondNumber = random.Next(MIN_NUMBER, maxNumber);

        if (gameType == "Division")
        {
            EnsureValidDivision();
        }
    }

    private void EnsureValidDivision()
    {
        while (firstNumber < secondNumber || firstNumber % secondNumber != 0)
        {
            firstNumber = random.Next(MIN_NUMBER, MAX_NUMBER_DIVISION);
            secondNumber = random.Next(MIN_NUMBER, MAX_NUMBER_DIVISION);
        }
    }

    #region Timer

    private void StartGame()
    {
        StartTimer(); // Start the timer for the entire game session
        StartRound(); // Start the first round
    }

    private void StartRound()
    {
        CreateNewQuestion();
        if (timeLimit != int.MaxValue)
        {
            StartTimer();
        }
        else
        {
            TimerLabel.Text = "Time Left: Unlimited";
        }
    }

    private void CreateNewQuestion()
    {
        string gameOperand = GetGameOperand();
        GenerateNumbers();
        QuestionLabel.Text = $"{firstNumber} {gameOperand} {secondNumber}";
    }

    private void StartTimer()
    {
        timerCancellation?.Cancel();
        timerCancellation = new CancellationTokenSource();
        var token = timerCancellation.Token;

        Task.Run(async () =>
        {
            while (remainingTime > 0 && !token.IsCancellationRequested)
            {
                Dispatcher.Dispatch(() => TimerLabel.Text = $"Time Left: {remainingTime}s");
                await Task.Delay(1000, token);
                remainingTime--;
            }

            if (remainingTime <= 0 && !token.IsCancellationRequested)
            {
                Dispatcher.Dispatch(() =>
                {
                    TimerLabel.Text = "Time's up!";
                    GameOver(); // End the game when time runs out
                });
            }
        }, token);
    }

    #endregion

    private void OnAnswerSubmitted(object sender, EventArgs e)
    {
        if (!int.TryParse(AnswerEntry.Text, out int answer))
        {
            AnswerLabel.Text = "Please enter a valid number";
            return;
        }

        bool isCorrect = CheckAnswer(answer);
        ProcessAnswer(isCorrect);
        HandleRoundProgress();
    }

    private bool CheckAnswer(int answer) => gameType switch
    {
        "Addition" => answer == firstNumber + secondNumber,
        "Subtraction" => answer == firstNumber - secondNumber,
        "Multiplication" => answer == firstNumber * secondNumber,
        "Division" => answer == firstNumber / secondNumber,
        _ => false
    };

    private void HandleRoundProgress()
    {
        currentRound++;
        timerCancellation?.Cancel(); // Stop current round's timer

        if (currentRound <= totalRounds)
        {
            AnswerEntry.Text = string.Empty;
            StartRound();
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GameOperation gameOperation = GetGameOperation();

        timerCancellation?.Cancel();
        QuestionArea.IsVisible = false;
        BackToMenuBtn.IsVisible = true;
        GameOverLabel.Text = $"Game over! You got {currentScore} out of {totalRounds} right";
        
        SaveGameResult(gameOperation);
    }

    private GameOperation GetGameOperation() => gameType switch
    {
        "Addition" => GameOperation.Addition,
        "Subtraction" => GameOperation.Subtraction,
        "Multiplication" => GameOperation.Multiplication,
        "Division" => GameOperation.Division,
        _ => throw new ArgumentException("Invalid game type")
    };

    private void SaveGameResult(GameOperation gameOperation)
    {
        App.GameRepository.Add(new Game
        {
            DatePlayed = DateTime.UtcNow,
            Type = gameOperation,
            Score = currentScore
        });
    }

    private void ProcessAnswer(bool isCorrect)
    {
        if (isCorrect)
            currentScore++;
            
        AnswerLabel.Text = isCorrect ? "Correct" : "Incorrect";
    }

    private void OnBackToMenu(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
}