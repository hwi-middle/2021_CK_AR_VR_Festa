using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal_1 : NPC
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(4));

        while (!Door.IsNpcEntered) //손님이 입장할 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(2));

        while (true) //손님이 이동을 마칠 때 까지 대기
        {
            if (IsNavMeshAgentReachedDestination())
            {
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(GoToSpot(12));

        yield return StartCoroutine(StartNextDialog(1));

        //음료수 1개, 과자 1개 생성
        GameObject pickInstance = Instantiate(pick);

        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());


        GameObject payInstance = Instantiate(pay);

        PosSystem.OpenCashBox();
        while (true) //돈을 돈통에 넣고 올바른 금액을 누른 뒤 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount == 4000 && payInstance.transform.childCount == 0)
                {
                    break;
                }

                Manager.DecreaseLife();
                PosSystem.currentState = POSSystem.EProceedState.Paying;
            }

            yield return null;
        }
        PosSystem.CloseCashBox();

        Destroy(payInstance);
        yield return StartCoroutine(StartNextDialog(2));
        Destroy(pickInstance);

        StartCoroutine(GoToSpot(1));

        while (!Door.IsNpcEntered) //손님이 퇴장할 때 까지 대기
        {
            yield return null;
        }

        Finished = true;
    }
}