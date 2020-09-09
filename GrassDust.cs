using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GrassDust : MonoBehaviour,I_FootEvent
{
    public VisualEffect effect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enter(Vector3 pos)
    {
        effect.transform.position = pos;
        effect.SendEvent("Fire");
    }
}
