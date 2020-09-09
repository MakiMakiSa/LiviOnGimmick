using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class FootEventManager : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    bool exitPlay = false;

    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<I_FootEvent>()?.Enter(transform.position);

        FootSEManager.PlayClip(0, transform.position , transform.rotation , rb.velocity.magnitude * 0.15f);
    }

    private void OnTriggerExit(Collider other)
    {
        if(exitPlay)
        {
            FootSEManager.PlayClip(0, transform.position, transform.rotation, rb.velocity.magnitude * 0.1f);
        }
        //        other.gameObject.GetComponent<I_FootEvent>()?.Enter(transform.position);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("FECollision");
    //    collision.gameObject.GetComponent<I_FootEvent>()?.Enter(transform.position);
    //}

}
