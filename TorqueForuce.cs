using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueForuce : MonoBehaviour
{
    [SerializeField]
    Vector3 torque;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddTorque(torque * Time.deltaTime, ForceMode.VelocityChange);
    }
}
