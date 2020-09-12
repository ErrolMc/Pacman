using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HighScoreContainer
{
    const string PREFS_KEY = "HighScores";
    const int MAX_HIGH_SCORES = 5;

    public List<HighScore> highscores;

    public HighScoreContainer()
    {
        highscores = new List<HighScore>();
    }

    public List<HighScore> GetSortedScores()
    {
        highscores.Sort(CompareScores);
        return highscores;
    }

    public void AddHighScore(HighScore newScore)
    {
        if (highscores.Count < MAX_HIGH_SCORES)
            highscores.Add(newScore);
        else if (CanAddScore(newScore.score))
        {
            RemoveLowestScore();
            highscores.Add(newScore);
        }
    }

    void RemoveLowestScore()
    {
        if (highscores.Count > 0)
        {
            int ind = 0;
            int lowest = highscores[0].score;
            for (int i = 1; i < highscores.Count; i++)
            {
                int cur = highscores[i].score;
                if (cur < lowest)
                {
                    lowest = cur;
                    ind = i;
                }
            }

            highscores.RemoveAt(ind);
        }
    }

    public bool CanAddScore(int score)
    {
        if (highscores.Count < MAX_HIGH_SCORES)
            return true;

        for (int i = 0; i < highscores.Count; i++)
        {
            if (score > highscores[i].score)
                return true;
        }
        return false;
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(PREFS_KEY, json);
    }

    public static HighScoreContainer Load()
    {
        if (PlayerPrefs.HasKey(PREFS_KEY))
        {
            string json = PlayerPrefs.GetString(PREFS_KEY);
            return JsonUtility.FromJson<HighScoreContainer>(json);
        }
        
        return new HighScoreContainer();
    }

    static int CompareScores(HighScore hs1, HighScore hs2)
    {
        if (hs1.score > hs2.score)
            return -1;
        return hs1.score == hs2.score ? 0 : 1;
    }
}
