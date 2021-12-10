using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinVillain_2 : NPC
{
    [SerializeField] private GameObject pay2;
    [SerializeField] private GameObject pay3;
    [SerializeField] private GameObject pay4;

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

        var pickInstance = Instantiate(pick);
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());
        PosSystem.OpenCashBox();
        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.Cash);
        yield return StartCoroutine(StartNextDialog(2));


        //500원 동전 15개 생성
        var payInstance = Instantiate(pay);
        yield return StartCoroutine(StartNextDialog(1));

        //100원 동전 14개 생성
        var pay2Instance = Instantiate(pay2);
        yield return StartCoroutine(StartNextDialog(1));

        while (payInstance.transform.childCount != 0 || pay2Instance.transform.childCount != 0)
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(1));

        UnityAction fail = delegate { Manager.DecreaseLife(); };

        Reply1Button.onClick.AddListener(DefaultReply1Btn);
        Reply2Button.onClick.AddListener(fail);

        Continue = false;
        while (!Continue) //올바른 입력 대기
        {
            yield return null;
        }

        Reply1Button.onClick.RemoveListener(DefaultReply1Btn);
        Reply2Button.onClick.RemoveListener(fail);
        yield return StartCoroutine(StartNextDialog(2));

        //어린이 은행 지폐 3개 생성
        var pay3Instance = Instantiate(pay3);

        yield return StartCoroutine(StartNextDialog(4));
        Reply1Button.onClick.AddListener(fail);
        Reply2Button.onClick.AddListener(DefaultReply2Btn);

        Continue = false;
        while (!Continue) //올바른 입력 대기
        {
            yield return null;
        }
        Reply1Button.onClick.RemoveListener(fail);
        Reply2Button.onClick.RemoveListener(DefaultReply2Btn);

        yield return StartCoroutine(StartNextDialog(2));
        //1000원 지폐 3개 생성
        var pay4Instance = Instantiate(pay4);
        yield return StartCoroutine(StartNextDialog(1));

        while (true) //올바른 금액을 누른 뒤 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount == 11900 && pay4Instance.transform.childCount == 0)
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

        yield return StartCoroutine(StartNextDialog(2));
        Destroy(pickInstance);
        Destroy(pay3Instance);

        yield return StartCoroutine(GoToSpot(1));

        Destroy(payInstance);
        Destroy(pay2Instance);
        Destroy(pay4Instance);

        Finished = true;
    }
}