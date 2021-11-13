using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestVillain : Villain
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();        
        StartCoroutine(Act());
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    private IEnumerator Act()
    {
        yield return StartCoroutine(StartNextDialog());
        yield return StartCoroutine(StartNextDialog());
        yield return StartCoroutine(StartNextDialog());
        yield return StartCoroutine(StartNextDialog());
        yield return StartCoroutine(StartNextDialog());
        yield return StartCoroutine(StartNextDialog());

    }
}