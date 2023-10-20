﻿using System.Linq;

public class HangmanGame
{
    public string SecretWord { get; private set; }
    public string CurrentGuess { get; private set; }
    public int MistakeCount { get; private set; }
    public bool IsGameOver => MistakeCount >= 5 || !CurrentGuess.Contains('_');

    public HangmanGame()
    {
        ResetGame();
    }

    public void ResetGame()
    {
        SecretWord = WordManager.ChooseRandomWord();
        CurrentGuess = new string('_', SecretWord.Length);
        MistakeCount = 0;
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
            MistakeCount++;
        }

        CurrentGuess = new string(currentGuessArray);

        if (IsGameOver)
        {
            return CurrentGuess.Contains('_') ? GameResult.Lost : GameResult.Won;
        }
        return letterFound ? GameResult.CorrectGuess : GameResult.WrongGuess;
    }
}

public enum GameResult
{
    CorrectGuess,
    WrongGuess,
    Won,
    Lost
}