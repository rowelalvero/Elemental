using System;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressionManager : MonoBehaviour
{
    public static GameProgressionManager Instance;

    [Header("Progression Data")]
    private Dictionary<int, Achievement> achievements = new Dictionary<int, Achievement>(); // ID -> Achievement
    private Dictionary<int, Quest> quests = new Dictionary<int, Quest>(); // ID -> Quest
    private Dictionary<int, Goal> goals = new Dictionary<int, Goal>(); // ID -> Goal

    private void Awake()
    {
        // Ensure only one instance of the GameProgressionManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Preserve this manager across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    // Method to add an achievement (or check if it exists)
    public void AddAchievement(int id, string name)
    {
        if (!achievements.ContainsKey(id))
        {
            Achievement newAchievement = new Achievement(id, name);
            achievements.Add(id, newAchievement);
            Debug.Log($"Achievement Added: {newAchievement.Name}");
        }
        else
        {
            Debug.Log($"Achievement with ID {id} already exists.");
        }
    }

    // Method to add a quest (or check if it exists)
    public void AddQuest(int id, string questName)
    {
        if (!quests.ContainsKey(id))
        {
            Quest newQuest = new Quest(id, questName);
            quests.Add(id, newQuest);
            Debug.Log($"Quest Added: {newQuest.Name}");
        }
        else
        {
            Debug.Log($"Quest with ID {id} already exists.");
        }
    }

    // Method to add a goal (or check if it exists)
    public void AddGoal(int id, string goalName)
    {
        if (!goals.ContainsKey(id))
        {
            Goal newGoal = new Goal(id, goalName);
            goals.Add(id, newGoal);
            Debug.Log($"Goal Added: {newGoal.Name}");
        }
        else
        {
            Debug.Log($"Goal with ID {id} already exists.");
        }
    }

    // Handle the input of script, function name, and id
    public void HandleProgression(int id, string scriptName, string functionName)
    {
        // You can extend this to look for different scripts and functions dynamically.
        // For simplicity, we'll call methods on this script for now.

        if (scriptName.Equals("Achievement"))
        {
            AddAchievement(id, functionName);  // Example: functionName is the achievement name
        }
        else if (scriptName.Equals("Quest"))
        {
            AddQuest(id, functionName); // Example: functionName is the quest name
        }
        else if (scriptName.Equals("Goal"))
        {
            AddGoal(id, functionName); // Example: functionName is the goal name
        }
        else
        {
            Debug.LogWarning($"No handler for {scriptName}");
        }
    }

    // Method to check if an achievement exists
    public bool IsAchievementExist(int id)
    {
        return achievements.ContainsKey(id);
    }

    // Method to check if a quest exists
    public bool IsQuestExist(int id)
    {
        return quests.ContainsKey(id);
    }

    // Method to check if a goal exists
    public bool IsGoalExist(int id)
    {
        return goals.ContainsKey(id);
    }

    // You can add saving and loading logic if necessary here.
    public void SaveProgress()
    {
        // Example: Save data using PlayerPrefs or other methods
        PlayerPrefs.SetString("Achievements", SerializeAchievements());
        PlayerPrefs.SetString("Goals", SerializeGoals());
        PlayerPrefs.SetString("Quests", SerializeQuests());
        PlayerPrefs.Save();
        Debug.Log("Progress saved!");
    }

    public void LoadProgress()
    {
        // Example: Load data using PlayerPrefs or other methods
        DeserializeAchievements(PlayerPrefs.GetString("Achievements"));
        DeserializeGoals(PlayerPrefs.GetString("Goals"));
        DeserializeQuests(PlayerPrefs.GetString("Quests"));
        Debug.Log("Progress loaded!");
    }

    private string SerializeAchievements()
    {
        // Serialize achievements to a simple string (you can improve this serialization)
        List<string> achievementNames = new List<string>();
        foreach (var achievement in achievements.Values)
        {
            achievementNames.Add(achievement.Name);
        }
        return string.Join(",", achievementNames);
    }

    private string SerializeGoals()
    {
        List<string> goalNames = new List<string>();
        foreach (var goal in goals.Values)
        {
            goalNames.Add(goal.Name);
        }
        return string.Join(",", goalNames);
    }

    private string SerializeQuests()
    {
        List<string> questNames = new List<string>();
        foreach (var quest in quests.Values)
        {
            questNames.Add(quest.Name);
        }
        return string.Join(",", questNames);
    }

    private void DeserializeAchievements(string data)
    {
        string[] achievementNames = data.Split(',');
        foreach (var name in achievementNames)
        {
            // You could create an ID system or unique identifiers to deserialize more advanced data.
            AddAchievement(achievements.Count + 1, name);
        }
    }

    private void DeserializeGoals(string data)
    {
        string[] goalNames = data.Split(',');
        foreach (var name in goalNames)
        {
            AddGoal(goals.Count + 1, name);
        }
    }

    private void DeserializeQuests(string data)
    {
        string[] questNames = data.Split(',');
        foreach (var name in questNames)
        {
            AddQuest(quests.Count + 1, name);
        }
    }
}

// Achievement class to represent an achievement
[System.Serializable]
public class Achievement
{
    public int ID;
    public string Name;

    public Achievement(int id, string name)
    {
        ID = id;
        Name = name;
    }
}

// Quest class to represent a quest
[System.Serializable]
public class Quest
{
    public int ID;
    public string Name;

    public Quest(int id, string name)
    {
        ID = id;
        Name = name;
    }
}

// Goal class to represent a goal
[System.Serializable]
public class Goal
{
    public int ID;
    public string Name;

    public Goal(int id, string name)
    {
        ID = id;
        Name = name;
    }
}
