using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TutorialDialog : Villain
{
    public GameObject snack;
    public BarcodeScanner scanner;
    public POSSystem posSystem;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Act());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator Act()
    {
        yield return StartCoroutine(StartNextDialog(3));
        _continue = false;
        while (!_continue) //튜토리얼 스킵여부 버튼 입력 대기
        {
            yield return null;
        }

        Instantiate(snack);

        yield return StartCoroutine(StartNextDialog(5));
        while (!scanner.IsGrabbed) //스캐너를 집을 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(3));
        while (!posSystem.IsEmpty) //물건을 스캔할 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(2));
        while (!posSystem.IsEmpty) //확인 버튼 누를 때 까지 대기
        {
            yield return null;
        }

        yield return StartCoroutine(StartNextDialog(1));
        while (!posSystem.IsEmpty) //천원권을 집을 때 까지 대기
        {
            yield return null;
        }
    }


    public void SkipTutorial()
    {
        //튜토리얼 씬 분리, 이후 게임 씬으로 이동하는 코드 필요
        throw new NotImplementedException();
        //SceneManager.LoadScene("");
    }
}