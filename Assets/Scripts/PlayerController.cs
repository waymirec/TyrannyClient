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
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);

                PacketWriter movePacket = new PacketWriter(TyrannyOpcode.Move);
                movePacket.Write(Registry.Get<WorldClient>().Id.ToByteArray());
                movePacket.Write(agent.transform.position.x);
                movePacket.Write(agent.transform.position.y);
                movePacket.Write(agent.transform.position.z);
                movePacket.Write(hit.point.x);
                movePacket.Write(hit.point.y);
                movePacket.Write(hit.point.z);
                Registry.Get<WorldClient>().Send(movePacket);
            }
        }
    }
}
