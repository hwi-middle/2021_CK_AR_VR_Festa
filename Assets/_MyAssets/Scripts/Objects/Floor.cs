using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private GameObject respawnSpot;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Cash":
            case "Goods":
            case "Scanner":
            case "Receipt":
                other.gameObject.transform.position = respawnSpot.transform.position;
                break;
        }
    }
}