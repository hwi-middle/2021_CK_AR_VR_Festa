using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POSSystem : MonoBehaviour
{
    private int _totalPrice = 0;
    private List<Goods> _goodsList = new List<Goods>();
    private List<int> _goodsCount = new List<int>();
    private AudioSource _audioSource;

    [SerializeField] private GameObject[] posRows; //포스기 상품정보에서 한 줄에 출력되는 텍스트들의 부모 오브젝트
    [SerializeField] private Text totalText; //합계 금액이 출력되는 텍스트

    // Start is called before the first frame update
    void Start()
    {        
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

    public void ResetGoods()
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
    }

    private void Refresh()
    {
        for (int i = 0; i < _goodsList.Count; i++)
        {
            posRows[i].transform.GetChild(0).GetComponent<Text>().text = _goodsList[i].goodsName;
            posRows[i].transform.GetChild(1).GetComponent<Text>().text = $"{_goodsList[i].unitPrice:n0}";
            posRows[i].transform.GetChild(2).GetComponent<Text>().text = $"{_goodsCount[i]:n0}";
            posRows[i].transform.GetChild(3).GetComponent<Text>().text =
                $"{_goodsList[i].unitPrice * _goodsCount[i]:n0}";
        }

        totalText.text = "₩" + $"{_totalPrice:n0}";
    }
}