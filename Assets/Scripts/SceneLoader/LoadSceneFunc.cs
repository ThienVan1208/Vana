using UnityEngine;

// Used thru buttons.
public class LoadSceneFunc : MonoBehaviour
{
    public void LoadSceneByIndex(int numScene)
    {
        LoadSceneHandler.LoadSceneByIndex(numScene);
    }
    public void LoadNextScene()
    {
        LoadSceneHandler.LoadNextScene();
    }
}
