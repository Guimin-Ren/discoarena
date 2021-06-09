using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MIrrorUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MirrorBehavior selfMirror;
    [SerializeField] private Image lifeBar;
    private bool prepared = false;
    void Start()
    {
        StartCoroutine(WaitPlayers());
    }

    // Update is called once per frame
    void Update()
    {
        if (prepared) {
            if (selfMirror.startRecover) {
                lifeBar.color = new Color(1,1,1,0.4f);
                lifeBar.fillAmount = 1 - (selfMirror.MirrorRecoverTimer / selfMirror.MirrorRecoverTime);
            } else {
                lifeBar.color = new Color(1,1,1,1f);
                lifeBar.fillAmount = selfMirror.MirrorLifeTimer / selfMirror.MirroLifeTime;
            }
        }
    }
     IEnumerator WaitPlayers()
    {
        yield return new WaitUntil(() => FindObjectsOfType<MirrorBehavior>() != null);
        while (selfMirror == null) {
            MirrorBehavior[] mbs = FindObjectsOfType<MirrorBehavior>();
            foreach(MirrorBehavior mb in mbs)
            {
                if (mb.isFirstPhone && mb.owner.GetComponent<NetworkIdentity>().isLocalPlayer) {
                    selfMirror = mb;
                    prepared = true;
                    break;
                }
            }
        }
        
    }

}
