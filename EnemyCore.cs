using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyCore : MonoBehaviour
{

    Animator animator;

    int anm_MoveMode;

    float lookWeight;

    NavMeshAgent nm;
    // Start is called before the first frame update
    void Start()
    {
        nm = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        anm_MoveMode = Animator.StringToHash("moveMode");
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 toPos = (transform.position - PlayerIKManager.main.transform.position);
        bool lookFlag = false;
        if (toPos.magnitude < 10f)
        {
            nm.SetDestination(toPos.normalized + transform.position);

            if (nm.velocity.magnitude > 0f)
            {
                //逃げる
                animator.SetInteger(anm_MoveMode, 3);
            }
            else
            {
                //ムリぽ
                animator.SetInteger(anm_MoveMode, 2);
                lookFlag = true;
            }
        }
        else
        {
            //ヨユー
            animator.SetInteger(anm_MoveMode, 1);
            lookFlag = true;
        }

        lookWeight += lookFlag? Time.deltaTime * 3f: Time.deltaTime * -3f;
        lookWeight = Mathf.Clamp(lookWeight, 0f, 1f);
        Vector3 lookVel = PlayerIKManager.main.transform.position - transform.position;
        lookVel.y = 0f;
        Quaternion q =  Quaternion.LookRotation(lookVel);

        transform.rotation = Quaternion.Lerp(transform.rotation, q, lookWeight);





    }

}
