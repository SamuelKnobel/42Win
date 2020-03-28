using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides screen utilities
/// </summary>
public static class ScreenUtils
{
    #region Fields

    // cached for efficient boundary checking
    static float screenLeft;
    static float screenRight;
    static float screenTop;
    static float screenBottom;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the left edge of the screen in world coordinates
    /// </summary>
    /// <value>left edge of the screen</value>
    public static float ScreenLeft
    {
        get { return screenLeft; }
    }

    /// <summary>
    /// Gets the right edge of the screen in world coordinates
    /// </summary>
    /// <value>right edge of the screen</value>
    public static float ScreenRight
    {
        get { return screenRight; }
    }

    /// <summary>
    /// Gets the top edge of the screen in world coordinates
    /// </summary>
    /// <value>top edge of the screen</value>
    public static float ScreenTop
    {
        get { return screenTop; }
    }

    /// <summary>
    /// Gets the bottom edge of the screen in world coordinates
    /// </summary>
    /// <value>bottom edge of the screen</value>
    public static float ScreenBottom
    {
        get { return screenBottom; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the screen utilities
    /// </summary>
    public static void Initialize()
    {
        // save screen edges in world coordinates
        float screenZ = -Camera.main.transform.position.z;
        Vector3 lowerLeftCornerScreen = new Vector3(0, 0, screenZ);
        Vector3 upperRightCornerScreen = new Vector3(
            Screen.width, Screen.height, screenZ);
        Vector3 lowerLeftCornerWorld =
            Camera.main.ScreenToWorldPoint(lowerLeftCornerScreen);
        Vector3 upperRightCornerWorld =
            Camera.main.ScreenToWorldPoint(upperRightCornerScreen);
        screenLeft = lowerLeftCornerWorld.x;
        screenRight = upperRightCornerWorld.x;
        screenTop = upperRightCornerWorld.y;
        screenBottom = lowerLeftCornerWorld.y;
    }
    
    public static void addWalls()
    {
        EdgeCollider2D Walls = Camera.main.gameObject.GetComponent<EdgeCollider2D>();
        if (Walls == null)
        {
            Camera.main.gameObject.AddComponent<EdgeCollider2D>();
             Walls = Camera.main.gameObject.GetComponent<EdgeCollider2D>();
        }

        Walls.sharedMaterial = new PhysicsMaterial2D();
        Walls.sharedMaterial.bounciness = 1;
        Walls.sharedMaterial.friction = 0;
        Vector2[] colliderpoints = new Vector2[4];
         colliderpoints[0] = new Vector2(ScreenLeft,ScreenBottom);
         colliderpoints[1] = new Vector2(ScreenLeft,ScreenTop);
        colliderpoints[2] = new Vector2(ScreenRight,ScreenTop);
        colliderpoints[3] = new Vector2(ScreenRight, ScreenBottom);
        Walls.points = colliderpoints;

    }

    #endregion
}
