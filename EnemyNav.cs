using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNav : MonoBehaviour
{
    public Vector3 tPos;

    [SerializeField]
    LayerMask mask;

    public Transform thisTrans;
    public Transform target;

    NavMeshAgent NMA;


    public Vector3 tVel;


    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        NMA = GetComponent<NavMeshAgent>();
        NMA.radius = 1f;


        Renderer rend = GetComponent<Renderer>();
        if (rend) { rend.enabled = false; }
        Collider col = GetComponent<Collider>();
        if (col) { col.enabled = false; }


        lr = gameObject.AddComponent<LineRenderer>();


        Vector3 pos = thisTrans.position;
        transform.position = pos;



    }


    // Update is called once per frame
    void FixedUpdate()
    {
        NMA.Warp(thisTrans.position);

        //道を検索
        NavMeshPath nmp = new NavMeshPath();
        if (NMA.CalculatePath(target.position, nmp))
        {
            switch (nmp.status)
            {
                case NavMeshPathStatus.PathComplete://完了
                    {
                        tPos = target.position;
                    }
                    break;
                case NavMeshPathStatus.PathInvalid://無効
                    {
                    }
                    break;
                case NavMeshPathStatus.PathPartial://途中
                    {
                        if (nmp.corners.Length > 1)
                        {
                            tPos = nmp.corners[1];
                        }
                        else
                        {
                            tPos = nmp.corners[0];
                        }
                        //目標を一番近い辺から法線方向に移動
                        if (NMA.FindClosestEdge(out NavMeshHit hitEdge))
                        {
                            tPos += (hitEdge.normal * NMA.radius * 4);
                        }
                    }
                    break;
                default: { }break;
            }

        }



        Vector3[] poss = { Vector3.zero, Vector3.zero };
        poss = new Vector3[] { thisTrans.position, tPos };
        lr.SetPositions(poss);


    }
}
