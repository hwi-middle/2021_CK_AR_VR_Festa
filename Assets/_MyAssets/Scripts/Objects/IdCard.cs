using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdCard : MonoBehaviour
{
    public class IdInfo
    {
        public string KoreanName;
        public string HanjaName;
        public string ID;
        public string Address;
        public string Date;
        public string Institution;
        public Sprite Pic;
    }

    private IdInfo _info = null;

    public IdInfo Info
    {
        set
        {
            if (value == null) return;
            _info = value;
        }
    }
    
    [SerializeField] private Text nameText;
    [SerializeField] private Text idText;
    [SerializeField] private Text addressText;
    [SerializeField] private Text dateText;
    [SerializeField] private Text institutionText;
    [SerializeField] private Image picImage;

    public Button nameButton;
    public Button idButton;
    public Button dateButton;
    public Button picButton;
    public Button passButton;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        if (_info == null) return;

        nameText.text = $"{_info.KoreanName}({_info.HanjaName})";
        idText.text = _info.ID;
        addressText.text = _info.Address;
        dateText.text = _info.Date;
        institutionText.text = _info.Institution;
        picImage.sprite = _info.Pic;
    }
}