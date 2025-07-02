using UnityEngine;


[CreateAssetMenu(fileName = "GameConfigSO", menuName = "Config/GameConfigSO")]
public class GameConfigSO : ScriptableObject
{
    [Header("In-game Configuration")]
    #region Init Game
    public readonly int initCardNum = 10;
    public readonly int minCard2Play = 2;
    public readonly int maxCard2Play = 4;
    public readonly float cardHolderSize = 1f;
    public readonly float cardSize = 1f;
    public readonly Vector2 handHolderPos = new Vector2(316, 85);
    public readonly Vector2 virtualHolderPos = new Vector2(316, -50f);
    public readonly Vector2 inGameLeftButtonPos = new Vector2(214f, 15f);
    public readonly Vector2 inGameRightButtonPos = new Vector2(429f, 15f);




    #endregion

    [Header("Card Configuration")]
    #region Card Infor
    // These rotate speed is depend on render mode of canvas.
    public float cardRotateSpeed { get; private set; }
    public readonly float overlayCardRotateSpeed = 100f;
    public readonly float cameraCardRotateSpeed = 10f;
    public readonly float worldCardRotateSpeed = 10f;
    public readonly float cardRotateAngle = 60f;
    public void SetCardRotateSpeed(float speed)
    {
        cardRotateSpeed = speed;
    }
    #endregion

}