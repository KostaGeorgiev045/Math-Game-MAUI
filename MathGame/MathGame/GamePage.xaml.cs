using MathGame.Models;
using Microsoft.UI.Composition.Interactions;

namespace MathGame;

public partial class GamePage : ContentPage
{
	public string GameType {  get; set; }

	int firstNumber = 0;
	int secondNumber = 0;
	int score = 0;
    private int totalRounds;
    private int currentRound;

    public GamePage(string gameType, int rounds)
	{
		InitializeComponent();
		GameType = gameType;
        totalRounds = rounds; // Use dynamic rounds
		currentRound = 1;

        BindingContext = this;

		CreateNewQuestion();
	}

	private void CreateNewQuestion()
	{
		var gameOperand = GameType switch
		{
			"Addition" => "+",
			"Subtraction" => "-",
			"Multiplication" => "*",
			"Division" => "/",
			_ => ""
		};

		var random = new Random();

		firstNumber = GameType != "Division" ? random.Next(1, 9) : random.Next(1, 99);
		secondNumber = GameType != "Division" ? random.Next(1, 9) : random.Next(1, 99);

        if(GameType == "Division")
		{
			while(firstNumber < secondNumber || firstNumber % secondNumber != 0)
			{
				firstNumber = random.Next(1, 99);
				secondNumber = random.Next(1, 99);
			} 
		}

		QuestionLabel.Text = $"{firstNumber} {gameOperand} {secondNumber}";
    }

	private void OnAnswerSubmitted(object sender, EventArgs e)
	{
		var answer = Int32.Parse(AnswerEntry.Text);
		var isCorrect = false;

		switch (GameType)
		{
			case "Addition":
				isCorrect = answer == firstNumber + secondNumber; 
				break;
            case "Subtraction":
                isCorrect = answer == firstNumber - secondNumber;
                break;
            case "Multiplication":
                isCorrect = answer == firstNumber * secondNumber;
                break;
            case "Division":
                isCorrect = answer == firstNumber / secondNumber;
                break;
        }

		ProcessAnswer(isCorrect);
		currentRound++;

        // Check if we should continue or end the game
        if (currentRound <= totalRounds)
        {
            CreateNewQuestion();
            AnswerEntry.Text = string.Empty; // Clear the answer entry for the next question
        }
        else
        {
            GameOver();
        }
    }

	private void GameOver()
	{
		GameOperation gameOperation = GameType switch
		{
			"Addition" => GameOperation.Addition,
			"Subtraction" => GameOperation.Subtraction,
			"Multiplication" => GameOperation.Multiplication,
			"Division" => GameOperation.Division,
		};

		QuestionArea.IsVisible = false;
		BackToMenuBtn.IsVisible = true;
		GameOverLabel.Text = $"Game over! You got {score} out of {totalRounds} right";

		App.GameRepository.Add(new Game
		{
			DatePlayed = DateTime.UtcNow,
			Type = gameOperation,
			Score = score
		});
	}

	private void ProcessAnswer(bool isCorrect)
	{
		if (isCorrect)
			score ++;

		AnswerLabel.Text = isCorrect ? "Correct" : "Incorrect";
	}

	private void OnBackToMenu(object sender, EventArgs e)
	{
		Navigation.PushAsync(new MainPage());
	}
}