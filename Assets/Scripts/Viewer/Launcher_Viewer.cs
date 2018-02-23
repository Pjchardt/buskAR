using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher_Viewer : Photon.PunBehaviour
{
    //TODO: Have one launcher and set whether busker or viewer when button clicked

    #region Public Variables

    /// <summary>
    /// The PUN loglevel. 
    /// </summary>
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

    public GameObject ViewerPrefab;
    GameObject ViewerReference;

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

    public void ConnectToPhoton(string roomName, UI_Networking.ConnectedToPhotonDelegate function)
    {
        buskerRoomName = roomName;
        PlayerConnected = function;

        PhotonNetwork.playerName = "Viewer xx"; //will rename when in (somehow!)
        ViewerReference = Instantiate(ViewerPrefab);

        Connect();
    }

    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    /// 
    public void Connect()
    {
        
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
    }


    #endregion

    #region Photon.PunBehaviour CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Launcher_Viewer: OnConnectedToMaster() was called by PUN");
 
        // #Critical we need at this point to attempt joining room for busker
        PhotonNetwork.JoinRoom(buskerRoomName);
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("Launcher_Viewer: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Launcher_Viewer:OnPhotonJoinFailed() was called by PUN. More info needs to be added to this log!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Launcher_Viewer: OnJoinedRoom() called by PUN. Now this client is in a room.");
        if (PlayerConnected != null)
            PlayerConnected(true);
        else
            Debug.Log("Callback function PlayerConnected is null!");
    }

    #endregion
}
