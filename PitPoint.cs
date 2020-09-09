using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class PitPoint : MonoBehaviour
{
    static public List<PitPoint> pits = new List<PitPoint>();

    static int sid_EmitColor = Shader.PropertyToID("Emit");
    static int sid_EmitLv = Shader.PropertyToID("EmitLv");

   

    static public PitPoint search(Transform t)
    {
        var p = pits.FindAll(b => b.pilot == t).ToList();
        if (p.Count > 0)
        {
            var distList = p.OrderBy(b => Vector3.Distance(b.transform.position, t.position)).ToList();
            return distList[0];
        }
        return null;
    }

    static public void UD()
    {
        foreach(PitPoint p in pits)
        {
            p.ud();
        }
    }
    //------------------------------------------------
    [SerializeField]
    Transform Root;

    List<Renderer> rends = new List<Renderer>();
    Transform pilot = null;
    public bool isRideOn = false;


    [SerializeField]
    Vector3 cameraOffset;
    public Vector3 CameraOffset => cameraOffset;

    private void Start()
    {

        rends.AddRange(Root.gameObject.GetComponentsInChildren<Renderer>());


        pits.Add(this);
    }


    public void ud()
    {
        var mpb = new MaterialPropertyBlock();
        mpb.SetColor(sid_EmitColor, Color.black);
        mpb.SetFloat(sid_EmitLv, 0f);

        isRideOn = false;
        if (pilot)
        {
            if (!pilot.gameObject.activeInHierarchy)
            {
                pilot = null;
            }
            else
            {
                isRideOn = Vector3.Distance(transform.position , pilot.position) < 0.1f;

                if(isRideOn)
                {
                    mpb.SetColor(sid_EmitColor, Color.blue);
                    mpb.SetFloat(sid_EmitLv, 200f);
                }
                else
                {
                    mpb.SetColor(sid_EmitColor, Color.red);
                    mpb.SetFloat(sid_EmitLv, 50f);
                }

            }


        }


        foreach (Renderer r in rends)
        {
            r.SetPropertyBlock(mpb);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        pilot = other.transform;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(!pilot)
    //    {
    //        pilot = other.transform;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == pilot)
        {
            pilot = null;
        }

    }



}
