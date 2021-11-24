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
    private AudioSource _audioSource;

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

    public bool forceScanningMode = false; //튜토리얼에서 정해진 구간을 벗어나지 않게 하기 위해 일시적으로 스캐닝으로 고정

    [SerializeField] private GameObject[] posRows; //포스기 상품정보에서 한 줄에 출력되는 텍스트들의 부모 오브젝트
    [SerializeField] private Text totalText; //합계 금액이 출력되는 텍스트
    [SerializeField] private Text paidText; //낸 금액이 출력되는 텍스트
    [SerializeField] private Text changeText; //거스름돈이 출력되는 텍스트

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
        Finishing //결제 완료
    }

    // Start is called before the first frame update
    void Awake()
    {
        Init();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddGoods(Goods goods)
    {
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

    public void RectifyGoods(string goodsName)
    {
        for (int i = 0; i < _goodsList.Count; i++)
        {
            //존재하는 상품인지 확인
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

    public void ResetGoodsAndRefresh()
    {
        ResetGoods();
        Refresh();
    }
    
    private void ResetGoods()
    {
        _goodsList.Clear();
        _goodsCount.Clear();
        for (int i = 0; i < posRows.Length; i++)
        {
            posRows[i].transform.GetChild(0).GetComponent<Text>().text = "";
            posRows[i].transform.GetChild(1).GetComponent<Text>().text = "";
            posRows[i].transform.GetChild(2).GetComponent<Text>().text = "";
            posRows[i].transform.GetChild(3).GetComponent<Text>().text = "";
        }

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
                break;
            case "backspace":
                if (_paidAmountString == "") break;
                _paidAmountString = _paidAmountString.Substring(0, _paidAmountString.Length - 1);
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
                    changeText.text = _changeAmount.ToString();
                }

                break;
            case "apply": //확인 (바코드 스캔 완료 알림)
                if (currentState == EProceedState.Scanning && !IsEmpty && !forceScanningMode)
                {
                    currentState = EProceedState.Paying;
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

    public void SetState(EProceedState s)
    {
        currentState = s;
    }
}