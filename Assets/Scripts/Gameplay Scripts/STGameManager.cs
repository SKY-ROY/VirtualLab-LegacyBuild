using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class STGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject UI_InformPanel_Gameobject;
    public GameObject searchForGamesButtonGameObject;
    public TextMeshProUGUI UI_Inform_Text;
    public GameObject adjust_Button;
    public GameObject raycastCenter_Image;

    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        UI_InformPanel_Gameobject.SetActive(true);
        //UI_InformText.text = "Search for Games to BATTLE!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods
    
    public void JoinRandomRoom()
    {
        UI_Inform_Text.text = "Searching for available rooms...";

        PhotonNetwork.JoinRandomRoom();

        searchForGamesButtonGameObject.SetActive(false);
    }
    
    public void OnQuitMatchButtonClicked()
    {
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        
        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }
    }

    #endregion

    #region Photon Callback Methods

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        UI_Inform_Text.text = message;

        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        adjust_Button.SetActive(false);
        raycastCenter_Image.SetActive(false);

        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            UI_Inform_Text.text = "Joined to " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";
        }
        else
        {
            UI_Inform_Text.text = "Joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeavtivateAfterSeconds(UI_InformPanel_Gameobject, 2f));
        }
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string displayMessage = newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " (Player count: " + PhotonNetwork.CurrentRoom.PlayerCount + ")";
        
        Debug.Log(displayMessage);
        UI_Inform_Text.text = displayMessage;

        StartCoroutine(DeavtivateAfterSeconds(UI_InformPanel_Gameobject, 2f));
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    #endregion

    #region Private Methods

    void CreateAndJoinRoom()
    {
        string randomRoomName = "Room" + Random.Range(0,1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        //Creating the room
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeavtivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);
    }

    #endregion
}
