using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBox : MonoBehaviour
{
    [SerializeField]
    GameObject BulletPrf;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Shot", 1f, 0.03f);
    }

    void Shot()
    {
        GameObject obj =  Instantiate(BulletPrf);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;

        obj.GetComponent<Rigidbody>().AddForce(transform.forward*0.1f, ForceMode.Impulse);

        Destroy(obj, 30f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
