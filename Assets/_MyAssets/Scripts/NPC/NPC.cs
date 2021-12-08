using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField] private TextAsset script; //대사 파일
    [SerializeField] private bool textureSwapImplemented = false; //피격 시 텍스처 스왑 구현여부 저장 (임시)
    [SerializeField] private SkinnedMeshRenderer renderObject;
    [SerializeField] private Texture originalTexture;
    [SerializeField] private Texture hitTexture;
    [SerializeField] private Animator animator;
    private int _lastCollidedInstanceId = -1;

    private readonly DialogCSVReader _csvReader = new DialogCSVReader(); //CSV 파서
    private int _index = 1; //대사의 고유번호를 저장
    private DialogCSVReader.Row _currentLine = null; //이번 index의 대사 정보

    private static readonly Color PlayerColor = new Color(0f, 1f, 0.7867756f);
    private static readonly Color VillainColor = new Color(1f, 1f, 1f);

    protected bool Finished = false; //공략이 완료되었는지
    protected bool Continue = true; //게임 진행을 계속 진행할지 지정, 각 자식 스크립트에서 사용

    public static bool LoadedStaticObjects
    {
        get => _loadedStaticObjects;
        set => _loadedStaticObjects = value;
    }

    private static bool _loadedStaticObjects = false;
    protected static GameManager Manager;
    private static Text _dialogText; //대사를 출력할 텍스트
    private static AudioSource _dialogAudioSource;
    private static GameObject _reply1Canvas;
    private static GameObject _reply2Canvas;
    protected static Button Reply1Button;
    protected static Button Reply2Button;
    private static Text _reply1Text;
    private static Text _reply2Text;
    protected static POSSystem PosSystem;
    protected static AutomaticDoor Door;

    [SerializeField] protected GameObject pick; //손님이 구매할 물건 프리팹
    [SerializeField] protected GameObject pay; //손님이 지불할 현금 프리팹
    protected Dictionary<string, int> CorrectPicks = new Dictionary<string, int>(); //손님이 구매할 물건들, 제대로 스캔했는지 비교하기 위해 사용됨

    private static GameObject _spotsParent; //손님이 이동할 스팟들의 부모 오브젝트
    private static readonly Transform[] Spots = new Transform[12]; //스팟 번호를 그대로 사용하기 위해 0번 index는 비워둠.
    private NavMeshAgent _navMeshAgent;
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
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
        Debug.Log("ACTIVATED");
        if (!_loadedStaticObjects)
        {
            Door = GameObject.FindWithTag("MainGate").GetComponent<AutomaticDoor>();
            _dialogText = GameObject.FindWithTag("DialogText").GetComponent<Text>();
            _dialogAudioSource = _dialogText.transform.parent.GetComponent<AudioSource>();
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
                Spots[i] = _spotsParent.transform.GetChild(i);
            }

            Manager = GameManager.Instance;
            PosSystem = POSSystem.Instance;
            _loadedStaticObjects = true;
        }

        _csvReader.Load(script);
        _navMeshAgent = GetComponent<NavMeshAgent>();
        AddCorrectPicks(pick);
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
            _dialogAudioSource.Play();
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

    protected IEnumerator GoToSpot(int idx)
    {
        _navMeshAgent.SetDestination(Spots[idx - 1].position);
        if (animator != null)
            animator.SetBool("Walk", true);
        while (true)
        {
            if (IsNavMeshAgentReachedDestination())
            {
                break;
            }

            yield return null;
        }

        if (animator != null)
            animator.SetBool("Walk", false);
        yield return StartCoroutine(Turn(GetDirectionBySpotIndex(idx)));
    }

    public bool IsNavMeshAgentReachedDestination()
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
            case 6:
            case 9:
                return ERotateDirection.Right;

            case 5:
            case 7:
            case 8:
            case 10:
            case 11:
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

    protected void ResetCorrectPicks()
    {
        CorrectPicks.Clear();
    }

    protected void AddCorrectPicks(GameObject p)
    {
        if (p == null) return;

        for (int i = 0; i < p.transform.childCount; i++)
        {
            var goodsInfo = p.transform.GetChild(i).GetChild(0).GetComponent<Goods>();
            if (CorrectPicks.ContainsKey(goodsInfo.goodsName))
            {
                CorrectPicks[goodsInfo.goodsName]++;
            }
            else
            {
                CorrectPicks.Add(goodsInfo.goodsName, 1);
            }
        }
    }

    protected bool CheckScannedCorrectly()
    {
        //현재 스캔된 오브젝트들을 Dictionary로 묶음
        var currentScannedGoods = new Dictionary<string, int>();
        for (int i = 0; i < PosSystem.GoodsList.Count; i++)
        {
            currentScannedGoods.Add(PosSystem.GoodsList[i].goodsName, PosSystem.GoodsCount[i]);
        }

        //손님이 실제 고른 것과 현재 스캔한 것이 같은지 확인
        foreach (var key in CorrectPicks.Keys)
        {
            if (currentScannedGoods.TryGetValue(key, out var num))
            {
                if (CorrectPicks[key] != num)
                {
                    //수량이 다름
                    return false;
                }
            }
            else
            {
                //현재 스캔한 오브젝트들에 손님이 고른 물건이 없음
                return false;
            }
        }

        //물건의 종류와 수량이 모두 일치
        return true;
    }

    //올바르게 상품을 스캔한 상태에서 확인 버튼 누를 때 까지 대기
    protected IEnumerator WaitUntilScanCorrectlyAndApply()
    {
        while (true)
        {
            if (PosSystem.currentState == POSSystem.EProceedState.Paying)
            {
                if (CheckScannedCorrectly())
                {
                    yield break;
                }

                Manager.DecreaseLife();
                PosSystem.currentState = POSSystem.EProceedState.Scanning;
            }

            yield return null;
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Cash":
            case "Goods":
            case "Scanner":
            case "Receipt":

                if (animator != null)
                {
                    if (_lastCollidedInstanceId == other.gameObject.GetInstanceID()) return;
                    _lastCollidedInstanceId = other.gameObject.GetInstanceID();
                    animator.SetTrigger("Hit");
                    if (textureSwapImplemented)
                        StartCoroutine(SwapTexture());
                }

                break;
        }
    }

    protected virtual void OnCollisionExit(Collision other)
    {
        _lastCollidedInstanceId = -1;
    }

    private IEnumerator SwapTexture()
    {
        renderObject.material.SetTexture(BaseMap, hitTexture);

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.idle"))
        {
            yield return null;
        }

        while (animator.IsInTransition(0))
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.hit"))
        {
            yield return null;
        }

        renderObject.material.SetTexture(BaseMap, originalTexture);
    }
}