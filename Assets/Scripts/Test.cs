using UnityEngine;

public class Test : MonoBehaviour
{


    public Vector3 vel = Vector3.zero;
    
    public void Move(Transform target)
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref vel, 0.00001f);
    }
}
