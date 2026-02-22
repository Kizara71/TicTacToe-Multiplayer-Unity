using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkMangerUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    void Awake()
    {
        startHostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }
}
