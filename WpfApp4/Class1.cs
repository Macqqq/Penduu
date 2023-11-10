using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;

public static class WordManager
{
    private const string FilePath = "words.json"; // Chemin du fichier JSON pour enregistrer les mots

    public static bool FileExists()
    {
        return File.Exists(FilePath); // Vérifie si le fichier JSON existe
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

        for (int i = 0; i < 5; i++) // Sélectionner 5 mots au hasard
        {
            int index = random.Next(words.Count);
            randomWords.Add(words[index]);
            words.RemoveAt(index);
        }

        var wordsObject = new { words = randomWords.ToArray() }; // Crée un objet JSON avec les mots sélectionnés
        string jsonText = JsonConvert.SerializeObject(wordsObject); // Convertit l'objet en texte JSON

        File.WriteAllText(FilePath, jsonText); // Enregistre le texte JSON dans le fichier
    }

    public static string ChooseRandomWord()
    {
        string jsonText = File.ReadAllText(FilePath); // Lit le contenu du fichier JSON
        var wordsObject = JsonConvert.DeserializeObject<dynamic>(jsonText); // Désérialise le JSON en un objet dynamique
        string[] words = wordsObject.words.ToObject<string[]>(); // Obtient la liste des mots à partir de l'objet JSON

        Random random = new Random();
        return words[random.Next(words.Length)]; // Retourne un mot choisi au hasard parmi la liste
    }
}
