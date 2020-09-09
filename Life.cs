using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
    [SerializeField]
    PlayerManager parent;

    [SerializeField]
    float speed;

    Vector3 iVel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        iVel = Vector3.Lerp(iVel, parent.currentInputVel, Time.deltaTime*speed);
        transform.position = parent.transform.position + parent.upNormal + iVel;
        
    }
}
