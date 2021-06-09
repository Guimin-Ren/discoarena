using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*robot generator for robot coop modern. Not using for the current game*/
public class RobotGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] generateFields;
    [SerializeField] private int repeatTimes = 5;
    [SerializeField] private float intervalTime = 5f;
    public float stage1time = 20f;
    [SerializeField] private GameObject robotPrefab;
    private bool isGenerated = false;
    enum Pattern {
        still, pattern1, pattern2
    }
    Pattern currPattern;
    // Start is called before the first frame update
    private void Awake() {
        isGenerated = false;
    }
    
    void Start()
    {
        isGenerated = true;
        currPattern = Pattern.pattern1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGenerated) {
            if (currPattern == Pattern.pattern1) {
                StartGenerateRobots1();
                currPattern = Pattern.still;
            } else if (currPattern == Pattern.pattern2) {
                StartGenerateRobots2();
                currPattern = Pattern.still;
            }
            
        }
    }

    public void StartGenerateRobots() {
        isGenerated = true;
        currPattern = Pattern.pattern1;
    }
    [Server]
    private void StartGenerateRobots1() {
        StartCoroutine(GenerateRobots1());
        StopCoroutine(GenerateRobots2());
    }
    [Server]
    private void StartGenerateRobots2() {
        StartCoroutine(GenerateRobots2());
        StopCoroutine(GenerateRobots1());
    }

    IEnumerator GenerateRobots1() {
        while (repeatTimes > 0) {
            GameObject robot = Instantiate(robotPrefab,generateFields[1], true);
            NetworkServer.Spawn(robot);
            yield return new WaitForSeconds(intervalTime);
            repeatTimes--;
        }
        currPattern = Pattern.pattern2;
       
    }
    IEnumerator GenerateRobots2() {
        while (true) {
            int i = Random.Range(3, 6);
            GameObject robot = Instantiate(robotPrefab,generateFields[i], true);
            NetworkServer.Spawn(robot);
            yield return new WaitForSeconds(intervalTime / 2);
        }
       
    }


}
