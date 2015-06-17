using UnityEngine;
using System.Collections;

public class ServerSetup : MonoBehaviour {

	public string ip;
	public int MSport;
	public int FCport;

	public GameObject tester;

	// Use this for initialization
	void Start () {
		MasterServer.ipAddress = ip;
		MasterServer.port = MSport;
		Network.natFacilitatorIP = ip;
		Network.natFacilitatorPort = FCport;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy(){
		Network.Disconnect();
		MasterServer.UnregisterHost();
	}

	void OnGUI() {
		if (GUILayout.Button("Start Server")) {
			Debug.Log("initializing server");
			bool useNat = !Network.HavePublicAddress();
			Network.InitializeServer(32, 3000, useNat);
			MasterServer.RegisterHost("Skirmish", "ArmyCombat", "Testing");
			//ip = MasterServer.ipAddress;
			//port = MasterServer.port;
		}

		if (GUILayout.Button("Server Info")) {
			MasterServer.RequestHostList("Skirmish");
			if(MasterServer.PollHostList().Length != 0){
				HostData[] hostData = MasterServer.PollHostList();
				int i = 0;
				while (i < hostData.Length) {
					Debug.Log("Game name: " + hostData[i].gameName);
					Debug.Log("players: " + hostData[i].connectedPlayers);
					Debug.Log("game type: " + hostData[i].gameType);
					Debug.Log("guid: " + hostData[i].guid);
					i++;
				}
				MasterServer.ClearHostList();
			}
		}

		if (GUILayout.Button("Connect to Server")) {
			Debug.Log("connecting to server");
			//Network.Connect(ip, port);
			Debug.Log(Network.isClient);
			Debug.Log(Network.isServer);
		}
	}

	private int playerCount = 0;
    void OnPlayerConnected(NetworkPlayer player) {
        Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
        Network.Instantiate(tester, Vector3.zero, Quaternion.identity, 0);
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
    	playerCount--;
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

	void OnFailedToConnect(NetworkConnectionError error) {
        Debug.Log("Could not connect to server: " + error);
    }
}
