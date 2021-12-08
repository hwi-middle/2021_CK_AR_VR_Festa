using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationVillain_3 : NPC
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(12));

        while (!Door.IsNpcEntered) //손님이 입장할 때 까지 대기
        {
            yield return null;
        }
        
        yield return StartCoroutine(StartNextDialog(1));

        while (true) //손님이 이동을 마칠 때 까지 대기
        {
            if (IsNavMeshAgentReachedDestination())
            {
                break;
            }

            yield return null;
        }
        
        yield return StartCoroutine(StartNextDialog(5));
        
    }
}
