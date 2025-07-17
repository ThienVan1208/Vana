using UnityEngine;

public static class Utils 
{
    public static bool GetPercent(float percent)
    {
        return Random.Range(0f, 100f) == percent;
    }
}
