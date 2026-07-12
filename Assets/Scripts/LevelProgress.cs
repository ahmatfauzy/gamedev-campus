using UnityEngine;

public static class LevelProgress
{
    private const string COMPLETED_PREFIX = "Level_Completed_";
    private const string UNLOCKED_PREFIX = "Level_Unlocked_";

    public static bool IsLevelCompleted(string levelName)
    {
        return PlayerPrefs.GetInt(COMPLETED_PREFIX + levelName, 0) == 1;
    }

    public static void SetLevelCompleted(string levelName)
    {
        PlayerPrefs.SetInt(COMPLETED_PREFIX + levelName, 1);
        PlayerPrefs.Save();
    }

    public static bool IsLevelUnlocked(string levelName)
    {
        return PlayerPrefs.GetInt(UNLOCKED_PREFIX + levelName, 0) == 1;
    }

    public static void UnlockLevel(string levelName)
    {
        PlayerPrefs.SetInt(UNLOCKED_PREFIX + levelName, 1);
        PlayerPrefs.Save();
    }

    public static string GetNextLevelName(string currentLevel)
    {
        switch (currentLevel)
        {
            case "Level1": return "Level2";
            default: return null;
        }
    }
}
