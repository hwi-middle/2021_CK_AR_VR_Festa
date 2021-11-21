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

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(4));
        
        
        while (!Door.IsNpcEntered) //손님이 입장할 때 까지 대기
        {
            Debug.Log("입장 대기");
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
        
        
        while (true) //올바른 금액을 누르고 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount != 3500)
                {
                    PosSystem.currentState = POSSystem.EProceedState.Paying;
                    SetIndexTo(1001);
                    yield return StartCoroutine(StartNextDialog(1));
                }
                else
                {
                    break;
                }
            }
            yield return null;
        }
    }
}
