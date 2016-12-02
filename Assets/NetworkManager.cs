﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using PlayGroup;
using UnityEngine.SceneManagement;

namespace Network
{

	public class NetworkManager : Photon.PunBehaviour {

		public static NetworkManager control;

		public PhotonLogLevel logLevel;
		public byte maxPlayersOnServer = 32;
		public bool isConnected = false;

		//Client version number
		string _gameVersion = "1";

	
	void Awake(){
			if (control == null) {
			
				control = this;
			
			} else {
			
				Destroy (this);
			
			}

			PhotonNetwork.logLevel = logLevel;
			//no lobby, just server(room)
			PhotonNetwork.autoJoinLobby = false;
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.automaticallySyncScene = true;

	}

	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

		public void Connect(){ //Called from login window

			// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
			if (PhotonNetwork.connected)
			{
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
				PhotonNetwork.JoinRandomRoom();
				Debug.Log ("JOIN RANDOM ROOM");
			}else{
				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings(_gameVersion);
				Debug.Log ("CONNECT TO THE PUNderdome");
			}


		}

		//Network public functions

		public void LeaveMap(){
		
		
			PhotonNetwork.LeaveRoom ();
			SceneManager.LoadSceneAsync ("Lobby");
		
		}

		public void LoadMap()
		{
			if (!PhotonNetwork.isMasterClient) {
				Debug.Log ("You are not the master client, joining map");
				SceneManager.LoadSceneAsync ("Kitchen-Reconstruct");
			} else {
				Debug.Log ("You are the master client, loading the level (default kitchen_construct)");
								SceneManager.LoadSceneAsync ("Kitchen-Reconstruct");
			}
		}

		//PUN CALLBACKS BELOW:

		public override void OnConnectedToMaster ()
		{
			Debug.Log ("Connect to PUNderdome");
			UIManager.control.chatControl.ReportToChannel ("Server: connecting to server...");
			PhotonNetwork.playerName = UIManager.control.chatControl.UserName;
			PhotonNetwork.JoinRandomRoom ();
		} 

		public override void OnDisconnectedFromPhoton ()
		{
			Debug.Log ("DISCONNECTED");
			UIManager.control.chatControl.ReportToChannel ("Server: disconnected.");
			isConnected = false;
		}

		public override void OnPhotonRandomJoinFailed (object[] codeAndMsg)
		{
			Debug.Log ("Room Join Failed, creating our own server to be loners on");
	

			PhotonNetwork.CreateRoom (null, new RoomOptions () { maxPlayers = maxPlayersOnServer }, null); //Create the room with default settings and 32 max players
		}

		public override void OnJoinedRoom ()
		{
			Debug.Log ("Successfully joined!");

			UIManager.control.chatControl.ReportToChannel("Welcome to unitystation. Press T to chat");
			isConnected = true;
			PlayerManager.control.CheckIfSpawned (); // Spawn the character if in the game already (This is for development when you are working on the map scenes)

			if (PhotonNetwork.isMasterClient && GameData.control.isInGame) { // This is used if you logged in while working on the map in the editor, it will set up the server aswell
			
				LoadMap ();
			
			} 
		}

		public override void OnPhotonPlayerDisconnected( PhotonPlayer other  )
		{
			Debug.Log( "PUNderDomePlayerDisconnected() " + other.name ); // seen when other disconnects


		}

		public override void OnPhotonPlayerConnected( PhotonPlayer other  ) 
		{
			Debug.Log( "OnPhotonPlayerConnected() " + other.name ); // not seen if you're the player connecting


		}

	
}
}