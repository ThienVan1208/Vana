using UnityEngine;


[CreateAssetMenu(fileName = "GameConfigSO", menuName = "Config/GameConfigSO")]
public class GameConfigSO : ScriptableObject
{
    #region Init Game
    public readonly int initCardNum = 10;
    public readonly int minCard2Play = 2;
    public readonly int maxCard2Play = 4;
    public readonly float cardHolderSize = 1f;
    public readonly float cardSize = 1f;
    #endregion

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