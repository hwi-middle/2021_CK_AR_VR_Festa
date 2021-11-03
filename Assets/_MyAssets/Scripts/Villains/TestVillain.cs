using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestVillain : MonoBehaviour
{
    [SerializeField] private TextAsset script; //대사 파일
    [SerializeField] private Text dialogText; //대사를 출력할 텍스트
    private readonly DialogCSVReader _csvReader = new DialogCSVReader(); //CSV 파서
    private int _index = 1; //대사의 고유번호를 저장

    private readonly Color _playerColor = new Color(1f, 1f, 1f);
    private readonly Color _villainColor = new Color(1f, 0f, 0f);

    private bool _isPlaying = false; //대사를 출력 중인지


    // Start is called before the first frame update
    void Start()
    {
        _csvReader.Load(script);
        StartCoroutine(Act());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private enum ESpeed
    {
        VerySlow = 1,
        Slow,
        Normal,
        Fast,
        VeryFast
    }

    private IEnumerator Act()
    {
        yield return StartCoroutine(StartNextDialog());
        StartCoroutine(StartNextDialog());
    }

    private IEnumerator StartNextDialog()
    {
        DialogCSVReader.Row line = _csvReader.Find_id(_index.ToString());
        float.TryParse(line.time, out var time);
        StartCoroutine(ShowNextDialog(line));
        yield return new WaitForSeconds(time);
        StopCoroutine(ShowNextDialog(line));
    }
    
    private IEnumerator ShowNextDialog(DialogCSVReader.Row line)
    {
        _isPlaying = true;
        
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
        }

        _index++;
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
}