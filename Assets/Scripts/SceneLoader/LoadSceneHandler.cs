using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneHandler : MonoBehaviour
{
    private SceneLoadBase _sceneLoader;
    [SerializeField] private Camera _cam;
    public static string SceneLoaderPath = "SceneLoader";
    private static int _nextSceneIndex = 0;
    private static int _prevSceneIndex = 0;

    [System.Obsolete]
    private void Awake()
    {
        _sceneLoader = transform.GetChild(0).GetComponent<SceneLoadBase>();
        _sceneLoader.InitLoader();
        _cam.gameObject.SetActive(false);
    }

    // Used this method to load sceneLoader, then the start method will load next scene.
    public static void LoadSceneByIndex(int index)
    {
        _prevSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _nextSceneIndex = index;
        SceneManager.LoadScene(SceneLoaderPath, LoadSceneMode.Additive);
    }

    private async void Start()
    {
        await HelpLoadingScene();
    }
    private async UniTask HelpLoadingScene()
    {
        try
        {
            // Start loading effect.
            _sceneLoader.StartLoading();
            await UniTask.Delay((int)(_sceneLoader.GetEffectTime() * 1000f));

            // Time.timeScale = 0;

            // Undload previous scene.
            await SceneManager.UnloadSceneAsync(_prevSceneIndex);
            _cam.gameObject.SetActive(true);

            // Load new scene with LoadSceneMode.Additive -> the new scene is not automatically set active unless doing it manually.
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_nextSceneIndex, LoadSceneMode.Additive);

            // Handle loading effect.
            await GetProgressEffect(loadOperation);

            // Wait for the new scene to fully initialized.
            await UniTask.WaitUntil(() =>
            {
                return loadOperation.isDone;
            });

            // Time.timeScale = 1;

            // Get end loading effect.
            _sceneLoader.EndLoading();

            // Active new scene after loading new scene with additive mode or handling loading effects (if have).
            Scene nextScene = SceneManager.GetSceneByBuildIndex(_nextSceneIndex);
            if (nextScene.IsValid() && nextScene.isLoaded)
            {
                _cam.gameObject.SetActive(false);
                SceneManager.SetActiveScene(nextScene);
            }

            // Wait to unload sceneLoader.
            await UniTask.Delay((int)(_sceneLoader.GetEffectTime() * 1000f));

            await SceneManager.UnloadSceneAsync(SceneLoaderPath);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Unitask is cancelled.");
        }

    }

    private async UniTask GetProgressEffect(AsyncOperation asyncOperation)
    {
        asyncOperation.allowSceneActivation = false;

        float stallTime = 0.9f;

        /*
        - Wait to load next scene asynchronously.
        - When allowSceneActivation = false -> loadOperation.progress will not exceed 0.9(= stallTime).
        - Can have some progress UI effects in here.
        */
        while (asyncOperation.progress < stallTime)
        {
            // Add some progress UI like progress slider.
            float progress = Mathf.Clamp01(asyncOperation.progress / stallTime);
            Debug.Log($"Progress {progress}");
            await UniTask.Yield(); // prevent main thread blocking.
        }

        asyncOperation.allowSceneActivation = true;
    }
}
