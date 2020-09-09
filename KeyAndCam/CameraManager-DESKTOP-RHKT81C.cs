using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

//using UnityEngine.Rendering.PostProcessing;

    [DefaultExecutionOrder(-1000)]
public class CameraManager : MonoBehaviour
{

    Vector3 rsVel;
    Vector3 v;
    [SerializeField]
    float speed = 10f;


    CinemachineVirtualCamera cinema;

    void Start()
    {

    }

    void Move_3D_UpdateKey()
    {
        v = KeyManagerHub.main.GetStickVel(false);
        Vector3 vv = v;
        v.x = vv.y;
        v.y = vv.x;

        v.x += v.x;
        v.y += v.y;

        rsVel = Vector3.Lerp(rsVel, v, Time.deltaTime * 60f);

        Vector3 e = Camera.main.transform.eulerAngles;

        e = Vector3.Lerp(e, e + (rsVel * speed), Time.deltaTime * 3f);
        e.x = Mathaf.NormalizeRad(e.x);
        if (e.x > 75f) e.x = 75f;
        if (e.x < -75f) e.x = -75f;

        Camera.main.transform.eulerAngles = e;

    }


    private void Update()
    {
        Move_3D_UpdateKey();
        
    }

}
