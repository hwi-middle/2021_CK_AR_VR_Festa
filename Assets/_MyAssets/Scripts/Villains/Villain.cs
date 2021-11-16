using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Villain : MonoBehaviour
{
    [SerializeField] private TextAsset script; //대사 파일
    [SerializeField] private Text dialogText; //대사를 출력할 텍스트
    private readonly DialogCSVReader _csvReader = new DialogCSVReader(); //CSV 파서
    private int _index = 1; //대사의 고유번호를 저장

    private readonly Color _playerColor = new Color(1f, 1f, 1f);
    private readonly Color _villainColor = new Color(1f, 0f, 0f);

    //private bool _isPlaying = false; //대사를 출력 중인지
    protected bool _continue = true; //게임 진행을 계속 진행할지 지정, 각 자식 스크립트에서 사용

    private GameObject _reply1Canvas;
    private GameObject _reply2Canvas;
    private Button _reply1Button;
    private Button _reply2Button;
    private Text _reply1Text;
    private Text _reply2Text;

    private DialogCSVReader.Row _currentLine = null;

    private enum ESpeed
    {
        VerySlow = 1,
        Slow,
        Normal,
        Fast,
        VeryFast
    }

    protected virtual void Awake()
    {
        _csvReader.Load(script);
        _reply1Canvas = GameObject.FindWithTag("Reply1");
        _reply2Canvas = GameObject.FindWithTag("Reply2");
        _reply1Button = _reply1Canvas.transform.GetChild(0).GetComponent<Button>();
        _reply2Button = _reply2Canvas.transform.GetChild(0).GetComponent<Button>();
        _reply1Text = _reply1Canvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        _reply2Text = _reply2Canvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        _reply1Button.onClick.AddListener(delegate { Reply1Btn(); });
        _reply2Button.onClick.AddListener(delegate { Reply2Btn(); });
        _reply1Canvas.SetActive(false);
        _reply2Canvas.SetActive(false);
    }

    protected IEnumerator StartNextDialog(int iteration = 1)
    {
        Debug.Assert(iteration > 0);
        for (int i = 0; i < iteration; i++)
        {
            _currentLine = _csvReader.Find_id(_index.ToString());
            float.TryParse(_currentLine.time, out var time);
            yield return StartCoroutine(ShowNextDialog(_currentLine));
            yield return new WaitForSeconds(time);
        }
    }

    private IEnumerator ShowNextDialog(DialogCSVReader.Row line)
    {
        //_isPlaying = true;

        string speaker = line.speaker.ToLower().Trim();
        string dialog = line.dialog;
        ESpeed speed = (ESpeed) int.Parse(line.speed);
        float delay = GetSpeedValue(speed);

        string reply1 = line.reply1.Trim();
        string reply2 = line.reply2.Trim();

        //화자에 따라 텍스트 컬러 변경
        if (speaker == "p")
        {
            dialogText.color = _playerColor;
        }
        else if (speaker == "v")
        {
            dialogText.color = _villainColor;
        }
        else
        {
            Debug.LogError($"InvalidSpeaker: {speaker}");
        }

        //대사 출력
        dialogText.text = "";
        yield return new WaitForSeconds(delay);
        foreach (var t in dialog)
        {
            dialogText.text += t;
            yield return new WaitForSeconds(delay);
        }

        //플레이어의 답변 버튼 출력
        if (reply1 != "")
        {
            //버튼 출력
            _reply1Canvas.SetActive(true);
            _reply2Canvas.SetActive(true);

            _reply1Text.text = reply1;
            _reply2Text.text = reply2;
        }

        _index++;
        // isplaying = false;
    }

    private float GetSpeedValue(ESpeed s)
    {
        switch (s)
        {
            case ESpeed.VerySlow:
                return 0.5f;
            case ESpeed.Slow:
                return 0.3f;
            case ESpeed.Normal:
                return 0.1f;
            case ESpeed.Fast:
                return 0.05f;
            case ESpeed.VeryFast:
                return 0.01f;
            default:
                Debug.Assert(false);
                return -1.0f;
        }
    }

    public void Reply1Btn()
    {
        SetIndexTo(int.Parse(_currentLine.goto1));
        DeactivateButtons();
        _continue = true;
    }

    public void Reply2Btn()
    {
        SetIndexTo(int.Parse(_currentLine.goto2));
        DeactivateButtons();
        _continue = true;
    }

    private void DeactivateButtons()
    {
        _reply1Canvas.SetActive(false);
        _reply2Canvas.SetActive(false);
    }

    protected void SetIndexTo(int idx)
    {
        if (idx < 0) return;
        _index = idx;
    }

    //버튼 등에서 사용하기 위한 함수
    public void SetContinue(bool b)
    {
        _continue = b;
    }

    private void OnDisable()
    {
        _reply1Button.onClick.RemoveListener(Reply1Btn);
        _reply2Button.onClick.RemoveListener(Reply2Btn);
    }
}