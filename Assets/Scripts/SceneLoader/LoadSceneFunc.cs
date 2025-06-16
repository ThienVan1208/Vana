using UnityEngine;

// Used thru buttons.
public class LoadSceneFunc : MonoBehaviour
{
    public int numScene;
    public void LoadNextScene()
    {
        LoadSceneHandler.LoadSceneByIndex(numScene);
    }
}
