using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;
using AlphabetButtons;

public class HangmanGame
{
    private MainWindow mainWindow; // Référence à la fenêtre principale

    public string SecretWord { get; private set; } // Mot secret à deviner
    public string CurrentGuess { get; private set; } // Mot partiellement deviné
    public int MistakeCount { get; private set; } // Nombre d'erreurs
    public bool IsGameOver => MistakeCount >= 5 || !CurrentGuess.Contains('_'); // Indique si la partie est terminée

    public HangmanGame(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
        ResetGame(); // Initialise une nouvelle partie
    }

    public void ResetGame()
    {
        SecretWord = WordManager.ChooseRandomWord(); // Sélectionne un nouveau mot secret
        CurrentGuess = new string('_', SecretWord.Length); // Initialise le mot partiellement deviné
        MistakeCount = 0; // Réinitialise le nombre d'erreurs
    }

    public GameResult CheckLetter(string letter)
    {
        bool letterFound = false;
        char[] currentGuessArray = CurrentGuess.ToCharArray();

        for (int i = 0; i < SecretWord.Length; i++)
        {
            if (SecretWord[i].ToString() == letter.ToLower())
            {
                currentGuessArray[i] = SecretWord[i];
                letterFound = true;
            }
        }

        if (!letterFound)
        {
            MistakeCount++; // Incrémente le nombre d'erreurs si la lettre n'a pas été trouvée
        }

        CurrentGuess = new string(currentGuessArray);

        if (IsGameOver)
        {
            // Indique le résultat de la partie (gagnée, perdue ou en cours)
            return CurrentGuess.Contains('_') ? GameResult.Lost : GameResult.Won;
        }

        return letterFound ? GameResult.CorrectGuess : GameResult.WrongGuess;
    }
}

public enum GameResult
{
    CorrectGuess, // La lettre choisie était correcte
    WrongGuess,   // La lettre choisie était incorrecte
    Won,          // Le joueur a gagné la partie
    Lost          // Le joueur a perdu la partie
}

