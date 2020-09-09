using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AvaterTiltManager : MonoBehaviour
{
    Animator animator;
    int anmID_tilt_X = Animator.StringToHash("tilt_X");
    int anmID_tilt_Z = Animator.StringToHash("tilt_Z");

    int layerID;
    void Start()
    {
        animator = GetComponent<Animator>();

        layerID = animator.GetLayerIndex("Tilt");
    }

    // Update is called once per frame
    void Update()
    {

        float z = Vector3.Dot(transform.forward, Vector3.up);
        float x = Vector3.Dot(transform.right, Vector3.up);

        animator.SetFloat(anmID_tilt_X, x);
        animator.SetFloat(anmID_tilt_Z, z);

        animator.SetLayerWeight(layerID, new Vector2(x, z).magnitude*1.5f);


    }
}
