using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// A container for the configuration data
/// </summary>
public class ConfigurationData
{
    #region Fields

    public Dictionary<ConfigurationDataValueName, float> values =
    new Dictionary<ConfigurationDataValueName, float>();

    #endregion

    #region Properties

    public int Height
    {
        get { return (int)values[ConfigurationDataValueName.Height]; }
    }

    public int Width
    {
        get { return (int)values[ConfigurationDataValueName.Width]; }
    }
    int difficulty;
    public int Difficulty
    {
        set
        {
            difficulty = value;
            if (difficulty == 0)
            {
                GameManager.currentAI_ThinkDepth = (int)values[ConfigurationDataValueName.AIDepthEasy];
                GameManager.currentAI_ThinkTime = (int)values[ConfigurationDataValueName.AIThinkTimeEasy];
                GameManager.currentPlayer_ThinkTime = (int)values[ConfigurationDataValueName.PlayerThinkTimeEasy];
            }
            else if (difficulty == 1)
            {
                GameManager.currentAI_ThinkDepth = (int)values[ConfigurationDataValueName.AIDepthMedium];
                GameManager.currentAI_ThinkTime = (int)values[ConfigurationDataValueName.AIThinkTimeMedium];
                GameManager.currentPlayer_ThinkTime = (int)values[ConfigurationDataValueName.PlayerThinkTimeHard];
            }
            else if (difficulty == 2)
            {
                GameManager.currentAI_ThinkDepth = (int)values[ConfigurationDataValueName.AIDepthHard];
                GameManager.currentAI_ThinkTime = (int)values[ConfigurationDataValueName.AIThinkTimeHard];
                GameManager.currentPlayer_ThinkTime = (int)values[ConfigurationDataValueName.PlayerThinkTimeHard];
            }
        }
        get
        {
            return (int)values[ConfigurationDataValueName.Difficulty];
        }

    }


    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// Reads configuration data from a file. If the file
    /// read fails, the object contains default values for
    /// the configuration data
    /// </summary>
    public ConfigurationData()
    {
        SetDefaultValues();
    }


    /// <summary>
    /// Sets the configuration data fields to default values
    /// csv string
    /// </summary>
    void SetDefaultValues()
    {
        values.Clear();
        values.Add(ConfigurationDataValueName.Height, 6);
        values.Add(ConfigurationDataValueName.Width, 9);
        values.Add(ConfigurationDataValueName.Difficulty,0);
        values.Add(ConfigurationDataValueName.AIThinkTimeEasy,.01f);
        values.Add(ConfigurationDataValueName.AIThinkTimeMedium,.1f);
        values.Add(ConfigurationDataValueName.AIThinkTimeHard,1f);
        values.Add(ConfigurationDataValueName.PlayerThinkTimeEasy,20f);
        values.Add(ConfigurationDataValueName.PlayerThinkTimeMedium,10f);
        values.Add(ConfigurationDataValueName.PlayerThinkTimeHard,5f);
        values.Add(ConfigurationDataValueName.AIDepthEasy,10f);
        values.Add(ConfigurationDataValueName.AIDepthMedium,20f);
        values.Add(ConfigurationDataValueName.AIDepthHard,40f);

    }

    #endregion
}
