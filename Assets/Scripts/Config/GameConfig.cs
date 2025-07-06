using UnityEngine;

public class GameConfig : MonoBehaviour
{
    [SerializeField] private Canvas _playableCanvas;
    private void Awake()
    {
        switch (_playableCanvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                GameConfiguration.SetCardRotateSpeed(GameConfiguration.overlayCardRotateSpeed);
                break;
            case RenderMode.ScreenSpaceCamera:
                GameConfiguration.SetCardRotateSpeed(GameConfiguration.cameraCardRotateSpeed);
                break;
            
            // Currently this option is chosen.
            default:
                GameConfiguration.SetCardRotateSpeed(GameConfiguration.worldCardRotateSpeed);
                break;
        }
    }
}
