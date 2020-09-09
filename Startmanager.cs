using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100000)]
public class Startmanager : MonoBehaviour
{

    [SerializeField]
    List<GameObject> disableObjs = new List<GameObject>();

    [SerializeField]
    List<GameObject> enableObjs = new List<GameObject>();

    [SerializeField]
    float timer = 3f;


    bool firstFlag = true;
    // Update is called once per frame
    void Update()
    {
        if(firstFlag)
        {
            firstFlag = false;
            foreach (GameObject obj in disableObjs)
            {
                obj.SetActive(false);
            }
        }

        timer -= Time.deltaTime;
        if(timer < 0f)
        {
            foreach (GameObject obj in enableObjs)
            {
                obj.SetActive(true);
            }
            Destroy(gameObject);
        }


    }
}
