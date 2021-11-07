using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        NetworkConfig.InitNetWork();
        NetworkConfig.ConnectToServer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        NetworkConfig.DisconnectFromServer();
    }
}
