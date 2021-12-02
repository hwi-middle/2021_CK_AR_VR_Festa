using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForthVillain_1 : NPC
{
    [SerializeField] private GameObject pick2;
    [SerializeField] private GameObject pick3;
    [SerializeField] private GameObject pick4;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(8));

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
        var pickInstance = Instantiate(pick); //빵 3개 생성
        yield return StartCoroutine(StartNextDialog(1));
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());
        PosSystem.currentState = POSSystem.EProceedState.Scanning;

        yield return StartCoroutine(StartNextDialog(2));
        yield return StartCoroutine(GoToSpot(5));
        yield return new WaitForSeconds(5.0f);
        yield return StartCoroutine(GoToSpot(12));
        AddCorrectPicks(pick2);
        var pick2Instance = Instantiate(pick2); //음료수 1개, 초콜릿 1개 생성
        yield return StartCoroutine(StartNextDialog(1));
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());
        
        PosSystem.currentState = POSSystem.EProceedState.Scanning;
        yield return StartCoroutine(StartNextDialog(1));
        yield return StartCoroutine(GoToSpot(6));
        yield return new WaitForSeconds(4.0f);
        yield return StartCoroutine(GoToSpot(12));
        AddCorrectPicks(pick3);
        var pick3Instance = Instantiate(pick3); //음료수 1개 생성
        yield return StartCoroutine(StartNextDialog(1));
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());

        PosSystem.currentState = POSSystem.EProceedState.Scanning;
        yield return StartCoroutine(StartNextDialog(2));
        yield return StartCoroutine(GoToSpot(7));
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(GoToSpot(12));
        AddCorrectPicks(pick4);
        var pick4Instance = Instantiate(pick4); //컵라면 2개 생성
        yield return StartCoroutine(StartNextDialog(1));
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());

        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.CreditCard);
        
        while (true) //승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(PosSystem.ProceedCreditCardPayment());
        
        Destroy(pickInstance);
        Destroy(pick2Instance);
        Destroy(pick3Instance);
        Destroy(pick4Instance);

        yield return StartCoroutine(StartNextDialog(2));

        StartCoroutine(GoToSpot(1));

        while (!Door.IsNpcEntered) //손님이 퇴장할 때 까지 대기
        {
            yield return null;
        }

        Finished = true;
    }
}