using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    Rigidbody rb;


    [SerializeField]
    GameObject destroyEffect;

    Vector3 prevPos;

    [SerializeField]
    LayerMask mask;

    // Start is called before the first frame update
    public void Init()
    {
        
        rb.velocity = transform.forward * speed;

        prevPos = transform.position;
        Destroy(gameObject, 3f);

    }

    private void FixedUpdate()
    {
        if(Physics.Linecast(prevPos  , transform.position , out RaycastHit hit , mask))
        {
            Destroy(hit);


            //当たったものにBreakSystemがついてれば跡を残す
            BreakSystem bs = hit.transform.GetComponent<BreakSystem>();
            if (bs)
            {
                bs.BreakAtPoint(hit.point, null);
            }

        }

        prevPos = transform.position;
    }


    void Destroy(RaycastHit hit)
    {

        Quaternion q = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward , hit.normal) + hit.normal);


        GameObject obj = GameObject.Instantiate(destroyEffect , hit.point , q/*,  collision.transform*/);
        Destroy(obj, 5f);
        Destroy(gameObject);
    }
}
