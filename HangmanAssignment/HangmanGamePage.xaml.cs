using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace HangmanAssignment
{
    public partial class HangmanGamePage : ContentPage
    {
        private readonly string[] _wordsToGuess = { "TYPESCRIPT", "PYTHON", "JAVASCRIPT" }; // The words to guess
        private int _currentWordIndex = 0; // Index to track which word is being guessed
        private char[] _currentGuess; // The current guessed word (with blanks)
        private int _wrongGuesses = 0; // Count of incorrect guesses
        private HashSet<char> _usedLetters = new(); // Track guessed letters

        public HangmanGamePage()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Reset game state for the current word
            string currentWord = _wordsToGuess[_currentWordIndex];
            _currentGuess = new string('_', currentWord.Length).ToCharArray();
            _wrongGuesses = 0;
            _usedLetters.Clear();

            // Update UI
            UpdateWordLabel();
            UpdateHangmanImage();
        }

        private void UpdateWordLabel()
        {
            // Update the word label to show guessed letters and blanks
            string displayWord = string.Join(" ", _currentGuess);
            var wordLabel = this.FindByName<Label>("WordLabel");
            wordLabel.Text = displayWord;
        }

        private void UpdateHangmanImage()
        {
            // Update the image based on the number of wrong guesses
            var hangmanImage = this.FindByName<Image>("HangmanImage");
            hangmanImage.Source = $"hang{_wrongGuesses + 1}.png"; // Assumes images are named hang1.png, hang2.png, etc.
        }

        private void OnGuessButtonClicked(object sender, EventArgs e)
        {
            var entry = this.FindByName<Entry>("GuessEntry");
            string userGuess = entry.Text?.Trim().ToUpper();

            // Validate input
            if (string.IsNullOrEmpty(userGuess) || userGuess.Length != 1)
            {
                DisplayAlert("Invalid Guess", "Please enter a single letter.", "OK");
                return;
            }

            char guessedChar = userGuess[0];

            // Check if the letter was already guessed
            if (_usedLetters.Contains(guessedChar))
            {
                DisplayAlert("Duplicate Guess", "You already guessed that letter.", "OK");
                return;
            }

            _usedLetters.Add(guessedChar);

            // Process the guess
            string currentWord = _wordsToGuess[_currentWordIndex];
            if (currentWord.Contains(guessedChar))
            {
                // Correct guess
                for (int i = 0; i < currentWord.Length; i++)
                {
                    if (currentWord[i] == guessedChar)
                    {
                        _currentGuess[i] = guessedChar;
                    }
                }

                UpdateWordLabel();

                // Check if the player has guessed the current word
                if (new string(_currentGuess) == currentWord)
                {
                    DisplayAlert("Congratulations!", $"You guessed the word: {currentWord}!", "Next Word");

                    // Move to the next word or restart if all words are guessed
                    _currentWordIndex++;
                    if (_currentWordIndex < _wordsToGuess.Length)
                    {
                        InitializeGame();
                    }
                    else
                    {
                        DisplayAlert("Congratulations!", "You guessed all the words and Survived!", "Play Again");
                        _currentWordIndex = 0; // Reset for a new game
                        InitializeGame();
                    }
                }
            }
            else
            {
                // Wrong guess
                _wrongGuesses++;
                UpdateHangmanImage();

                // Check if the player has lost
                if (_wrongGuesses == 6) // Assuming 6 is the max incorrect guesses
                {
                    DisplayAlert("Game Over, and You Died", $"The word was {currentWord}. Try again!", "Restart");
                    _currentWordIndex = 0; // Reset to the first word
                    InitializeGame();
                }
            }

            // Clear the input field
            entry.Text = string.Empty;
        }
    }
}
