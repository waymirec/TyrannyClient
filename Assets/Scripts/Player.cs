using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tyranny.Networking;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        /*
        if (NetworkClient.Instance.Connected)
        {
            PacketWriter packetOut = new PacketWriter(TyrannyOpcode.Move);
            packetOut.Write(Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOP"));
            packetOut.Write(1.0f);
            packetOut.Write(1.0f);
            packetOut.Write(1.0f);
            Debug.Log("Sending movement");
            NetworkClient.Instance.Send(packetOut);
        }
        */
    }
}
