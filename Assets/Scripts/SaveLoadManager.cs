using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savedData.json");
    }

    public void SaveGame(GameManager gameManager, CardManager cardManager)
    {
        SaveData saveData = new SaveData
        {
            savedScore = gameManager.score,
            savedStreak = gameManager.streak,
            savedCardCounter = gameManager.cardCounter,
            savedFirstCard = gameManager.firstCard,
            savedSecondCard = gameManager.secondCard
        };

        foreach (Card card in cardManager.cards)
        {
            CardData cardData = new CardData
            {
                cardId = card.transform.GetSiblingIndex(), // Use the sibling index as the ID
                cardType = card.cardType,
                isFlipped = card.cardState == CardState.Flipped
            };
            saveData.savedCards.Add(cardData);
        }

        // Save the shuffled order of cards
        saveData.savedShuffledOrder = cardManager.currentShuffledOrder;

        // Convert saveData to JSON
        string json = JsonUtility.ToJson(saveData, true);

        // Write JSON to file
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved: " + saveFilePath);
    }

    public SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            return saveData;
        }

        Debug.Log("No save file found.");
        return null;
    }

    public void DeleteSaveFile()
    {
        // Check if the save file exists, and delete it as we will start a fresh game with no need to load an old save
        if (File.Exists(saveFilePath))
        {
            Debug.Log("Deleting save file");
            File.Delete(saveFilePath);
        }
    }
}
