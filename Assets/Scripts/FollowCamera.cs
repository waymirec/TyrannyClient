using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset = new Vector3(2, 2, 2); //Vector3.zero;

    void LateUpdate()
    {
        if (target != null && offset != Vector3.zero)
        {
            transform.position = target.transform.position + offset;
        }
    }
}
