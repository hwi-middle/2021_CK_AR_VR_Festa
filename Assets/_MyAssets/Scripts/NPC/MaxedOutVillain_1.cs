using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxedOutVillain_1 : NPC
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Act());
    }

    // Update is called once per frame
    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(10));

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

        yield return new WaitForSeconds(4.0f);
        yield return StartCoroutine(GoToSpot(2));
        yield return new WaitForSeconds(5.0f);
        yield return StartCoroutine(GoToSpot(6));
        yield return new WaitForSeconds(6.0f);
        yield return StartCoroutine(GoToSpot(12));

        var pickInstance = Instantiate(pick);
        yield return StartCoroutine(StartNextDialog(2));
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

        PosSystem.SetCreditCardPaymentAndRefresh();
        PosSystem.currentState = POSSystem.EProceedState.None;

        yield return StartCoroutine(StartNextDialog(1));
        yield return StartCoroutine(ShowMaxedOutPopUp());
        yield return StartCoroutine(StartNextDialog(5));

        //과자 5개 제거
        for (int i = 0; i < 5; i++)
        {
            DestroyImmediate(pickInstance.transform.GetChild(0).gameObject);
        }

        PosSystem.ResetGoodsAndRefresh();
        ResetCorrectPicks();
        AddCorrectPicks(pickInstance);

        yield return StartCoroutine(GoToSpot(6));
        yield return new WaitForSeconds(5.0f);
        yield return StartCoroutine(GoToSpot(12));
        PosSystem.currentState = POSSystem.EProceedState.Scanning;

        yield return StartCoroutine(StartNextDialog(4));
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

        PosSystem.SetCreditCardPaymentAndRefresh();
        PosSystem.currentState = POSSystem.EProceedState.None;
        yield return StartCoroutine(ShowMaxedOutPopUp());
        yield return StartCoroutine(StartNextDialog(3));

        //라면 3개 제거
        for (int i = 0; i < 3; i++)
        {
            DestroyImmediate(pickInstance.transform.GetChild(0).gameObject);
        }

        PosSystem.ResetGoodsAndRefresh();
        ResetCorrectPicks();
        AddCorrectPicks(pickInstance);

        yield return StartCoroutine(GoToSpot(10));
        yield return new WaitForSeconds(5.0f);
        yield return StartCoroutine(GoToSpot(12));
        PosSystem.currentState = POSSystem.EProceedState.Scanning;

        yield return StartCoroutine(StartNextDialog(4));
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

        PosSystem.SetCreditCardPaymentAndRefresh();
        PosSystem.currentState = POSSystem.EProceedState.None;

        yield return StartCoroutine(ShowMaxedOutPopUp());
        yield return StartCoroutine(StartNextDialog(4));

        //음료수 2개 제거
        for (int i = 0; i < 2; i++)
        {
            DestroyImmediate(pickInstance.transform.GetChild(0).gameObject);
        }

        PosSystem.ResetGoodsAndRefresh();
        ResetCorrectPicks();
        AddCorrectPicks(pickInstance);

        yield return StartCoroutine(GoToSpot(2));
        yield return new WaitForSeconds(5.0f);
        yield return StartCoroutine(GoToSpot(12));
        PosSystem.currentState = POSSystem.EProceedState.Scanning;

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
        yield return StartCoroutine(StartNextDialog(3));
        yield return StartCoroutine(GoToSpot(1));

        Finished = true;
    }

    private IEnumerator ShowMaxedOutPopUp()
    {
        PosSystem.SetPopUpMessage("카드결제", "IC 카드 정보 읽는 중");
        yield return new WaitForSeconds(1.0f);

        PosSystem.SetPopUpMessage("카드결제", "연결 중");
        yield return new WaitForSeconds(0.8f);

        PosSystem.SetPopUpMessage("카드결제", "데이터 처리 중");
        yield return new WaitForSeconds(1.5f);

        PosSystem.SetPopUpMessage("카드결제", "응답전문 수신완료");
        yield return new WaitForSeconds(0.3f);

        PosSystem.SetPopUpMessage("카드결제", "ACK 전송완료");
        yield return new WaitForSeconds(0.5f);

        PosSystem.SetPopUpMessage("승인거절", "거절 사유: 한도초과");
        yield return new WaitForSeconds(2.0f);

        PosSystem.ClosePopUpWindow();
    }
}