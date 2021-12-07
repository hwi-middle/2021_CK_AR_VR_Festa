using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vibtest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputManager.PlayVibration(InputManager.Controller.LTouch, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}