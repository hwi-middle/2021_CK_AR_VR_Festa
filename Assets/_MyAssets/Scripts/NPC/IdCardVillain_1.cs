using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class IdCardVillain_1 : NPC
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
            KoreanName = "김철수",
            HanjaName = "金鐵水",
            ID = "320615-XXXXXXX",
            Address = "서울특별시 행복구\n" +
                      "평화로 93, 1203동 1234호\n" +
                      "(사랑동, 육개장아파트)",
            Date = "2021.11.25",
            Institution = "서울특별시 행복구청장",
            Pic = img
        };

        UnityAction pass = delegate { Continue = true; };
        UnityAction fail = delegate { Manager.DecreaseLife(); };

        idCard.nameButton.onClick.AddListener(fail);
        idCard.idButton.onClick.AddListener(pass);
        idCard.dateButton.onClick.AddListener(fail);
        idCard.picButton.onClick.AddListener(fail);
        idCard.passButton.onClick.AddListener(fail);

        Continue = false;
        idCard.gameObject.SetActive(true);

        while (!Continue) //올바른 입력 대기
        {
            yield return null;
        }

        idCard.nameButton.onClick.RemoveListener(fail);
        idCard.idButton.onClick.RemoveListener(pass);
        idCard.dateButton.onClick.RemoveListener(fail);
        idCard.picButton.onClick.RemoveListener(fail);
        idCard.passButton.onClick.RemoveListener(fail);

        idCard.gameObject.SetActive(false);

        yield return StartCoroutine(StartNextDialog(7));

        Reply1Button.onClick.AddListener(fail);
        Reply2Button.onClick.AddListener(DefaultReply2Btn);

        Continue = false;
        while (!Continue) //올바른 입력 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(2));

        Reply1Button.onClick.AddListener(fail);
        Reply2Button.onClick.AddListener(DefaultReply2Btn);
        Continue = false;
        while (!Continue) //올바른 입력 대기
        {
            yield return null;
        }
        var navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed *= 1.5f;

        StartCoroutine(GoToSpot(1));
        Finished = true;
    }
}