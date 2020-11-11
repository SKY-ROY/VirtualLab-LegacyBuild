using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject UI_LoginGameObject;

    [Header("Lobby UI")]
    public GameObject UI_LobbyGameObject;
    public GameObject UI_3DGameObject;

    [Header("Connection Status UI")]
    public GameObject UI_ConnectionStatusGameObject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;


    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            //Activating only Lobby UI
            UI_ConnectionStatusGameObject.SetActive(false);
            UI_LoginGameObject.SetActive(false);
 
            UI_LobbyGameObject.SetActive(true);
            UI_3DGameObject.SetActive(true);
        }
        else 
        {
            //Activating only Login UI since we did not connect to photon yet
            UI_LobbyGameObject.SetActive(false);
            UI_3DGameObject.SetActive(false);
            UI_ConnectionStatusGameObject.SetActive(false);

            UI_LoginGameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }
    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonClicked()
    {
        string playerName = playerNameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            UI_LobbyGameObject.SetActive(false);
            UI_3DGameObject.SetActive(false);
            UI_LoginGameObject.SetActive(false);

            showConnectionStatus = true;
            UI_ConnectionStatusGameObject.SetActive(true);

            //Only change name if photon is not connected already
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;

                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player name is invalid or empty");
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        //SceneManager.LoadScene("Scene_Loading");
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }

    #endregion

    #region PHOTON Callback Methods
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet.");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server.");

        //Disabling every other UI object
        UI_LoginGameObject.SetActive(false);
        UI_ConnectionStatusGameObject.SetActive(false);
        
        //Enabling only the lobby UI elements and 3D object
        UI_LobbyGameObject.SetActive(true);
        UI_3DGameObject.SetActive(true);
    }
    #endregion
}
