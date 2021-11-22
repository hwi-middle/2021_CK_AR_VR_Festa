using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Boss : NPC
{
    public BarcodeScanner scanner;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        PosSystem.forceScanningMode = true;
        StartCoroutine(Act());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator Act()
    {
        yield return StartCoroutine(GoToSpot(12));

        yield return StartCoroutine(StartNextDialog(3));

        Continue = false;
        bool skip = false;
        
        UnityAction fn;
        Reply1Button.onClick.AddListener(DefaultReply1Btn);
        Reply2Button.onClick.AddListener(fn = delegate
        {
            skip = true;
            Continue = true;
        });
        
        while (!Continue) //튜토리얼 스킵여부 버튼 입력 대기
        {
            yield return null;
        }
        
        Reply1Button.onClick.RemoveListener(DefaultReply1Btn);
        Reply2Button.onClick.RemoveListener(fn);
        
        if (skip)
        {
            DeactivateButtons();
            SetIndexTo(1002);
            yield return StartCoroutine(StartNextDialog(1));
            yield return new WaitForSeconds(3.0f);
            yield return StartCoroutine(GoToSpot(1));
            PosSystem.forceScanningMode = false;
            Finished = true;
            yield break;
        }

        yield return StartCoroutine(StartNextDialog(3));

        GameObject pickInstance = Instantiate(pick); //스캔해볼 과자 오브젝트 생성

        yield return StartCoroutine(StartNextDialog(2));
        while (!scanner.GetComponent<OVRGrabbable>().isGrabbed) //스캐너를 집을 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(3));
        while (PosSystem.IsEmpty) //물건을 스캔할 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(3));
        PosSystem.forceScanningMode = false;
        while (PosSystem.currentState != POSSystem.EProceedState.Paying || PosSystem.TotalPrice != 800) //1개의 상품만 찍은 상태에서 확인 버튼 누를 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(1));
        var payInstance = Instantiate(pay); //집어들 천원권 오브젝트 생성
        while (!payInstance.GetComponent<OVRGrabbable>().isGrabbed) //천원권을 집을 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(1));
        while (payInstance != null) //천원권이 CashBox에 닿아 Destroy될 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(3));
        while (true) //올바른 금액을 누르고 승인을 누를 때 까지 대기
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (PosSystem.PaidAmount != 1000)
                {
                    PosSystem.currentState = POSSystem.EProceedState.Paying;
                    SetIndexTo(1001);
                    yield return StartCoroutine(StartNextDialog(1));
                    yield return new WaitForSeconds(4.0f);
                    SetIndexTo(19);
                    yield return StartCoroutine(StartNextDialog(1));
                }
                else
                {
                    break;
                }
            }

            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(4));
        yield return StartCoroutine(GoToSpot(1));
        
        Destroy(pickInstance);
        Finished = true;
    }
}