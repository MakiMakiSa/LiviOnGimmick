using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager_PC : MonoBehaviour
{
    [SerializeField]
    float MouseSpeed = 0.01f;
    void GetKey()
    {

    }

    static public void lockCursor()
    {
#pragma warning disable CS0618 // 型またはメンバーが古い形式です
//        Screen.lockCursor = true;
#pragma warning restore CS0618 // 型またはメンバーが古い形式です
    }
    static public void freeCursor()
    {
#pragma warning disable CS0618 // 型またはメンバーが古い形式です
//        Screen.lockCursor = false;
#pragma warning restore CS0618 // 型またはメンバーが古い形式です
    }
    void Start()
    {
#pragma warning disable CS0618 // 型またはメンバーが古い形式です
//        Screen.lockCursor = true;
#pragma warning restore CS0618 // 型またはメンバーが古い形式です
    }


    public Vector3 GetStickVel(bool isLeft)
    {
        Vector3 vel = Vector3.zero;
        if (isLeft)
        {
            vel.x += Input.GetKey(KeyCode.D) ? 1f : 0f;
            vel.x -= Input.GetKey(KeyCode.A) ? 1f : 0f;
            vel.y += Input.GetKey(KeyCode.W) ? 1f : 0f;
            vel.y -= Input.GetKey(KeyCode.S) ? 1f : 0f;
        }
        else
        {
            vel.x = Input.GetAxis("Mouse X")*MouseSpeed;
            vel.y = -Input.GetAxis("Mouse Y") * MouseSpeed;

            if(vel.magnitude > 1f)
            {
                vel = vel.normalized;
            }

        }

        return vel;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        
    }
}
