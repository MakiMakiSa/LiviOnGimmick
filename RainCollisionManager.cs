using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RainCollisionManager : MonoBehaviour
{
    [SerializeField]
    Transform head;

    VisualEffect ve;
    // Start is called before the first frame update
    void Start()
    {
        ve = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        ve.SetVector3("position", head.position);
        
    }
}
