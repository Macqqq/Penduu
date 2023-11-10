using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Linq;

namespace AlphabetButtons
{
    public partial class MainWindow : Window
    {
        private string secretWord; // Le mot secret à deviner
        private string currentGuess; // Le mot partiellement deviné
        private int mistakeCount; // Le nombre d'erreurs commises
        private List<string> hangmanImagesPaths = new List<string>()
        {
            "1.png",
            "2.png",
            "3.png",
            "4.png",
            "5.png",
            "6.png",
            "7.png"
        }; // Chemins des images du pendu
        private int currentImageIndex = 0; // Index de l'image actuellement affichée
        private DispatcherTimer timer; // Minuterie pour le compte à rebours
        private int remainingSeconds; // Temps restant en secondes

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer(); // Initialise la minuterie
            EnsureWordsJsonExists(); // Vérifie si le fichier JSON existe
            StartNewGame(); // Démarre une nouvelle partie
        }

        // Initialise la minuterie
        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
        }

        // Gère l'événement Tick de la minuterie
        private void TimerTick(object sender, EventArgs e)
        {
            remainingSeconds--;
            TimerLabel.Content = $"Temps restant : {remainingSeconds} secondes";

            // Vérifie si le temps est écoulé
            if (remainingSeconds <= 0)
            {
                timer.Stop();
                MessageBox.Show("Le temps est écoulé !");
                StartNewGame(); // Redémarre une nouvelle partie
            }
        }

        // Vérifie si le fichier JSON des mots existe, sinon le crée
        private void EnsureWordsJsonExists()
        {
            if (!WordManager.FileExists())
            {
                WordManager.CreateWordsJson();
            }
        }

        // Démarre une nouvelle partie
        private void StartNewGame()
        {
            ResetButtonStates(); // Réinitialise l'état des boutons de lettres
            secretWord = WordManager.ChooseRandomWord(); // Sélectionne un nouveau mot secret
            currentGuess = new string('_', secretWord.Length); // Initialise le mot partiellement deviné
            mistakeCount = 0; // Réinitialise le nombre d'erreurs
            currentImageIndex = 0; // Réinitialise l'index de l'image du pendu
            remainingSeconds = 30; // Réinitialise le compte à rebours à 30 secondes
            TimerLabel.Content = $"Temps restant : {remainingSeconds} secondes";
            EnableLetterButtons(true); // Active les boutons de lettres
            UpdateUI(); // Met à jour l'interface utilisateur
            timer.Start(); // Démarre la minuterie
        }

        // Met à jour l'interface utilisateur
        private void UpdateUI()
        {
            Textpendu.Text = currentGuess;

            if (currentImageIndex < hangmanImagesPaths.Count)
            {
                // Charge et affiche l'image du pendu correspondante
                string imagePath = $"C:\\Users\\SLAB60\\source\\repos\\WpfApp4\\WpfApp4\\Images\\{hangmanImagesPaths[currentImageIndex]}";
                BitmapImage imageSource = new BitmapImage(new Uri(imagePath));
                HangmanImage.Source = imageSource;
            }
        }

        // Gère le clic sur un bouton de lettre
        private void LetterButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string chosenLetter = button.Content.ToString();
                CheckLetter(chosenLetter); // Vérifie si la lettre choisie est correcte
                button.IsEnabled = false; // Désactive le bouton de lettre pour éviter les doublons
            }
        }

        // Active ou désactive tous les boutons de lettre
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

        // Réinitialise l'état des boutons de lettre
        public void ResetButtonStates()
        {
            List<Button> buttonsToReset = new List<Button>
            {
                ButtonA, ButtonB, ButtonC, ButtonD, ButtonE, ButtonF, ButtonG,
                ButtonH, ButtonI, ButtonJ, ButtonK, ButtonL, ButtonM, ButtonN,
                ButtonO, ButtonP, ButtonQ, ButtonR, ButtonS, ButtonT, ButtonU,
                ButtonV, ButtonW, ButtonX, ButtonY, ButtonZ
            };

            foreach (var button in buttonsToReset)
            {
                button.Content = button.Name.Substring(6);
            }
        }

        // Vérifie si la lettre choisie est correcte
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


        // Fonction pour afficher une lettre du mot secret et la révéler
        private void Indice()
        {
            // Vérifiez si le mot secret est vide ou s'il ne reste plus de lettres à deviner
            if (string.IsNullOrEmpty(secretWord) || !currentGuess.Contains('_'))
            {
                return; // Ne faites rien si le mot secret est déjà trouvé ou s'il est vide
            }

            // Trouvez la première lettre non révélée dans le mot secret
            for (int i = 0; i < secretWord.Length; i++)
            {
                if (secretWord[i] != '_' && currentGuess[i] == '_')
                {
                    // Remplacez le '_' par la lettre dans le TextBlock
                    currentGuess = currentGuess.Substring(0, i) + secretWord[i] + currentGuess.Substring(i + 1);

                    // Mettez à jour le TextBlock
                    Textpendu.Text = currentGuess;

                    // Mettez à jour l'interface utilisateur pour refléter les changements
                    UpdateUI();

                    break; // Sortez de la boucle une fois que la première lettre non révélée a été trouvée
                }
            }
        }


        // Gérez le clic sur un bouton pour obtenir un indice
        private void IndiceButton_Click(object sender, RoutedEventArgs e)
        {
            Indice(); // Appelez la fonction Indice lorsque le bouton d'indice est cliqué
        }
    }
}
