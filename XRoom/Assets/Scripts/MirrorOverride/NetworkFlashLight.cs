using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;


public class NetworkFlashLight : NetworkManager
{
    public delegate void OnAddNewPlayer(GameObject obj);

    public event OnAddNewPlayer onAddNewPlayerEvent;

    List<GameObject> requestPlayerList;

    public GameObject ArSceneInitialized;

    public GameObject ARSession;

    // get light source start transform
    public Transform lightSourceTransStart;

    public bool numReady = false;

    // num limit for players
    public int playerNum = 2;

    public string networkAddressTest = "172.26.6.44";

    // index for player
    int savedIndex = 1;

    public override void OnServerAddPlayer(NetworkConnection conn){
        GameObject player = Instantiate(playerPrefab);

        // assign player index
        player.GetComponent<PlayerStatus>().index = savedIndex;
        savedIndex++;

        NetworkServer.AddPlayerForConnection(conn, player);

        // spawn lightSource
        if(numPlayers == playerNum && isNetworkActive){


            PosAndRot posAndRot = new PosAndRot();
            HelperFunction.GetRelativePosAndRot(FindObjectOfType<MarkerIdentity>().transform, lightSourceTransStart, out posAndRot);

            GameObject lightSource = Instantiate(spawnPrefabs.Find(prefab =>prefab.name =="LightSourcePivot"), posAndRot.pos, posAndRot.rot);
            NetworkServer.Spawn(lightSource);
            
            GameObject rayManager = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "RayManager"), transform);
            NetworkServer.Spawn(rayManager);

            GameObject scoreManager = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "ScoreManager"), transform);
            NetworkServer.Spawn(scoreManager);

            GameObject gameManager = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"), transform);
            NetworkServer.Spawn(gameManager);

            GameObject audioManager = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "AudioManager"), transform);
            NetworkServer.Spawn(audioManager);

            numReady = true;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn){
        base.OnServerConnect(conn);
    }

    public void AddNewRequestPlayer(GameObject obj){
        requestPlayerList.Add(obj);
    }

    public override void OnStartServer() {
        ArSceneInitialized.SetActive(true);
        ARSession.SetActive(false);
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        ArSceneInitialized.SetActive(false);
        base.OnStartClient();
    }

}

