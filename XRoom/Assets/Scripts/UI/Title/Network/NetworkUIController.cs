using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetworkUIController : MonoBehaviour
{
    public NetworkManager manager;

    public InputField inputField;

    public TitleUIController titleUIController;

    private void Awake()
    {
        manager = FindObjectOfType<NetworkManager>();
        titleUIController = GetComponentInParent<TitleUIController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        inputField.text = manager.networkAddress;
    }

    // Update is called once per frame
    void Update()
    {
        manager.networkAddress = inputField.text;

    }

    public void StartServer()
    {
        manager.StartServer();
        Debug.Log("Start Server clicked");
        CloseTitle();
    }

    public void StartClient()
    {
        manager.StartClient();
        Debug.Log("Start Client clicked");
        CloseTitle();
    }

    public void CloseTitle()
    {
        titleUIController.CloseTitle();
    }
}
