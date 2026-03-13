using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button createButton;
    [SerializeField] private Button cancelButton;
    private void Awake() {
        cancelButton.onClick.AddListener(()=>gameObject.SetActive(false));
        createButton.onClick.AddListener(CreateLobby);
    }
    private async void CreateLobby()
    {
        gameObject.SetActive(false);
        await LobbyManager.Instance.CreateLobby(inputField.text);
    }
}
