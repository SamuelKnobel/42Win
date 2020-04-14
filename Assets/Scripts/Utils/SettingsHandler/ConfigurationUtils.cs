using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides access to configuration data
/// </summary>
public static class ConfigurationUtils
{

    static ConfigurationData ConfigurationData;

    #region Properties

    /// <summary>
    /// Gets the paddle move units per second
    /// </summary>
    /// <value>paddle move units per second</value>
    public static int Height
    {
        get { return ConfigurationData.Height; }
    }
    public static int Width
    {
        get { return ConfigurationData.Width; }
    }

    public static void changeSettingValue(ConfigurationDataValueName valueName, float value)
    {
        bool b = ConfigurationData.values.TryGetValue(valueName, out float v);
        if (b)
        {
            ConfigurationData.values[valueName] = value;
            if (valueName == ConfigurationDataValueName.Difficulty)
            {
                ConfigurationData.Difficulty = (int)value;
            }
        }

    }

    #endregion

    /// <summary>
    /// Initializes the configuration utils
    /// </summary>
    public static void Initialize()
    {
        ConfigurationData = new ConfigurationData();
    }
}
