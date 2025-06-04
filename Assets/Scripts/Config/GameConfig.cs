using UnityEngine;

public class GameConfig : MonoBehaviour
{
    [SerializeField] private GameConfigSO _gameConfigSO;
    [SerializeField] private Canvas _playableCanvas;
    private void Awake()
    {
        switch (_playableCanvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                _gameConfigSO.SetCardRotateSpeed(_gameConfigSO.overlayCardRotateSpeed);
                break;
            case RenderMode.ScreenSpaceCamera:
                _gameConfigSO.SetCardRotateSpeed(_gameConfigSO.cameraCardRotateSpeed);
                break;
            
            // Currently this option is chosen.
            default:
                _gameConfigSO.SetCardRotateSpeed(_gameConfigSO.worldCardRotateSpeed);
                break;
        }
    }
}
