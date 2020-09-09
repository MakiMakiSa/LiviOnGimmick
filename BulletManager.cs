using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[DefaultExecutionOrder(1000)]
public class BulletManager : MonoBehaviour
{
    [SerializeField]
    VisualEffect ve;

    // Start is called before the first frame update
    void Start()
    {
        Bullet_VFX.ve = ve;
        Bullet_VFX.Init();
    }

    // Update is called once per frame
    void Update()
    {
        Bullet_VFX.UpdateMap();
    }
}
