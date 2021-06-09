using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class CommandLine : MonoBehaviour
{

    NetworkManager networkManager;
    private void Awake()
    {
        var args = Environment.GetCommandLineArgs();
        bool clientMode = true;
        for (int x = 0; x < args.Length; x++)
        {
            string arg = args[x];
            if (arg == "-server")
            {
                clientMode = false;
            }

            if (arg == "-i")
            {
                if (x == args.Length - 1)
                {
                    Debug.LogError("need value after -i");
                    return;
                }

                string ip = args[x + 1];
                //AppSettings.SetAddress(ip);
            }

            if (arg == "-p")
            {
                if (x == args.Length - 1)
                {
                    Debug.LogError("need value after -p");
                    return;
                }

                string port = args[x + 1];
                //AppSettings.SetPort(int.Parse(port));
            }

            if (arg == "-a")
            {
                if (x == args.Length - 1)
                {
                    Debug.LogError("need value after -a");
                    return;
                }

                string cubes = args[x + 1];
                //AppSettings.CubeAmount = int.Parse(cubes);
            }
        }

        if (!clientMode)
        {
            networkManager = GetComponent<NetworkManager>();
            networkManager.StartServer();
            Debug.Log("server start success");
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
