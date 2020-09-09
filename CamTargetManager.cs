using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CamTargetManager : MonoBehaviour
{
    PositionConstraint constraint;

    public float ballWeight;

    // Start is called before the first frame update
    void Start()
    {
        constraint = GetComponent<PositionConstraint>();


    }

    // Update is called once per frame
    void Update()
    {
        ConstraintSource Current = constraint.GetSource(0);
        ConstraintSource Option = constraint.GetSource(1);


        //float d = Vector3.Distance(Current.sourceTransform.position, Option.sourceTransform.position);

        //float offset = 3f;
        //d = Mathf.Clamp(d, 0f, offset);
        //d /= offset;

        Current.weight = 1;
        Option.weight = 0;


        Current.weight -= ballWeight;
        Option.weight += ballWeight;

        //Current.weight = Mathf.Clamp(Current.weight, 0f, 1f);
        //Option.weight = Mathf.Clamp(Option.weight, 0f, 1f);


        constraint.SetSource(0, Current);
        constraint.SetSource(1, Option);
    }
}
