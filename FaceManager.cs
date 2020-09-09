using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceManager : MonoBehaviour
{
    [SerializeField]
    SkinnedMeshRenderer faceMesh;

    [SerializeField]
    PlayerManager pm;

    enum ShapeType
    {
        Null,
        Blink,
        Smile,
        Foo
    }
    ShapeType prevSt;
    ShapeType st;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        prevSt = st;
        if(pm.ballRB.velocity.y > 0.1f || pm.ballRB.velocity.magnitude > 1f)
        {
            st = ShapeType.Smile;
        }

        if(pm.ballRB.velocity.y < -0.1f)
        {
            if(pm.isAerial)
            {
                if (pm.ballRB.velocity.y < 0f)
                {
                    st = ShapeType.Foo;
                }
            }
            else
            {
                st = ShapeType.Null;
            }
        }

        //シェイプは重い、極力呼ばないようにする
        if(prevSt != st)
        {
            faceMesh.SetBlendShapeWeight((int)prevSt, 0f);
            faceMesh.SetBlendShapeWeight((int)st, 50f);

            //for (int i = 0; i < System.Enum.GetNames(typeof(ShapeType)).Length; i++)
            //{
            //    float w = st == (ShapeType)i ? 100f : 0f;

            //    faceMesh.SetBlendShapeWeight(i, w);
            //}
        }

    }
}
