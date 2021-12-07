using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BackAndForthVillain_2 : NPC
{
    [SerializeField] private GameObject pick2;
    [SerializeField] private GameObject pick3;
    [SerializeField] private GameObject pay2;

    [SerializeField] private GameObject receipt1;
    [SerializeField] private GameObject receipt2;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        PosSystem.currentState = POSSystem.EProceedState.None;
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(12));

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

        var pickInstance = Instantiate(pick); //과자 4개 생성
        yield return StartCoroutine(StartNextDialog(5));

        yield return StartCoroutine(GoToSpot(1));
        yield return new WaitForSeconds(8.0f);
        yield return StartCoroutine(GoToSpot(12));
        var receipt1Instance = Instantiate(receipt1);

        PosSystem.currentState = POSSystem.EProceedState.Refund;
        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.Refund);
        yield return StartCoroutine(StartNextDialog(1));

        var receipt1Info = receipt1Instance.transform.GetChild(0).GetComponent<Receipt>();
        while (true) //영수증 바코드를 스캔할 때 까지 대기
        {
            if (receipt1Info.isScanned)
            {
                break;
            }

            yield return null;
        }

        PosSystem.SetPopUpMessage("거래 확인됨", "확인 버튼을 눌러 환불처리를 완료하십시오");

        StartCoroutine(StartNextDialog(4));

        while (true) //확인 버튼을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.None)
            {
                PosSystem.currentState = POSSystem.EProceedState.Scanning;
                break;
            }

            yield return null;
        }

        Destroy(receipt1Instance);
        PosSystem.ClosePopUpWindow();
        ResetCorrectPicks();
        AddCorrectPicks(pick2); //오이칩 2개
        var payInstance = Instantiate(pay);
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());

        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.Cash);
        PosSystem.OpenCashBox();
        while (true) //돈을 돈통에 넣고 올바른 금액을 누른 뒤 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount == 3600 && payInstance.transform.childCount == 0)
                {
                    break;
                }

                Manager.DecreaseLife();
                PosSystem.currentState = POSSystem.EProceedState.Paying;
            }

            yield return null;
        }

        PosSystem.CloseCashBox();
        PosSystem.ClosePopUpWindow();

        Destroy(payInstance);
        Destroy(pickInstance);
        yield return StartCoroutine(GoToSpot(1));
        yield return new WaitForSeconds(6.0f);
        yield return StartCoroutine(GoToSpot(12));

        yield return StartCoroutine(StartNextDialog(4));
        var receipt2Instance = Instantiate(receipt2);
        var pick3Instance = Instantiate(pick3);
        var pay2Instance = Instantiate(pay2);
        PosSystem.currentState = POSSystem.EProceedState.Refund;
        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.Refund);

        var receipt2Info = receipt2Instance.transform.GetChild(0).GetComponent<Receipt>();
        while (true) //영수증 바코드를 스캔할 때 까지 대기
        {
            if (receipt2Info.isScanned)
            {
                break;
            }

            yield return null;
        }

        PosSystem.SetPopUpMessage("거래 확인됨", "확인 버튼을 눌러 환불처리를 완료하십시오");

        while (true) //확인 버튼을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.None)
            {
                PosSystem.currentState = POSSystem.EProceedState.Scanning;
                break;
            }

            yield return null;
        }

        Destroy(receipt2Instance);
        PosSystem.ClosePopUpWindow();
        ResetCorrectPicks();
        AddCorrectPicks(pick3); //오이칩 1개
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());

        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.Cash);
        PosSystem.OpenCashBox();
        while (true) //돈을 돈통에 넣고 올바른 금액을 누른 뒤 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount == 1800 && pay2Instance.transform.childCount == 0)
                {
                    break;
                }

                Manager.DecreaseLife();
                PosSystem.currentState = POSSystem.EProceedState.Paying;
            }

            yield return null;
        }

        PosSystem.ClosePopUpWindow();
        PosSystem.CloseCashBox();
        Destroy(pay2Instance);

        yield return StartCoroutine(StartNextDialog(1));
        Destroy(pick3Instance);
        StartCoroutine(GoToSpot(1));

        while (!Door.IsNpcEntered) //손님이 퇴장할 때 까지 대기
        {
            yield return null;
        }

        Finished = true;
    }
}