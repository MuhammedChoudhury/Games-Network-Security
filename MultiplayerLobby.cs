using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    
    public Transform LoginPanel;
    public Transform SelectionPanel;
    public Transform CreateRoomPanel;
    public Transform InsideRoomPanel;
    public Transform ListRoomsPanel;

    public InputField roomNameInput;

    public InputField playerNameInput;

    public GameObject textPrefab;
    public GameObject startGameButton;
    public Transform insideRoomPlayerList;

    string playerName;

    public Transform listRoomPanel;
    public GameObject roomEntryPrefab;
    public Transform listRoomPanelContent;
    public Transform chatPanel;

    Dictionary<string, RoomInfo> cachedRoomList;

    public Chat chat;

    private void Start()
    {
        playerNameInput.text = playerName = string.Format("Player{0}", Random.Range(1,1000000));

        cachedRoomList = new Dictionary<string, RoomInfo>();

        PhotonNetwork.AutomaticallySyncScene = true;
    }



    public void LoginButtonClicked()
    {
        if (playerNameInput.text.Trim() != "")
        {
        PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;

        PhotonNetwork.ConnectUsingSettings();
        UpdatePlayfabUsername(playerName);
        }
        else
        {
            Debug.Log("Player name is invalid.");
        }
}

    public void DisconnectButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }

    public void ListRoomsClicked()
    {
        PhotonNetwork.JoinLobby();
    }

    public void StartGameClicked()
    {

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("Multiplayer");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We have connected to the master server!");
        ActivatePanel("Selection");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from the master server!");
        ActivatePanel("Login");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room has been created!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room!");
    }

    public override void OnJoinedRoom()
    {

        var authenticationValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        chat.userName = PhotonNetwork.LocalPlayer.NickName;
        chat.ChatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authenticationValues);

        Debug.Log("Room has been joined!");
        ActivatePanel("InsideRoom");
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            playerListEntry.GetComponent<Text>().text = player.NickName;
            playerListEntry.name = player.NickName;
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        ActivatePanel("ListRooms");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room Update: " + roomList.Count);

        DestroyChildren(listRoomPanelContent);

        UpdatedCachedRoomList(roomList);

        foreach (var room in cachedRoomList)
        {
            var newRoomEntry = Instantiate(roomEntryPrefab, listRoomPanelContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.roomName = room.Key;
            newRoomEntryScript.roomText.text = string.Format("[{0} - ({1}/{2})]", room.Key, room.Value.PlayerCount, room.Value.MaxPlayers);
        }   
    }

    public void ActivatePanel(string panelName)
    {
        LoginPanel.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
        CreateRoomPanel.gameObject.SetActive(false);
        InsideRoomPanel.gameObject.SetActive(false);
        ListRoomsPanel.gameObject.SetActive(false);
        chatPanel.gameObject.SetActive(false);

        if (panelName == LoginPanel.gameObject.name)
            LoginPanel.gameObject.SetActive(true);
        else if (panelName == SelectionPanel.gameObject.name)
            SelectionPanel.gameObject.SetActive(true);
        else if (panelName == CreateRoomPanel.gameObject.name)
            CreateRoomPanel.gameObject.SetActive(true);
        else if (panelName == InsideRoomPanel.gameObject.name)
            InsideRoomPanel.gameObject.SetActive(true);
        else if (panelName == ListRoomsPanel.gameObject.name)
            ListRoomsPanel.gameObject.SetActive(true);
        else if (panelName == chatPanel.gameObject.name)
            chatPanel.gameObject.SetActive(true);    
    }

    public void CreateARoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {

        chat.ChatClient.Disconnect();
        
        Debug.Log("Room has been joined!");
        ActivatePanel("CreateRoom");

        DestroyChildren(insideRoomPlayerList);
    }

    public void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void LeaveLobbyClicked()
    {
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
        DestroyChildren(listRoomPanelContent);
        DestroyChildren(insideRoomPlayerList);
        cachedRoomList.Clear();
        ActivatePanel("Selection");
    }

    public void OnJoinRandomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void UpdatedCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
                cachedRoomList.Remove(room.Name);
            else 
                cachedRoomList[room.Name] = room;
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("A Player joined the Room");
        var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
        playerListEntry.GetComponent<Text>().text = newPlayer.NickName;
        playerListEntry.name = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("A Player left the Room");
        foreach (Transform child in insideRoomPlayerList)
        {
            if(child.name == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join Room. " + message);
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random Room. " + message);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // void UpdatePlayfabUsername(string name)
    // {
    //     PlayFabClientAPI.UpdateUserTitleDisplayName();
    // }

    void PlayFabUpdateUserTitleDisplayNameResult(UpdateUserTitleDisplayNameResult updateUsetTitleDisplayNameResult)
    {
        Debug.Log("PlayFab - UserTitleDisplayName updated.");
    }

    void PlayFabUpdateUserTitleDisplayNameError(PlayFabError PlayFabUpdateUserTitleDisplayNameError)
    {
        Debug.Log("PlayFab - Error occured while updating UserTitleDisplayName: " + PlayFabUpdateUserTitleDisplayNameError.ErrorMessage);
    }


void UpdatePlayfabUsername(string name)
{
    UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
    {
        DisplayName = name 
    };
    PlayFabClientAPI.UpdateUserTitleDisplayName(request, PlayFabUpdateUserTitleDisplayNameResult, PlayFabUpdateUserTitleDisplayNameError);
}

    
}
