using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject manuCamera;
    [SerializeField] private GameObject networkingManagerUIPanel;

    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private void Awake()
    {
        hostBtn.onClick.AddListener(() =>
        {
            if( NetworkManager.Singleton.StartHost())
            {
                PlayerSuccessfullyConnected();
            }
            else
            {
                print("Host can't connect");
            }
        });

        clientBtn.onClick.AddListener(() =>
        {
            if(NetworkManager.Singleton.StartClient())
            {
                PlayerSuccessfullyConnected();
            }
            else
            {
                print("Client can't connect");
            }
        });
    }

    private void PlayerSuccessfullyConnected()
    {
        networkingManagerUIPanel.SetActive(false);
        manuCamera.SetActive(false);
    }
}