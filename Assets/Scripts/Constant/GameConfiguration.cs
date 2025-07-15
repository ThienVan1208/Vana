using UnityEngine;

public static class GameConfiguration
{
    #region Init Game
    public static readonly int initCardNum = 10;
    public static readonly int minCard2Play = 2;
    public static readonly int maxCard2Play = 4;
    public static readonly float cardHolderSize = 1f;
    public static readonly float cardSize = 1f;
    public static readonly int maxCardDrawNum = 3;
    public static readonly Vector2 handHolderPos = new Vector2(316, 85);
    public static readonly Vector2 playerButtonPanelPos = new Vector2(316, 20);
    public static readonly Vector2 virtualHolderPos = new Vector2(316, -50f);
    #endregion




    #region Card Infor
    // These rotate speed is depend on render mode of canvas.
    public static float cardRotateSpeed { get; private set; }
    public static readonly float overlayCardRotateSpeed = 100f;
    public static readonly float cameraCardRotateSpeed = 10f;
    public static readonly float worldCardRotateSpeed = 10f;
    public static readonly float cardRotateAngle = 60f;
    public static void SetCardRotateSpeed(float speed)
    {
        cardRotateSpeed = speed;
    }

    public static int CardCountThreshold = 21;
    #endregion
}
