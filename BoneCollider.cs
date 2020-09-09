using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCollider : MonoBehaviour
{
    Collider col;
    bool prevCollisionFlag = false;
    int count = 0;


    public bool isTrigger
    {
        get
        { return col.isTrigger; }
        set
        {
            col.isTrigger = value;
            prevCollisionFlag = true;
            count = 10;//（要修正箇所）
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();

        //    Rigidbody rb;
        //rb = gameObject.AddComponent<Rigidbody>();
        //rb.isKinematic = true;
        //rb.interpolation = RigidbodyInterpolation.Extrapolate;
        //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        //rb.mass = 1000000f;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (col.isTrigger)
        {
            if (!prevCollisionFlag)
            {
                count--;
                if (count <= 0)
                {
                    col.isTrigger = false;
                }
            }
            prevCollisionFlag = false;
        }

    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    collision.rigidbody.AddForce(collision.relativeVelocity , ForceMode.VelocityChange);
    //}

    private void OnTriggerEnter(Collider other)
    {
        prevCollisionFlag = true;
    }
    private void OnTriggerStay(Collider other)
    {

        prevCollisionFlag = true;
    }

}
