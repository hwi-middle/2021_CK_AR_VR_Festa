using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTestNpc : NPC
{

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(GoToSpot(1));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
