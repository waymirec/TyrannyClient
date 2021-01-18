using Tyranny.Client.Network;
using Tyranny.Client.System;
using Tyranny.Networking;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    //[SerializeField]
    private Camera cam;

    [SerializeField]
    private NavMeshAgent agent;

    private void Awake()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);

                var pos = agent.transform.position;
                PacketWriter movePacket = new PacketWriter(TyrannyOpcode.Move);
                movePacket.Write(Registry.Get<WorldClient>().Id.ToByteArray());
                movePacket.Write(pos.x);
                movePacket.Write(pos.y);
                movePacket.Write(pos.z);
                movePacket.Write(hit.point.x);
                movePacket.Write(hit.point.y);
                movePacket.Write(hit.point.z);
                Registry.Get<WorldClient>().Send(movePacket);
            }
        }
    }
}
