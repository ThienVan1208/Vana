using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GoogleAuth : MonoBehaviour
{
    public string GoogleAPI = "735053440217-7cinu2vmpsm047go1155k287c56r8506.apps.googleusercontent.com";
    FirebaseAuth auth;
    FirebaseUser user;

    public TextMeshProUGUI Username, UserEmail;

    public Image UserProfilePic;
    private string imageUrl;
    private bool isGoogleSignInInitialized = false;
    private void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void Login()
    {
        // Config for google setting.
        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
            };

            isGoogleSignInInitialized = true;
        }
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = GoogleAPI
        };
        GoogleSignIn.Configuration.RequestEmail = true;


        // This will wait for google sign-in.
        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        // Used to bridge the google sign-in task with firebase auth.
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

        // Check result of sign-in task.
        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
                Debug.Log("Cancelled");
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);

                Debug.Log("Faulted " + task.Exception);
            }
            else
            {
                /* 
                - Credential type acts as an ID card used to verify a user's identity.
                - In this context, @credential is used to authenticate the user with firebase.    
                */
                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                {
                    if (authTask.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                    }
                    else if (authTask.IsFaulted)
                    {
                        signInCompleted.SetException(authTask.Exception);
                        Debug.Log("Faulted In Auth " + task.Exception);
                    }
                    else
                    {
                        signInCompleted.SetResult(authTask.Result);
                        Debug.Log("Success");
                        user = auth.CurrentUser;
                        Username.text = user.DisplayName;
                        UserEmail.text = user.Email;

                        StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));
                    }
                });
            }
        });
    }
    private string CheckImageUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }
        return imageUrl;
    }

    IEnumerator LoadImage(string imageUri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            // Use the loaded texture here
            Debug.Log("Image loaded successfully");
            UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
        else
        {
            Debug.Log("Error loading image: " + www.error);
        }


    }
}