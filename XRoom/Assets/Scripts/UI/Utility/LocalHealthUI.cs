using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LocalHealthUI : NetworkBehaviour
{
    public MirrorBehavior mirrorBehavior;
    public PlayerStatus ps;

    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        if(netIdentity.isServer){
            //gameObject.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(netIdentity.isServer){
            gameObject.SetActive(false);
        }
        else{
            SetHealth();
        }
        
    }

    [ClientCallback]
    void SetHealth(){
        if(ps != null){
            text.text = ps.score.ToString();
        }
        else{
            PlayerIdentity[] players = FindObjectsOfType<PlayerIdentity>();
            foreach(PlayerIdentity player in players){
                if(player.GetComponent<PlayerStatus>().isLocalPlayer){
                    ps = player.GetComponent<PlayerStatus>();
                }
            }
        }
        
    }
}
