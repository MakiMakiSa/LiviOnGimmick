using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelVel : MonoBehaviour
{
    List<float> vels = new List<float>();


    public float vel
    {
        set
        {
            Debug.Log("set");
            vels[0] = value;
            Debug.Log("setA");
        }

        get
        {
            Debug.Log("get");
            float res = vels[vels.Count-1];
            Debug.Log("getA");

            return res;
        }
    }
    public void setLv(int lv)
    {
        Debug.Log("LV");
        vels = new List<float>();
        for(int i=0; i< lv; i++)
        {
            Debug.Log("L"+i);

            vels.Add(0f);
            Debug.Log("L" + i+"OK");
        }

        Debug.Log("LVa");

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("UD");

        for (int i=1; i< vels.Count; i++)
        {
            Debug.Log("UD" + i);
            vels[i] += vels[i - 1];
            Debug.Log("UD" + i + "OK");
        }
        Debug.Log("UD"  + "OK");
    }

}
