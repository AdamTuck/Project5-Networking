using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using TMPro;

public class GameNetworkManager : MonoBehaviour
{
    [SerializeField] private int maxConnections = 10;

    [SerializeField] private GameObject btnHost;
    [SerializeField] private GameObject btnClient;
    [SerializeField] private GameObject btnStart;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text playerIDText;
    [SerializeField] private TMP_InputField joinCodeText;
    [SerializeField] private TMP_Dropdown playerSkinDropdown;
    
    private string playerID;
    private string joinCode;
    private bool clientAuthenticated = false;

    private async void Start()
    {
        await AuthenticatePlayer();
    }

    async Task AuthenticatePlayer ()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            playerID = AuthenticationService.Instance.PlayerId;

            clientAuthenticated = true;

            playerIDText.text = $"Player ID: {playerID}";
            Debug.Log($"Player Authenticated Successfully - {playerID}");
        }
        catch (Exception e)
        {
            Debug.Log("An error occurred with authentication: " + e);
        }
    }

    public async Task<RelayServerData> AllocateRelayServerAndGetCode (int _maxConnections, string serverRegion = null)
    {
        Allocation allocation;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(_maxConnections, serverRegion);
        }
        catch (Exception e)
        {
            Debug.Log("An error occurred allocating relay server: " + e);
            throw;
        }

        Debug.Log($"Server Datapoints: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"Server ID: {allocation.AllocationId}");

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.Log("An error occurred getting join code: " + e);
            throw;
        }

        return new RelayServerData(allocation, "dtls");
    }

    IEnumerator ConfigureGetCodeAndJoinHost ()
    {
        var allocateAndGetCode = AllocateRelayServerAndGetCode(maxConnections);

        while(!allocateAndGetCode.IsCompleted)
        {
            yield return null;
        }

        if (allocateAndGetCode.IsFaulted)
        {
            Debug.LogError($"Cannot allocate the server due to an exception: {allocateAndGetCode.Exception.Message}");
            yield break;
        }

        var relayServerData = allocateAndGetCode.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();

        joinCodeText.gameObject.SetActive(true);
        joinCodeText.text = joinCode;
        joinCodeText.readOnly = true;
        btnStart.SetActive(true);
        statusText.text = "Joined as Host";
    }

    public async Task<RelayServerData> JoinRelayServerWithCode (string _joinCode)
    {
        JoinAllocation joinAllocation;

        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(_joinCode);
        }
        catch (Exception e)
        {
            Debug.Log($"Relay allocation join request failed: {e}");
            throw;
        }

        Debug.Log($"Client: {joinAllocation.ConnectionData[0]} {joinAllocation.ConnectionData[1]}");
        Debug.Log($"Host: {joinAllocation.HostConnectionData[0]} {joinAllocation.HostConnectionData[1]}");
        Debug.Log($"Client Join ID: {joinAllocation.AllocationId}");

        return new RelayServerData(joinAllocation, "dtls");
    }

    IEnumerator ConfigureUseCodeJoinClient (string _joinCode)
    {
        var joinAllocationFromCode = JoinRelayServerWithCode(_joinCode);

        while (!joinAllocationFromCode.IsCompleted)
        {
            yield return null;
        }

        if (joinAllocationFromCode.IsFaulted)
        {
            Debug.LogError($"Cannot join relay server due to an exception: {joinAllocationFromCode.Exception.Message}");
            yield break;
        }

        var relayServerData = joinAllocationFromCode.Result;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();

        statusText.text = "Joined as Client" ;
    }

    public void JoinHost()
    {
        if (!clientAuthenticated)
        {
            Debug.Log("Client not authenticated - cannot host");
            statusText.text = "Cannot host - not authenticated";
            return;
        }

        StartCoroutine(ConfigureGetCodeAndJoinHost());

        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        joinCodeText.gameObject.SetActive(false);
        playerSkinDropdown.gameObject.SetActive(false);
    }

    public void JoinClient()
    {
        if (!clientAuthenticated)
        {
            Debug.Log("Client not authenticated - cannot join");
            statusText.text = "Cannot join - not authenticated";
            return;
        }

        if (joinCodeText.text.Length <=0)
        {
            Debug.Log("No join code entered");
            statusText.text = "No join code entered";
            return;
        }

        StartCoroutine(ConfigureUseCodeJoinClient(joinCodeText.text));

        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        joinCodeText.gameObject.SetActive(false);
        playerSkinDropdown.gameObject.SetActive(false);

    }

    public void JoinServer ()
    {
        NetworkManager.Singleton.StartServer();
        statusText.text = "Joined as Server!";
    }
}