using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Photon.PunBehaviour
{
    public enum PlayerType { Unititalized, Busker, Viewer };
    PlayerType playerType = PlayerType.Unititalized;

    #region Public Variables

    /// <summary>
    /// The PUN loglevel. 
    /// </summary>
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so we will prompt user that room is full.
    /// </summary>   
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so we will prompt user room is full")]
    public byte MaxPlayersPerRoom = 32;

    public GameObject BuskerPrefab;
    public GameObject ViewerPrefab;
    GameObject PlayerReference;

    #endregion

    #region Private Variables

    /// <summary>
    /// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
    /// </summary>
    string _gameVersion = "1";

    /// <summary>
    /// This is the callback function to the UI when the player has connected
    /// </summary>
    UI_Networking.ConnectedToPhotonDelegate PlayerConnected;

    string buskerRoomName = "uninitialized";

    #endregion

    #region MonoBehaviour CallBacks

    void Awake()
    {
        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
        
        // #NotImportant
        // Force LogLevel
        PhotonNetwork.logLevel = Loglevel;
    }

    #endregion

    #region Public Methods

    public void ConnectToPhoton(PlayerType pType, string roomName, UI_Networking.ConnectedToPhotonDelegate function)
    {
        playerType = pType;
        buskerRoomName = roomName;
        PlayerConnected = function;
        
        switch (pType)
        {
            case PlayerType.Busker:
                PhotonNetwork.playerName = "Busker";
                break;
            case PlayerType.Viewer:
                PhotonNetwork.playerName = "Viewer xx";
                break;
            case PlayerType.Unititalized:
                Debug.Log("Player type is uninitialized when trying to connect to photon!");
                break;
        }

        Connect();
    }

    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt to create a room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    /// 
    public void Connect()
    {
        switch (playerType)
        {
            case PlayerType.Busker:
                if (PhotonNetwork.connected) // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
                {
                    // #Critical attempt to create room for this busker. If it fails, we'll get notified in OnPhotonCreateRoomFailed() and we'll prompt the user.
                    PhotonNetwork.CreateRoom(buskerRoomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
                }
                else
                {
                    // #Critical, we must first and foremost connect to Photon Online Server.
                    PhotonNetwork.ConnectUsingSettings(_gameVersion);
                }
                break;
            case PlayerType.Viewer:
                if (PhotonNetwork.connected) // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
                {
                    // #Critical we need at this point to attempt joining room for busker
                    PhotonNetwork.JoinRoom(buskerRoomName);
                }
                else
                {
                    // #Critical, we must first and foremost connect to Photon Online Server.
                    PhotonNetwork.ConnectUsingSettings(_gameVersion);
                }
                break;
        }
    }

    #endregion

    #region Photon.PunBehaviour CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");

        switch (playerType)
        {
            case PlayerType.Busker:
                // #Critical attempt to create room for this busker. If it fails, we'll get notified in OnPhotonCreateRoomFailed() and we'll prompt the user.
                PhotonNetwork.CreateRoom(buskerRoomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
                break;
            case PlayerType.Viewer:
                // #Critical we need at this point to attempt joining room for busker
                PhotonNetwork.JoinRoom(buskerRoomName);
                break;
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in the room: " + PhotonNetwork.room.Name);

        if (PlayerConnected != null)
            PlayerConnected(true);
        else
            Debug.Log("Callback function PlayerConnected is null!");

        

        switch (playerType)
        {
            case PlayerType.Busker:
                PhotonNetwork.Instantiate("Busker_Prefab", Vector3.zero, Quaternion.identity, 0);
                //PlayerReference = Instantiate(BuskerPrefab);
                break;
            case PlayerType.Viewer:
                //Turn off main camera since we have an ar camera now
                Camera.main.gameObject.SetActive(false);
                PlayerReference = Instantiate(ViewerPrefab);
                break;
            case PlayerType.Unititalized:
                Debug.Log("Player type is uninitialized when trying to connect to photon!");
                break;
        }
    }

    public override void OnLeftRoom()
    {
        switch (playerType)
        {
            case PlayerType.Busker:
                Debug.Log("Launcher: OnLeftRoom() called by PUN. This means room should be destroyed, as this player is the master.");
                break;
            case PlayerType.Viewer:
                Debug.Log("Launcher: OnLeftRoom() called by PUN. What do we do?");
                break;
        }
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        switch (playerType)
        {
            case PlayerType.Busker:
                Debug.Log("Launcher:OnPhotonCreateRoomFailed() was called by PUN. Could not create room, need to do stuff :P ");
                break;
            case PlayerType.Viewer:
                Debug.Log("Launcher:OnPhotonCreateRoomFailed() was called by PUN but type is Viewer. This shouldn't happen!");
                break;
        }
        
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        switch (playerType)
        {
            case PlayerType.Busker:
                Debug.Log("Launcher:OnPhotonJoinFailed() was called by PUN but playerType is Busker. THis should not happen!");
                break;
            case PlayerType.Viewer:
                Debug.Log("Launcher:OnPhotonJoinFailed() was called by PUN. More info needs to be added to this log!");
                break;
        }
        
    }

    #endregion
}
