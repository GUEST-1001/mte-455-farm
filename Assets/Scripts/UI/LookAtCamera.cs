using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        transform.eulerAngles = _cam.transform.eulerAngles;
    }
}
