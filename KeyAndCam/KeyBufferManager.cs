using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KeyBufferManager : MonoBehaviour
{
    static public float receptionTime = 0.2f;

    static public KeyBufferManager main;
    static public List<KeyBuffer> keyBuffers = new List<KeyBuffer>();


    private void Start()
    {
        if (!KeyBufferManager.main) KeyBufferManager.main = this;

    }
    static public KeyBuffer CreateKey()
    {
        KeyBuffer buff = new KeyBuffer();
        KeyBufferManager.keyBuffers.Add(buff);

        return buff;
    }
    // Update is called once per frame
    void Update()
    {
        foreach(KeyBuffer kb in keyBuffers)
        {
            kb.update();
        }
    }













}



public class KeyBuffer
{


    float enterTime;//押した時用
    float exitTime;//離された時用、不要かも


    bool Enter
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

    public bool EnterCheck(bool flag)
    {
        if (!flag) return false;
        return Enter;
    }
    public void SetEnter()
    {
        enterTime = KeyBufferManager.receptionTime;
    }
    public void SetExit()
    {
        exitTime = KeyBufferManager.receptionTime;
    }


    public void update()
    {
        if (enterTime > 0f) enterTime -= Time.deltaTime;
        if (exitTime > 0f) exitTime -= Time.deltaTime;
    }
}

