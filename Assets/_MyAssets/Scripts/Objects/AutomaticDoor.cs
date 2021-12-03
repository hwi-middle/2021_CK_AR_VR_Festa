using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    private bool _isNpcEntered;
    [SerializeField] private Animator animator;
    
    public bool IsNpcEntered => _isNpcEntered;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            _isNpcEntered = true;
            animator.SetTrigger("enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            _isNpcEntered = false;
            animator.SetTrigger("exit");
        }
    }
}