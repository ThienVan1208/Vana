using UnityEngine;
using Firebase;
using Firebase.Analytics;
using System.Threading.Tasks;

public class FirebaseInit : MonoBehaviour
{
    async void Start()
    {
        try
        {
            // Check and fix Firebase dependencies
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Initialize Firebase
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("Firebase initialized successfully!");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Firebase initialization failed: {ex.Message}");
        }
    }
}