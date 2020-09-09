using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position3DList : MonoBehaviour
{
    class Node3D
    {
        

        public Node3D L;
        public Node3D R;
        public Node3D U;
        public Node3D D;
        public Node3D F;
        public Node3D B;

        public Vector3 Position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Set(Vector3[] positions)
    {
        //ノード初期化
        Node3D[] nodes = new Node3D[positions.Length];
        for(int i=0; i< positions.Length;i++)
        {
            nodes[i].Position = positions[i];
        }

        //
        foreach(Node3D current in nodes)
        {
            foreach (Node3D node in nodes)
            {
                if(current != node)
                {
                    //if(current.L)
                    //{

                    //}

                }
            }
        }


        float l = float.MinValue;
        float r = float.MaxValue;

        Vector3 lPos = Vector3.zero;bool lFlag = false;
        Vector3 rPos = Vector3.zero; bool rFlag = false;

        //foreach (Vector3 v in positions)
        //{
        //    float x = v.x - current.x;
        //    if(x > 0f)
        //    {
        //        if(x < r)
        //        {
        //            r = x;
        //            rPos = v;
        //            rFlag = true;
        //        }
        //    }
        //    else
        //    {
        //        if (x > l)
        //        {
        //            l = x;
        //            lPos = v;
        //            rFlag = true;
        //        }
        //    }
        //}

        //if (rFlag) list.Add(rPos);
        //if (lFlag) list.Add(lPos);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
