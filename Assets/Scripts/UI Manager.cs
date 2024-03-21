using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    [SerializeField] private TMP_InputField localNickname;
    [SerializeField] private TMP_Text joinCode;
    [SerializeField] private TMP_Text playerNameTag;


    void UpdateNickname ()
    {
        playerNameTag.text = localNickname.text;
    }
}