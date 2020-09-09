using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBuffer : MonoBehaviour
{
    public float receptionTime = 0.1f;

    float enterTime;
    float exitTime;

    public bool Enter
    {
        get
        {
            bool flag = enterTime > 0f;
            enterTime = 0f;
            if (flag) exitTime = 0f;
            return flag;
        }
    }
    public bool Exit
    {
        get
        {
            bool flag = exitTime > 0f;
            exitTime = 0f;
            if (flag) enterTime = 0f;
            return flag;
        }
    }


    public void SetEnter()
    {
        enterTime = receptionTime;
    }
    public void SetExit()
    {
        exitTime = receptionTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enterTime > 0f) enterTime -= Time.deltaTime;
        if (exitTime > 0f) exitTime -= Time.deltaTime;
    }
}
