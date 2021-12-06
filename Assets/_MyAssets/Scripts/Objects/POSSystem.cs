using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class POSSystem : MonoBehaviour
{
    public EProceedState currentState = EProceedState.None;
    private List<Goods> _goodsList = new List<Goods>();
    private List<int> _goodsCount = new List<int>();
    private Stack<Goods> _scanStack = new Stack<Goods>();
    private Stack<string> _inputStack = new Stack<string>();
    private AudioSource _audioSource;

    [SerializeField] private Animator cashBoxAnimator;

    public List<Goods> GoodsList => _goodsList;
    public List<int> GoodsCount => _goodsCount;

    private int _totalPrice = 0;
    private int _paidAmount = 0;
    private string _paidAmountString = "";
    private int _changeAmount = 0;

    public bool IsEmpty
    {
        get
        {
            if (_goodsList.Count == 0)
            {
                return true;
            }

            return false;
        }
    }

    public int TotalPrice => _totalPrice;

    public int PaidAmount => _paidAmount;

    [HideInInspector] public bool forceScanningMode = false; //튜토리얼에서 정해진 구간을 벗어나지 않게 하기 위해 일시적으로 스캐닝으로 고정

    [SerializeField] private GameObject[] posRows; //포스기 상품정보에서 한 줄에 출력되는 텍스트들의 부모 오브젝트
    [SerializeField] private Text totalText; //합계 금액이 출력되는 텍스트
    [SerializeField] private Text paidText; //낸 금액이 출력되는 텍스트
    [SerializeField] private Text changeText; //거스름돈이 출력되는 텍스트


    public enum EPosPopUp
    {
        Cash,
        CreditCard,
        Refund
    }

    [SerializeField] private GameObject popUp;
    [SerializeField] private Text popUpTitle;
    [SerializeField] private Text popUpDescription;


    //싱글톤 처리
    private static POSSystem _instance;

    public static POSSystem Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }

    static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.FindWithTag("POSSystem");
            if (go == null)
            {
                Debug.LogError("POSSystem not found");
            }

            _instance = go.GetComponent<POSSystem>();
        }
    }

    public enum EProceedState //계산 단계
    {
        None, //아무것도 진행하고 있지 않음
        Scanning, //바코드 스캔 단계
        Paying, //결제 처리 단계
        Finishing, //결제 완료
        Refund //환불 (영수증 바코드 스캔)
    }

    // Start is called before the first frame update
    void Awake()
    {
        Init();
        _audioSource = GetComponent<AudioSource>();
        popUpTitle = popUp.transform.GetChild(1).GetComponent<Text>();
        popUpDescription = popUp.transform.GetChild(2).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddGoods(Goods goods)
    {
        _scanStack.Push(goods);
        _totalPrice += goods.unitPrice;
        _audioSource.Play();
        for (int i = 0; i < _goodsList.Count; i++)
        {
            //이미 존재하는 상품인지 확인
            if (goods.goodsName == _goodsList[i].goodsName)
            {
                _goodsCount[i]++;
                Refresh();
                return;
            }
        }

        _goodsList.Add(goods);
        _goodsCount.Add(1);
        Refresh();
    }

    private void UndoScanningAction()
    {
        if (_scanStack.Count == 0) return;
        string goodsName = _scanStack.Pop().goodsName;
        for (int i = 0; i < _goodsList.Count; i++)
        {
            if (goodsName == _goodsList[i].goodsName)
            {
                _totalPrice -= _goodsList[i].unitPrice;
                _goodsCount[i]--;
                if (_goodsCount[i] == 0)
                {
                    _goodsList.RemoveAt(i);
                    _goodsCount.RemoveAt(i);
                }

                Refresh();
                return;
            }
        }
    }

    private void UndoInputAction()
    {
        if (_scanStack.Count == 0) return;
        string key = _inputStack.Pop();

        if (key == "00")
        {
            _paidAmountString = _paidAmountString.Substring(0, _paidAmountString.Length - 2);
        }
        else
        {
            _paidAmountString = _paidAmountString.Substring(0, _paidAmountString.Length - 1);
        }
    }

    public void ResetGoodsAndRefresh()
    {
        ResetGoods();
        Refresh();
    }

    private void ResetGoods()
    {
        _scanStack.Clear();
        _inputStack.Clear();
        _goodsList.Clear();
        _goodsCount.Clear();
        _totalPrice = 0;
        _paidAmountString = "";
        _paidAmount = 0;
        _changeAmount = 0;
    }

    private void Refresh()
    {
        for (int i = 0; i < _goodsList.Count; i++)
        {
            posRows[i].transform.GetChild(0).GetComponent<Text>().text = _goodsList[i].goodsName;
            posRows[i].transform.GetChild(1).GetComponent<Text>().text = $"{_goodsList[i].unitPrice:n0}";
            posRows[i].transform.GetChild(2).GetComponent<Text>().text = $"{_goodsCount[i]:n0}";
            posRows[i].transform.GetChild(3).GetComponent<Text>().text = $"{_goodsList[i].unitPrice * _goodsCount[i]:n0}";
        }

        for (int i = _goodsList.Count; i < posRows.Length; i++)
        {
            posRows[i].transform.GetChild(0).GetComponent<Text>().text = "";
            posRows[i].transform.GetChild(1).GetComponent<Text>().text = "";
            posRows[i].transform.GetChild(2).GetComponent<Text>().text = "";
            posRows[i].transform.GetChild(3).GetComponent<Text>().text = "";
        }

        totalText.text = "₩" + $"{_totalPrice:n0}";
        paidText.text = "₩" + $"{_paidAmount:n0}";
        changeText.text = "₩" + $"{_changeAmount:n0}";
    }

    public void InputPosButton(string key)
    {
        switch (key)
        {
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
            case "0":
            case "00":
                if (_paidAmountString.Length >= 8) break;
                if (currentState != EProceedState.Paying) break;
                _paidAmountString += key;
                _inputStack.Push(key);
                break;
            case "undo":
                if (currentState == EProceedState.Scanning)
                {
                    UndoScanningAction();
                }
                else if (currentState == EProceedState.Paying)
                {
                    UndoInputAction();
                }

                break;
            case "reset": //리셋
                if (currentState == EProceedState.Scanning)
                {
                    ResetGoods();
                }
                else if (currentState == EProceedState.Paying)
                {
                    _paidAmountString = "";
                    _paidAmount = 0;
                }

                break;
            case "accept": //승인 (결제)
                if (currentState == EProceedState.Paying)
                {
                    currentState = EProceedState.Finishing;
                    _changeAmount = _paidAmount - _totalPrice;
                }

                break;
            case "apply": //확인 (바코드 스캔 완료 알림)
                if (currentState == EProceedState.Scanning && !IsEmpty && !forceScanningMode)
                {
                    currentState = EProceedState.Paying;
                }
                else if (currentState == EProceedState.Refund && !IsEmpty)
                {
                    currentState = EProceedState.None;
                }

                break;
            default:
                Debug.Assert(false);
                break;
        }

        if (_paidAmountString != "")
        {
            _paidAmount = int.Parse(_paidAmountString);
        }
        else
        {
            _paidAmount = 0;
        }

        Refresh();
    }

    public void OpenCashBox()
    {
        cashBoxAnimator.SetTrigger("open");
    }

    public void CloseCashBox()
    {
        cashBoxAnimator.SetTrigger("close");
    }

    public void SetState(EProceedState s)
    {
        currentState = s;
    }

    public void ClearChangeText()
    {
        _changeAmount = 0;
        Refresh();
    }

    public void OpenPopUpWindow(EPosPopUp p)
    {
        switch (p)
        {
            case EPosPopUp.Cash:
                SetPopUpMessage("현금결제", "현금을 수금하고 고객이 지불한 금액을 입력한 뒤 승인 버튼을 누르십시오");
                popUp.SetActive(true);
                break;
            case EPosPopUp.CreditCard:
                SetPopUpMessage("카드결제", "계속하려면 승인 버튼을 누르십시오");
                popUp.SetActive(true);
                break;
            case EPosPopUp.Refund:
                SetPopUpMessage("거래 취소", "영수증 하단 바코드를 스캔하십시오");
                popUp.SetActive(true);
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    public void ClosePopUpWindow()
    {
        popUp.SetActive(false);
    }

    public void SetPopUpMessage(string title, string description)
    {
        popUpTitle.text = title;
        popUpDescription.text = description;
    }

    public void SetCreditCardPaymentAndRefresh()
    {
        _paidAmount = _totalPrice;
        _changeAmount = 0;
        Refresh();
    }
    
    public IEnumerator ProceedCreditCardPayment()
    {
        SetCreditCardPaymentAndRefresh();
        
        SetPopUpMessage("카드결제", "IC 카드 정보 읽는 중");
        yield return new WaitForSeconds(1.0f);

        SetPopUpMessage("카드결제", "연결 중");
        yield return new WaitForSeconds(0.8f);

        SetPopUpMessage("카드결제", "데이터 처리 중");
        yield return new WaitForSeconds(1.5f);

        SetPopUpMessage("카드결제", "응답전문 수신완료");
        yield return new WaitForSeconds(0.3f);

        SetPopUpMessage("카드결제", "ACK 전송완료");
        yield return new WaitForSeconds(0.5f);

        SetPopUpMessage("거래완료", "IC카드를 제거해주십시오");
        yield return new WaitForSeconds(2.0f);

        ClosePopUpWindow();
    }
}