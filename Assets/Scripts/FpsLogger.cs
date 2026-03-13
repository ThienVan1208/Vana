using TMPro;
using UnityEngine;

public class FpsLogger : MonoBehaviour
{
    private float timer = 0f;
    private int frameCount = 0;
    public TextMeshProUGUI fpsTxt;

    void Update()
    {
        // Add to our frame count and track the time passed
        frameCount++;
        timer += Time.unscaledDeltaTime;

        // If one second has passed, calculate and log the FPS
        if (timer >= 1f)
        {
            int fps = Mathf.RoundToInt(frameCount / timer);

            fpsTxt.text = "FPS: " + fps.ToString();

            // Reset the counters for the next second
            frameCount = 0;
            timer -= 1f;
        }
    }
}
