using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that acts as the database for the highscores
/// </summary>
public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager instance;

    [SerializeField] int maxHighScores = 5;

    List<HighScore> highscores;

    void Awake()
    {
        instance = this;
        highscores = Load();
    }

    /// <summary>
    /// Returns a sorted list of the highscores
    /// </summary>
    /// <returns>A sorted list of the highscores</returns>
    public List<HighScore> GetSortedScores()
    {
        highscores.Sort(CompareScores);
        return highscores;
    }

    /// <summary>
    /// Adds a new highscore
    /// </summary>
    /// <param name="name">The name to go with the score</param>
    /// <param name="score">The score</param>
    public void AddHighScore(string name, int score)
    {
        HighScore newScore = new HighScore(name, score);

        if (highscores.Count < maxHighScores)
            highscores.Add(newScore);
        else if (CanAddScore(newScore.score))
        {
            RemoveLowestScore();
            highscores.Add(newScore);
        }

        Save();
    }

    /// <summary>
    /// Checks if a score can be added into the current list of highscores
    /// </summary>
    /// <param name="score">The score to query for</param>
    /// <returns>If the score can be added into the highscore list</returns>
    public bool CanAddScore(int score)
    {
        if (highscores.Count < maxHighScores)
            return true;

        for (int i = 0; i < highscores.Count; i++)
        {
            if (score > highscores[i].score)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Removes the lowest score from the highscore list
    /// </summary>
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

    /// <summary>
    /// Saves the highscores to disk
    /// </summary>
    void Save()
    {
        string json = JsonUtility.ToJson(new HighScoreContainer(highscores));
        PlayerPrefs.SetString("HighScores", json);
    }

    /// <summary>
    /// Loads the highscores from disk
    /// </summary>
    /// <returns>The highscore list that was stored on disk</returns>
    List<HighScore> Load()
    {
        if (PlayerPrefs.HasKey("HighScores"))
        {
            string json = PlayerPrefs.GetString("HighScores");
            return JsonUtility.FromJson<HighScoreContainer>(json).highScores;
        }

        return new List<HighScore>();
    }

    // Comparer function for sorting the highscore list
    static int CompareScores(HighScore hs1, HighScore hs2)
    {
        if (hs1.score > hs2.score)
            return -1;
        return hs1.score == hs2.score ? 0 : 1;
    }
}

/// <summary>
/// Serializable container for the highscores so we can save them to disk
/// </summary>
[System.Serializable]
class HighScoreContainer
{
    public List<HighScore> highScores;

    public HighScoreContainer(List<HighScore> highScores)
    {
        this.highScores = highScores;
    }
}