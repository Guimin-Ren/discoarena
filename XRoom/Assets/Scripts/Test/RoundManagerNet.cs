using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
//using UnityEditorInternal;

/*This scripts works as a game manager to control the game process.
 It was runned on the server and will call other scripts to run features*/
public enum Stage
{
    nonestage, stage1, stage2, stageGameover
}

public enum PrepStatus
{
    three, two, one, go, none
}

public class RoundManagerNet : NetworkBehaviour
{
    public float timer;
    [SyncVar]
    public float timerRatio = 1;

    // [SerializeField] private RobotGenerator RG;
     public bool IsGameReady;
     public bool IsGameOver;
     public List<PlayerIdentity> players = new List<PlayerIdentity>();

    private bool generate = false;
    private LightSourceNetController lightSourceNetController;

    [SyncVar]
    public bool stageReady = false;

    [SyncVar]
    public bool roundOver = false;

    [SyncVar]
    public bool stagePrepare = false;
    [SyncVar]
    public bool gameOver = false;

    [SyncVar]
    public PrepStatus prepStatus = PrepStatus.none;
     
    [SerializeField] public Stage currStage;
    [SerializeField] private int RoundTime = 4;
    [SerializeField] private int intervalTime = 3;
    [SerializeField] private int floatLightSourceNum = 3;
    [SerializeField] private float stageReadyTime = 5;
    [SerializeField] private float subRoundReadyTime = 5;

    [SerializeField] private PlayerStatus[] pss;
    [SerializeField] private int winPoint = 5;

    [SyncVar]
    public bool isPlayersReady = false;

    public bool canHit = false;

    public bool subRoundReady = false;
    private int pMirrorCount = 0;

    // Start is called before the first frame update
  
    [ServerCallback]
    void Start()
    {
        currStage = Stage.stage1;

        StartCoroutine(WaitPlayersReady());
    }

    [ServerCallback]
    IEnumerator WaitPlayersReady()
    {
        yield return new WaitUntil(() => FindObjectOfType<NetworkFlashLight>() != null);
        NetworkFlashLight networkFlashLight = FindObjectOfType<NetworkFlashLight>();
        yield return new WaitUntil(() => networkFlashLight.numReady == true);
        pss = FindObjectsOfType<PlayerStatus>();
        bool localCheck = false;
        while (!localCheck)
        {
            localCheck = true;
            foreach(PlayerStatus ps in pss)
            {
                if(ps.isReady == false)
                {
                    localCheck = false;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        isPlayersReady = true;
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        if (NetworkManager.singleton.isNetworkActive && !IsGameReady)
            {
                Debug.Log("try to search light");

                if (lightSourceNetController == null) {
                foreach (KeyValuePair<uint, NetworkIdentity> kvp in NetworkIdentity.spawned)
                {
                    if(kvp.Value.GetComponent<LightSourceNetController>()) {
                        lightSourceNetController = kvp.Value.GetComponent<LightSourceNetController>();
                        // Debug.Log("found lightSource");
                    }
                    
                }
                }
                if (lightSourceNetController != null && lightSourceNetController.isMarkerFound == true) {
                     foreach (KeyValuePair<uint, NetworkIdentity> kvp in NetworkIdentity.spawned)
                {
                    if(kvp.Value.GetComponent<PlayerIdentity>()) {
                        players.Add(kvp.Value.GetComponent<PlayerIdentity>());
                        // Debug.Log("found player");
                    }
                    

                }
                    IsGameReady = true;
                }
            }
            else if (!NetworkManager.singleton.isNetworkActive)
            {
                //Cleanup state once network goes offline
                IsGameReady = false;
               // LocalPlayer = null;
                players.Clear();
            }
        // Game Main Loop
        if (IsGameReady) {
            // Wait for players to set ready status
            if (!isPlayersReady)
            {
                Debug.Log("players not ready");
                return;
            }
            
            timerRatio = timer / intervalTime;
            AudioManager.instance.PlayBGM();

            // check if someone win the game with 5 point
            if (!gameOver) {
                foreach(PlayerStatus ps in pss)
                {
                    if(ps.score == winPoint)
                    {
                        //end the game and break
                        currStage = Stage.stageGameover;
                        timer = intervalTime;
                        break;
                    }
                }
            }
            // game stage 1 to generate light for players.
            if (currStage == Stage.stage1) {
                if (!stagePrepare && !stageReady)
                {
                    StartCoroutine(PrepareStage(stageReadyTime));
                }
                if (!stageReady || !subRoundReady)
                {
                    return;
                }
                if (RoundTime <= 0) {
                    currStage = Stage.stage2;
                    stageReady = false;
                    stagePrepare = false;
                    roundOver = true;
                    timer = intervalTime;
                }
                timer -= Time.deltaTime;
                
                if (timer <= 0) {
                    RoundTime--;
                    if (RoundTime <= 0) {
                        return;
                    }
                    StartCoroutine(PrepareSubRound(subRoundReadyTime));
                    timer = intervalTime;
                    
                }
                //game stage2 to generate the placed mirror
            } else if (currStage == Stage.stage2) {
                if (!stagePrepare && !stageReady)
                {
                    StartCoroutine(PrepareStage(stageReadyTime));
                }
                if (!stageReady || !subRoundReady)
                {
                    return;
                }

                timer -= Time.deltaTime;
                if (timer <= 0) {
                    if (pMirrorCount >= 2) {
                       ClearAllPlacedMirror();
                    }
                    pMirrorCount++;
                    foreach (PlayerIdentity player in players)
                    {
                        player.GetComponent<RelativeTransformRespawner>().SpawmPlacedMirror();
                    }
                    timer = intervalTime;            
                    StartCoroutine(PrepareSubRound(subRoundReadyTime));
                }
            } else if (currStage == Stage.stageGameover) {
                lightSourceNetController.CmdDestroyFloatLaser();
                gameOver = true;
            }
        }

    }

    //call this function to remove all the placed MIrrors
public void ClearAllPlacedMirror() {
    MirrorBehavior[] pMirrors = FindObjectsOfType<MirrorBehavior>();
    foreach(MirrorBehavior mb in pMirrors) {
        if (mb.isFirstPhone == false) {
            NetworkServer.Destroy(mb.gameObject);
        }
    }
}
    //funcitons to works on UI updatss
    IEnumerator PrepareStage(float waitTime)
    {
        stagePrepare = true;
        lightSourceNetController.CmdDestroyFloatLaser();
        yield return new WaitForSeconds(waitTime);
        roundOver = false;

        prepStatus = PrepStatus.three;
        yield return new WaitForSeconds(1.0f);
        prepStatus = PrepStatus.two;
        yield return new WaitForSeconds(1.0f);
        prepStatus = PrepStatus.one;
        yield return new WaitForSeconds(1.0f);
        prepStatus = PrepStatus.go;
        yield return new WaitForSeconds(1.0f);
        prepStatus = PrepStatus.none;
        stageReady = true;
        subRoundReady = true;
        canHit = true;
        timer = intervalTime;
        lightSourceNetController.CmdStartFloatLaser(floatLightSourceNum);
        if (currStage == Stage.stage2) {
            pMirrorCount++;
            foreach (PlayerIdentity player in players)
            {
                player.GetComponent<RelativeTransformRespawner>().SpawmPlacedMirror();
            }
        }
    }

    IEnumerator PrepareSubRound(float subRoundPrepTime)
    {
        subRoundReady = false;
        lightSourceNetController.CmdDestroyFloatLaser();
        roundOver = true;
        yield return new WaitForSeconds(subRoundPrepTime);
        roundOver = false;
        prepStatus = PrepStatus.three;
        yield return new WaitForSeconds(1.0f);
        prepStatus = PrepStatus.two;
        yield return new WaitForSeconds(1.0f);
        prepStatus = PrepStatus.one;
        yield return new WaitForSeconds(1.0f);
        prepStatus = PrepStatus.go;
        yield return new WaitForSeconds(1.0f);
        prepStatus = PrepStatus.none;
        subRoundReady = true;
        canHit = true;
        lightSourceNetController.CmdStartFloatLaser(floatLightSourceNum);
    }
}
