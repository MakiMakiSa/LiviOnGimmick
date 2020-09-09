using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemaChanger : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera cvc;

    private void OnTriggerEnter(Collider other)
    {
        cvc.Priority = 100;
    }

    private void OnTriggerExit(Collider other)
    {
        cvc.Priority = 0;
    }

}
