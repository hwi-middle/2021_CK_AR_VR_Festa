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
    private readonly DialogCSVReader _csvReader = new DialogCSVReader(); //CSV 파서
    private int _index = 1; //대사의 고유번호를 저장

    private static readonly Color PlayerColor = new Color(1f, 1f, 1f);
    private static readonly Color VillainColor = new Color(1f, 0f, 0f);

    protected bool Finished = false; //공략이 완료되었는지
    protected bool Continue = true; //게임 진행을 계속 진행할지 지정, 각 자식 스크립트에서 사용

    private static bool _loadedStaticObjects = false;
    private static Text _dialogText; //대사를 출력할 텍스트
    private static GameObject _reply1Canvas;
    private static GameObject _reply2Canvas;
    protected static Button Reply1Button;
    protected static Button Reply2Button;
    private static Text _reply1Text;
    private static Text _reply2Text;
    protected static POSSystem PosSystem;
    protected static AutomaticDoor Door;

    private DialogCSVReader.Row _currentLine = null;

    private static GameObject _spotsParent;
    private static Transform[] spots = new Transform[13]; //스팟 번호를 그대로 사용하기 위해 0번 index는 비워둠.
    private NavMeshAgent _navMeshAgent;
    public bool IsFinished => Finished; //공략이 완료되었는지

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
    protected enum ERotateDirection
    {
        Up,
        Down,
        Left,
        Right,
        None //오류 검출용
    }

    protected virtual void Awake()
    {
        if (!_loadedStaticObjects)
        {
            Door = GameObject.FindWithTag("MainGate").GetComponent<AutomaticDoor>();
            _dialogText = GameObject.FindWithTag("DialogText").GetComponent<Text>();
            _reply1Canvas = GameObject.FindWithTag("Reply1");
            _reply2Canvas = GameObject.FindWithTag("Reply2");
            Reply1Button = _reply1Canvas.transform.GetChild(0).GetComponent<Button>();
            Reply2Button = _reply2Canvas.transform.GetChild(0).GetComponent<Button>();
            _reply1Text = _reply1Canvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            _reply2Text = _reply2Canvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            _reply1Canvas.SetActive(false);
            _reply2Canvas.SetActive(false);
            _spotsParent = GameObject.FindWithTag("Spots");
            for (int i = 0; i < 12; i++)
            {
                spots[i + 1] = _spotsParent.transform.GetChild(i);
            }

            PosSystem = POSSystem.Instance;
            _loadedStaticObjects = true;
        }

        _csvReader.Load(script);
        _navMeshAgent = GetComponent<NavMeshAgent>();
        PosSystem.currentState = POSSystem.EProceedState.Scanning;
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
        string speaker = line.speaker.ToLower().Trim();
        string dialog = line.dialog;
        ESpeed speed = (ESpeed) int.Parse(line.speed);
        float delay = GetSpeedValue(speed);

        string reply1 = line.reply1.Trim();
        string reply2 = line.reply2.Trim();

        //화자에 따라 텍스트 컬러 변경
        if (speaker == "p")
        {
            _dialogText.color = PlayerColor;
        }
        else if (speaker == "v")
        {
            _dialogText.color = VillainColor;
        }
        else
        {
            Debug.LogError($"InvalidSpeaker: {speaker}");
        }

        //대사 출력
        _dialogText.text = "";
        yield return new WaitForSeconds(delay);
        foreach (var t in dialog)
        {
            _dialogText.text += t;
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

    protected void DefaultReply1Btn()
    {
        SetIndexTo(int.Parse(_currentLine.goto1));
        DeactivateButtons();
        Continue = true;
    }

    protected void DefaultReply2Btn()
    {
        SetIndexTo(int.Parse(_currentLine.goto2));
        DeactivateButtons();
        Continue = true;
    }

    protected void DeactivateButtons()
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
        Continue = b;
    }

    // private void OnEnable()
    // {
    //     Reply1Button.onClick.AddListener(delegate { Reply1Btn(); });
    //     Reply2Button.onClick.AddListener(delegate { Reply2Btn(); });
    // }
    //
    // private void OnDisable()
    // {
    //     Reply1Button.onClick.RemoveListener(Reply1Btn);
    //     Reply2Button.onClick.RemoveListener(Reply2Btn);
    // }

    protected IEnumerator GoToSpot(int idx)
    {
        _navMeshAgent.SetDestination(spots[idx].position);
        while (true)
        {
            if (IsNavMeshAgentReachedDestination())
            {
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(Turn(GetDirectionBySpotIndex(idx)));
    }

    protected bool IsNavMeshAgentReachedDestination()
    {
        if (!_navMeshAgent.pathPending)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private ERotateDirection GetDirectionBySpotIndex(int idx)
    {
        switch (idx)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                return ERotateDirection.Up;

            case 12:
                return ERotateDirection.Down;
            case 5:
            case 8:
            case 11:
                return ERotateDirection.Right;

            case 6:
            case 7:
            case 9:
            case 10:
                return ERotateDirection.Left;

            default:
                Debug.Assert(false);
                return ERotateDirection.None;
        }
    }

    private IEnumerator Turn(ERotateDirection dir)
    {
        Transform from = transform;
        float speed = 2f;

        float rotationAmount = 0f;
        switch (dir)
        {
            case ERotateDirection.Up:
                rotationAmount = 0f;
                break;
            case ERotateDirection.Right:
                rotationAmount = 90f;
                break;
            case ERotateDirection.Down:
                rotationAmount = 180f;
                break;
            case ERotateDirection.Left:
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