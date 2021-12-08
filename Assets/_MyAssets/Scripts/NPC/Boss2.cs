using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : NPC
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        PosSystem.currentState = POSSystem.EProceedState.None;
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        yield return StartCoroutine(GoToSpot(12));

        yield return StartCoroutine(StartNextDialog(40));
    }
}
