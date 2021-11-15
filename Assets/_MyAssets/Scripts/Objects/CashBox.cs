using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashBox : MonoBehaviour
{
    private POSSystem _posSystem;

    // Start is called before the first frame update
    void Start()
    {
        _posSystem = POSSystem.Instance;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Cash"))
        {
            Destroy(other.gameObject);
        }
    }
}