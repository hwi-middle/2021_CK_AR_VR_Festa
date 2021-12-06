using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Normal_3 : NPC
{
    [SerializeField] private IdCard idCard;
    [SerializeField] private Sprite img;

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

        yield return StartCoroutine(StartNextDialog(2));

        while (true) //손님이 이동을 마칠 때 까지 대기
        {
            if (IsNavMeshAgentReachedDestination())
            {
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(3));

        //신분증 UI 활성화
        idCard.Info = new IdCard.IdInfo()
        {
            KoreanName = "유개장에밤마라모고수 ",
            HanjaName = "柳開帳曀湴馬裸毛拷隋",
            ID = "990615-XXXXXXX",
            Address = "서울특별시 사랑구\n" +
                      "온새미로 11, 101동 1101호\n" +
                      "(아몬드동, 크런치아파트)",
            Date = "2021.11.25",
            Institution = "서울특별시 사랑구청장",
            Pic = img
        };
        StartCoroutine(StartNextDialog(1));

        UnityAction pass = delegate { Continue = true; };
        UnityAction fail = delegate { Manager.DecreaseLife(); };

        idCard.nameButton.onClick.AddListener(pass);
        idCard.idButton.onClick.AddListener(fail);
        idCard.dateButton.onClick.AddListener(fail);
        idCard.picButton.onClick.AddListener(fail);
        idCard.passButton.onClick.AddListener(pass);

        Continue = false;
        idCard.gameObject.SetActive(true);

        while (!Continue) //올바른 입력 대기
        {
            yield return null;
        }

        idCard.nameButton.onClick.RemoveListener(pass);
        idCard.idButton.onClick.RemoveListener(fail);
        idCard.dateButton.onClick.RemoveListener(fail);
        idCard.picButton.onClick.RemoveListener(fail);
        idCard.passButton.onClick.RemoveListener(pass);

        idCard.gameObject.SetActive(false);

        yield return StartCoroutine(StartNextDialog(2));

        bool doExtraConversation = false;
        UnityAction extraConversation = delegate
        {
            DeactivateButtons();
            doExtraConversation = true;
            Continue = true;
        };

        Reply1Button.onClick.AddListener(extraConversation);
        Reply2Button.onClick.AddListener(DefaultReply2Btn);
        
        Continue = false;
        while (!Continue) //올바른 입력 대기
        {
            yield return null;
        }

        Reply1Button.onClick.RemoveListener(extraConversation);
        Reply2Button.onClick.RemoveListener(DefaultReply2Btn);
        
        if (doExtraConversation)
        {
            yield return StartCoroutine(StartNextDialog(5));
        }
        
        yield return StartCoroutine(StartNextDialog(1));
        var pickInstance = Instantiate(pick);
        yield return StartCoroutine(StartNextDialog(1));
        yield return WaitUntilScanCorrectlyAndApply();
        
        yield return StartCoroutine(StartNextDialog(1));
        var payInstance = Instantiate(pay);
        PosSystem.OpenPopUpWindow(POSSystem.EPosPopUp.Cash);
        PosSystem.OpenCashBox();
        yield return StartCoroutine(StartNextDialog(1));

        while (true) //올바른 금액을 누른 뒤 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount == 4500 && payInstance.transform.childCount == 0)
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
        Destroy(payInstance);
        yield return StartCoroutine(GoToSpot(1));
        
        Finished = true;
    }
}