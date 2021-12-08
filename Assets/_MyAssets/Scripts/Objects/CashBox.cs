using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashBox : MonoBehaviour
{
    private POSSystem _posSystem;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _posSystem = POSSystem.Instance;
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Cash") && _posSystem.currentState == POSSystem.EProceedState.Paying)
        {
            _audioSource.Play();
            Destroy(other.gameObject);
        }
    }
}