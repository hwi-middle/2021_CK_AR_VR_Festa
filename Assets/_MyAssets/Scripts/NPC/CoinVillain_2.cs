using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVillain_2 : NPC
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(7));

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

        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(GoToSpot(9));
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(GoToSpot(5));
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(GoToSpot(12));
        
        yield return StartCoroutine(StartNextDialog(1));
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());

        var payInstance = Instantiate(pay);
        
    }
}