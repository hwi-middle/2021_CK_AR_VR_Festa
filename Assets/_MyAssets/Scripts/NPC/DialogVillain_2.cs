using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogVillain_2 : NPC
{
    [SerializeField] private GameObject pay2;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        StartCoroutine(GoToSpot(9));

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

        var pickInstance = Instantiate(pick);
        yield return StartCoroutine(WaitUntilScanCorrectlyAndApply());
        PosSystem.OpenCashBox();
        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.Cash);

        yield return StartCoroutine(StartNextDialog(9));
        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.Cash);
        PosSystem.OpenCashBox();
        var payInstance = Instantiate(pay);
        yield return StartCoroutine(StartNextDialog(1));

        UnityAction fail = delegate
        {
            Manager.DecreaseLife();
            DefaultReply1Btn();
        };

        Reply1Button.onClick.AddListener(fail);
        Reply2Button.onClick.AddListener(DefaultReply2Btn);

        Continue = false;
        while (!Continue) //입력 대기
        {
            yield return null;
        }

        Reply1Button.onClick.RemoveListener(fail);
        Reply2Button.onClick.RemoveListener(DefaultReply2Btn);

        yield return StartCoroutine(StartNextDialog(6));

        var pay2Instance = Instantiate(pay2);
        yield return StartCoroutine(StartNextDialog(1));

        while (true) //돈을 돈통에 넣고 올바른 금액을 누른 뒤 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount == 6000 && payInstance.transform.childCount == 0 && pay2Instance.transform.childCount == 0)
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
        yield return StartCoroutine(StartNextDialog(6));
        Destroy(pickInstance);
        Destroy(payInstance);
        Destroy(pay2Instance);
        StartCoroutine(GoToSpot(1));

        while (!Door.IsNpcEntered) //손님이 퇴장할 때 까지 대기
        {
            yield return null;
        }


        Finished = true;
    }
}