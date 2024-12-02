using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace HangmanAssignment
{
    public partial class HangmanGamePage : ContentPage
    {
        private string _wordToGuess = "CORRECTGUESS"; // The word to guess
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
            // Reset game state
            _currentGuess = new string('_', _wordToGuess.Length).ToCharArray();
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
            if (_wordToGuess.Contains(guessedChar))
            {
                // Correct guess
                for (int i = 0; i < _wordToGuess.Length; i++)
                {
                    if (_wordToGuess[i] == guessedChar)
                    {
                        _currentGuess[i] = guessedChar;
                    }
                }

                UpdateWordLabel();

                // Check if the player has won
                if (new string(_currentGuess) == _wordToGuess)
                {
                    DisplayAlert("Congratulations!", "You guessed the word!", "Play Again");
                    InitializeGame();
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
                    DisplayAlert("Game Over", $"The word was {_wordToGuess}. Try again!", "Restart");
                    InitializeGame();
                }
            }

            // Clear the input field
            entry.Text = string.Empty;
        }
    }
}
