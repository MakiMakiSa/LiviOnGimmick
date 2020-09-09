using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamModeChanger : MonoBehaviour
{

    public Transform[] Lines;

    [SerializeField]
    CameraManager.CamMode mode;
    [SerializeField]
    bool D2Flag = false;

    CameraManager cm;
    // Use this for initialization
    void Start()
    {
        cm = CameraManager.main;

        foreach(Transform l in Lines)
        {
            l.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (Lines.Length > 0)
        {
            cm.SetD2Lines(Lines);
        }
        if(D2Flag)
        {
        }
        else
        {
        }
    }
}
