using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Threading;

namespace AlphabetButtons
{
    public partial class MainWindow : Window
    {
        private string secretWord;
        private string currentGuess;
        private int mistakeCount;
        private List<string> hangmanImagesPaths = new List<string>()
    {
        "1.png",
        "2.png",
        "3.png",
        "4.png",
        "5.png",
        "6.png",
        "7.png"
    };
        private int currentImageIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            EnsureWordsJsonExists();
            StartNewGame();
        }

        private void EnsureWordsJsonExists()
        {
            if (!WordManager.FileExists())
            {
                WordManager.CreateWordsJson();
            }
        }

        private void StartNewGame()
        {
            secretWord = WordManager.ChooseRandomWord();
            currentGuess = new string('_', secretWord.Length);
            mistakeCount = 0;
            currentImageIndex = 0; // Réinitialisez l'index de l'image à zéro

            EnableLetterButtons(true);
            UpdateUI();
        }

        private void UpdateUI()
        {
            Textpendu.Text = currentGuess;

            // Vérifiez si vous avez atteint le nombre maximum d'images disponibles
            if (currentImageIndex < hangmanImagesPaths.Count)
            {
                string imagePath = $"C:\\Users\\SLAB60\\source\\repos\\WpfApp4\\WpfApp4\\Images\\{hangmanImagesPaths[currentImageIndex]}";
                BitmapImage imageSource = new BitmapImage(new Uri(imagePath));
                HangmanImage.Source = imageSource;
            }
        }

        private void LetterButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string chosenLetter = button.Content.ToString();
                CheckLetter(chosenLetter);
                button.IsEnabled = false;
            }
        }

        private void EnableLetterButtons(bool enable)
        {
            Grid letterGrid = this.Content as Grid;
            if (letterGrid != null)
            {
                foreach (var child in letterGrid.Children)
                {
                    if (child is Grid innerGrid)
                    {
                        foreach (var buttonChild in innerGrid.Children)
                        {
                            if (buttonChild is Button button && button.Name.StartsWith("Button"))
                            {
                                button.IsEnabled = enable;
                            }
                        }
                    }
                }
            }
        }

        private void CheckLetter(string letter)
        {
            bool letterFound = false;
            char[] currentGuessArray = currentGuess.ToCharArray();

            for (int i = 0; i < secretWord.Length; i++)
            {
                if (secretWord[i].ToString() == letter.ToLower())
                {
                    currentGuessArray[i] = secretWord[i];
                    letterFound = true;
                }
            }

            if (!letterFound)
            {
                mistakeCount++;

                // Incrémente l'index de l'image si vous n'avez pas atteint le maximum
                if (currentImageIndex < hangmanImagesPaths.Count)
                {
                    currentImageIndex++;
                }

                if (mistakeCount >= 7)
                {
                    MessageBox.Show($"Désolé, vous avez perdu ! Le mot était : {secretWord}");
                    StartNewGame();
                    return;
                }
            }


            currentGuess = new string(currentGuessArray);
            UpdateUI();

            if (!currentGuess.Contains("_"))
            {
                MessageBox.Show("Félicitations, vous avez deviné le mot !");
                StartNewGame();
            }
        }
    }
}