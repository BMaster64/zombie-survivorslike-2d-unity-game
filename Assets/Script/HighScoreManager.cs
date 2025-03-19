using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Get the high score for a specific stage
    public int GetHighScore(string stageName)
    {
        return PlayerPrefs.GetInt("HighScore_" + stageName, 0);
    }

    // Save a new high score for a specific stage
    public bool SaveHighScore(string stageName, int score)
    {
        int currentHighScore = GetHighScore(stageName);

        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore_" + stageName, score);
            PlayerPrefs.Save();
            return true; // New high score achieved
        }

        return false; // No new high score
    }

    // Get the highest level reached on a specific stage
    public int GetHighestLevel(string stageName)
    {
        return PlayerPrefs.GetInt("HighLevel_" + stageName, 0);
    }

    // Save the highest level reached on a specific stage
    public bool SaveHighestLevel(string stageName, int level)
    {
        int currentHighLevel = GetHighestLevel(stageName);

        if (level > currentHighLevel)
        {
            PlayerPrefs.SetInt("HighLevel_" + stageName, level);
            PlayerPrefs.Save();
            return true; // New high level achieved
        }

        return false; // No new high level
    }

    // Get the fastest completion time for a specific stage (in seconds)
    public float GetBestTime(string stageName)
    {
        return PlayerPrefs.GetFloat("BestTime_" + stageName, float.MaxValue);
    }

    // Save the fastest completion time for a specific stage
    public bool SaveBestTime(string stageName, float timeInSeconds)
    {
        float currentBestTime = GetBestTime(stageName);

        // Only save if this is a faster time (and not zero, which would mean no completion)
        if (timeInSeconds > 0 && timeInSeconds < currentBestTime)
        {
            PlayerPrefs.SetFloat("BestTime_" + stageName, timeInSeconds);
            PlayerPrefs.Save();
            return true; // New best time achieved
        }

        return false; // No new best time
    }

    // Reset all high scores for all stages
    public void ResetAllHighScores()
    {
        // Get all keys in PlayerPrefs that start with "HighScore_"
        List<string> keysToDelete = new List<string>();

        foreach (string key in GetAllKeys())
        {
            if (key.StartsWith("HighScore_") || key.StartsWith("HighLevel_") || key.StartsWith("BestTime_"))
            {
                keysToDelete.Add(key);
            }
        }

        // Delete all matching keys
        foreach (string key in keysToDelete)
        {
            PlayerPrefs.DeleteKey(key);
        }

        PlayerPrefs.Save();
    }

    // Get all keys in PlayerPrefs (helper method)
    private List<string> GetAllKeys()
    {
        // This is a workaround as Unity doesn't provide a direct way to get all keys
        // You might want to maintain a list of your keys in a more robust implementation
        List<string> knownKeys = new List<string>();

        // Add known stage names here
        string[] stageNames = { "Stage01", "Stage02", "Stage03" }; // Add more as needed

        foreach (string stage in stageNames)
        {
            knownKeys.Add("HighScore_" + stage);
            knownKeys.Add("HighLevel_" + stage);
            knownKeys.Add("BestTime_" + stage);
        }

        return knownKeys;
    }
}