using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private float Timer = 0f;
    private float limit = 1f;
    private int n = 1;
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        Debug.Log($"{n} : {Time.deltaTime}");
        n++;

        if (Timer > limit)
        {
            Debug.Log("1 sec.");
            Timer = 0f;
        }
    }
}
