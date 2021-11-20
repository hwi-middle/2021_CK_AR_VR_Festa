using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Boss : NPC
{
    public GameObject snack;
    public BarcodeScanner scanner;
    private POSSystem _posSystem;
    public GameObject cash1000;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _posSystem = POSSystem.Instance;
        _posSystem.currentState = POSSystem.EProceedState.Scanning;
        _posSystem.forceScanningMode = true;
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
        _continue = false;
        while (!_continue) //튜토리얼 스킵여부 버튼 입력 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(3));

        Instantiate(snack); //스캔해볼 과자 오브젝트 생성

        yield return StartCoroutine(StartNextDialog(2));
        while (!scanner.GetComponent<OVRGrabbable>().isGrabbed) //스캐너를 집을 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(3));
        while (_posSystem.IsEmpty) //물건을 스캔할 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(3));
        _posSystem.forceScanningMode = false;
        while (_posSystem.currentState != POSSystem.EProceedState.Paying || _posSystem.TotalPrice != 800) //1개의 상품만 찍은 상태에서 확인 버튼 누를 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(1));
        var cash = Instantiate(cash1000); //집어들 천원권 오브젝트 생성
        while (!cash.GetComponent<OVRGrabbable>().isGrabbed) //천원권을 집을 때 까지 대기
        {
            yield return null;
        }
        
        yield return StartCoroutine(StartNextDialog(1));
        while (cash != null) //천원권이 CashBox에 닿아 Destroy될 때 까지 대기
        {
            yield return null;
        }
        
        yield return StartCoroutine(StartNextDialog(3));
        while (true) //올바른 금액을 누르고 승인을 누를 때 까지 대기
        {
            if (_posSystem.currentState == POSSystem.EProceedState.Finishing)
            {
                if (_posSystem.PaidAmount != 800)
                {
                    _posSystem.currentState = POSSystem.EProceedState.Paying;
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
    }
    
    public void SkipTutorial()
    {
        //튜토리얼 씬 분리, 이후 게임 씬으로 이동하는 코드 필요
        throw new NotImplementedException();
        //SceneManager.LoadScene("");
    }
}