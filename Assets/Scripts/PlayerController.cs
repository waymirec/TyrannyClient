using Network;
using Tyranny.Networking;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    //[SerializeField]
    private Camera cam;

    [SerializeField]
    public NavMeshAgent agent;

    private Animator anim;

    private void Awake()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Processing Click...");
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit!");
                agent.SetDestination(hit.point);

                var pos = agent.transform.position;
                var movePacket = new PacketWriter<GameOpcode>(GameOpcode.MoveWorldEntity);
                movePacket.Write(pos.x);
                movePacket.Write(pos.y);
                movePacket.Write(pos.z);
                movePacket.Write(hit.point.x);
                movePacket.Write(hit.point.y);
                movePacket.Write(hit.point.z);
                Debug.Log("Sending move req.");
                //Registry.Get<WorldClient>().Send(movePacket);
            } else
            {
                Debug.Log("No Click Hit.");
            }
        }

        anim.SetFloat("DistanceRemaining", agent.remainingDistance);
    }
}
