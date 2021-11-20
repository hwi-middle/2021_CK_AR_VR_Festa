using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField] private TextAsset script; //대사 파일
    [SerializeField] private Text dialogText; //대사를 출력할 텍스트
    private readonly DialogCSVReader _csvReader = new DialogCSVReader(); //CSV 파서
    private int _index = 1; //대사의 고유번호를 저장

    private readonly Color _playerColor = new Color(1f, 1f, 1f);
    private readonly Color _villainColor = new Color(1f, 0f, 0f);

    //private bool _isPlaying = false; //대사를 출력 중인지
    protected bool _isFinished = false; //공략이 완료되었는지
    protected bool _continue = true; //게임 진행을 계속 진행할지 지정, 각 자식 스크립트에서 사용

    private GameObject _reply1Canvas;
    private GameObject _reply2Canvas;
    private Button _reply1Button;
    private Button _reply2Button;
    private Text _reply1Text;
    private Text _reply2Text;

    private DialogCSVReader.Row _currentLine = null;

    [SerializeField] private GameObject spotsParent;
    private Transform[] spots = new Transform[13]; //스팟 번호를 그대로 사용하기 위해 0번 index는 비워둠.
    private NavMeshAgent _navMeshAgent;
    public bool IsFinished => _isFinished; //공략이 완료되었는지

    //대사 출력 속도
    private enum ESpeed
    {
        VerySlow = 1,
        Slow,
        Normal,
        Fast,
        VeryFast
    }

    //탑뷰 기준 빌런의 회전 방향
    private enum RotateDirection
    {
        Up,
        Down,
        Left,
        Right,
        None //오류 검출용
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

        _navMeshAgent = GetComponent<NavMeshAgent>();
        for (int i = 0; i < 12; i++)
        {
            spots[i + 1] = spotsParent.transform.GetChild(i);
            Debug.Log(spots[i + 1].gameObject.name);
        }
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

    protected IEnumerator GoToSpot(int idx)
    {
        _navMeshAgent.SetDestination(spots[idx].position);
        while (true)
        {
            if (!_navMeshAgent.pathPending)
            {
                if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                {
                    if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return StartCoroutine(Turn(GetDirectionBySpotIndex(idx)));
    }

    private RotateDirection GetDirectionBySpotIndex(int idx)
    {
        switch (idx)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                return RotateDirection.Up;

            case 12:
                return RotateDirection.Down;
            case 5:
            case 8:
            case 11:
                return RotateDirection.Right;

            case 6:
            case 7:
            case 9:
            case 10:
                return RotateDirection.Left;

            default:
                Debug.Assert(false);
                return RotateDirection.None;
        }
    }

    private IEnumerator Turn(RotateDirection dir)
    {
        _navMeshAgent.enabled = false;
        Transform from = transform;
        float speed = 2f;

        float rotationAmount = 0f;
        switch (dir)
        {
            case RotateDirection.Up:
                rotationAmount = 0f;
                break;
            case RotateDirection.Right:
                rotationAmount = 90f;
                break;
            case RotateDirection.Down:
                rotationAmount = 180f;
                break;
            case RotateDirection.Left:
                rotationAmount = 270f;
                break;
            default:
                Debug.Assert(false);
                break;
        }

        float t = 0;
        while (t <= 1f)
        {
            transform.rotation = Quaternion.Slerp(from.rotation, Quaternion.Euler(0, rotationAmount, 0), t);
            t += Time.deltaTime * speed;
            yield return null;
        }
    }
}