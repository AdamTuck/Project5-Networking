using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GameNetworkManager : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;
    
    public void JoinHost()
    {
        NetworkManager.Singleton.StartHost();
        statusText.text = "Joined as Host";
    }

    public void JoinClient()
    {
        NetworkManager.Singleton.StartClient();
        statusText.text = "Joined as Client";
    }

    public void JoinServer ()
    {
        NetworkManager.Singleton.StartServer();
        statusText.text = "Joined as Server!";
    }
}