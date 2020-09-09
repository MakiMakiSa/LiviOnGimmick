using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[DefaultExecutionOrder(-1000)]
public class VFXPutterManager : MonoBehaviour
{
    static public VFXPutterManager main;

    [SerializeField]
    List<VisualEffect> ves = new List<VisualEffect>();

    static List<Transform> poss = new List<Transform>();

    int counter = 0;



    static public void Add(Transform t)
    {
        poss.Add(t);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!main) main = this;
    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        counter %= poss.Count;


        foreach (VisualEffect ve in ves)
        {
            ve.SetVector3("PlayerPosition", poss[counter].position);
        }
    }
}
