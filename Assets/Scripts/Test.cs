using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Test : MonoBehaviour
{
    public int numScene;
    public void TestUI()
    {
        LoadSceneHandler.LoadSceneByIndex(numScene);
    }
    public void testTimeScale()
    {
        Time.timeScale = Time.timeScale == 0 ? 1: 0;
    }

    
}
