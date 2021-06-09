using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*robot behavior for robot coop modern. Not using for the current game*/
public class RobotBehavior : NetworkBehaviour
{
    [SerializeField] private int destroyTime = 5;
    [SyncVar]
    [SerializeField] private GameObject player;
    public float moveSpeed = 0.5f;
    public int score = 1;

    public LayerMask mirrorLayer;
    // Start is called before the first frame update
    void Start()
    {
        List<PlayerIdentity> players = GameObject.Find("GameManager").GetComponent<RoundManagerNet>().players;
        int i = Random.Range(0, players.Count);
        player = players[i].gameObject;
        Debug.Log("player" + player);
        
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }
    
    public void onShot() {
        Debug.Log("on shot called");
        // behavior at shot
        //score puls one
        ScoreManager.instance.GetPoint1(score);
        // can be object pool
        Destroy(gameObject);

    }
    private void OnTriggerEnter(Collider other) {
        
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Mirror")) && other.gameObject == player) {
            ScoreManager.instance.LosePoint1(score);
            NetworkServer.Destroy(gameObject);
        }
    }
    IEnumerator waitForDestroy(){
        Debug.Log("start destroy robot");
        yield return new WaitForSeconds(destroyTime);
        NetworkServer.Destroy(gameObject);
    }
}
