using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
public static class WordManager
{
    private const string FilePath = "words.json";

    public static bool FileExists()
    {
        return File.Exists(FilePath);
    }

    public static void CreateWordsJson()
    {
        var words = new List<string>
            {
                "pomme", "raisin", "citron", "melon", "peche",
                "chat", "chien", "oiseau", "cheval", "ours",
                "maison", "ecole", "jardin", "pont", "ville"
            };

        Random random = new Random();
        var randomWords = new List<string>();

        for (int i = 0; i < 5; i++)  // Sélectionner 5 mots
        {
            int index = random.Next(words.Count);
            randomWords.Add(words[index]);
            words.RemoveAt(index);
        }

        var wordsObject = new { words = randomWords.ToArray() };
        string jsonText = JsonConvert.SerializeObject(wordsObject);

        File.WriteAllText(FilePath, jsonText);
    }

    public static string ChooseRandomWord()
    {
        string jsonText = File.ReadAllText(FilePath);
        var wordsObject = JsonConvert.DeserializeObject<dynamic>(jsonText);
        string[] words = wordsObject.words.ToObject<string[]>();

        Random random = new Random();
        return words[random.Next(words.Length)];
    }
}

