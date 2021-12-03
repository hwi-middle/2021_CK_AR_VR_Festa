using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVillain_1 : NPC
{
    [SerializeField] private GameObject pay2;
    [SerializeField] private GameObject pay3;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(3));

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

        yield return StartCoroutine(GoToSpot(6));
        yield return new WaitForSeconds(2.0f);

        yield return StartCoroutine(GoToSpot(8));
        yield return new WaitForSeconds(3.0f);

        yield return StartCoroutine(GoToSpot(12));

        //음료수 1개, 빵 1개, 초콜릿 1개 생성
        GameObject pickInstance = Instantiate(pick);
        yield return StartCoroutine(StartNextDialog(2));

        while (true) //올바르게 상품을 스캔한 상태에서 확인 버튼 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Paying)
            {
                if (CheckScannedCorrectly())
                {
                    break;
                }

                Manager.DecreaseLife();
                PosSystem.currentState = POSSystem.EProceedState.Scanning;
            }

            yield return null;
        }

        var forcePaying = ForcePaying();
        StartCoroutine(forcePaying);
        
        yield return StartCoroutine(StartNextDialog(2));

        //500원짜리 동전 14개 생성
        GameObject payInstance = Instantiate(pay);
        yield return StartCoroutine(StartNextDialog(1));

        //100원짜리 동전 6개 생성
        GameObject pay2Instance = Instantiate(pay2);
        yield return StartCoroutine(StartNextDialog(1));
        
        while (true) //돈을 돈통에 모두 넣을 때 까지 대기
        {
            // if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            // {
            //     PosSystem.currentState = POSSystem.EProceedState.Paying;
            // }

            if (payInstance.transform.childCount == 0 && pay2Instance.transform.childCount == 0)
            {
                break;
            }

            yield return null;
        }

        //100원짜리 동전 1개 생성
        yield return StartCoroutine(StartNextDialog(5));
        GameObject pay3Instance = Instantiate(pay3);
        StopCoroutine(forcePaying);

        while (true) //올바른 금액을 누른 뒤 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount == 7700 && pay3Instance.transform.childCount == 0)
                {
                    break;
                }

                Manager.DecreaseLife();
                PosSystem.currentState = POSSystem.EProceedState.Paying;
            }

            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(2));

        StartCoroutine(GoToSpot(1));

        while (!Door.IsNpcEntered) //손님이 퇴장할 때 까지 대기
        {
            yield return null;
        }

        Destroy(pickInstance);
        Destroy(payInstance);
        Destroy(pay2Instance);
        Destroy(pay3Instance);

        Finished = true;
    }

    private IEnumerator ForcePaying()
    {
        while (true)
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                Manager.DecreaseLife();
                PosSystem.currentState = POSSystem.EProceedState.Paying;
                PosSystem.ClaerChangeText();
            }

            yield return null;
        }
    }
}