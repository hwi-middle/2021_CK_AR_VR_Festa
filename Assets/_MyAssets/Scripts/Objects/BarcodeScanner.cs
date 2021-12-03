using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarcodeScanner : MonoBehaviour
{
    [SerializeField] private Transform laserPoint;
    [SerializeField] private POSSystem posSystem;
    [SerializeField] private Light scanLight;
    private OVRGrabbable _ovrGrabbable;
    private GameObject _prevScannedObject = null;

    // Start is called before the first frame update
    void Start()
    {
        _ovrGrabbable = GetComponent<OVRGrabbable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_ovrGrabbable.isGrabbed) return;
        InputManager.Controller currentController = (InputManager.Controller) _ovrGrabbable.grabbedBy.Controller;

        if (InputManager.Get(InputManager.Button.IndexTrigger, currentController))
        {
            Debug.DrawRay(laserPoint.position, laserPoint.forward * 100.0f, Color.red, 1.0f);

            scanLight.enabled = true;
            RaycastHit hit;
            if (Physics.Raycast(laserPoint.position, laserPoint.forward, out hit, scanLight.range))
            {
                GameObject hitGameObject = hit.collider.gameObject;
                if (hitGameObject == _prevScannedObject) return;

                if (hitGameObject.layer != LayerMask.NameToLayer("Barcode"))
                {
                    _prevScannedObject = null;
                    return;
                }

                if (hitGameObject.CompareTag("Receipt"))
                {
                    Receipt receiptInfo = hitGameObject.GetComponent<Receipt>();
                    if (receiptInfo.isScanned) return;
                    for (int i = 0; i < hitGameObject.transform.childCount; i++)
                    {
                        for (int j = 0; j < receiptInfo.goodsCount[i]; j++)
                        {
                            posSystem.AddGoods(hitGameObject.transform.GetChild(i).GetComponent<Goods>());
                        }
                    }

                    receiptInfo.isScanned = true;
                    _prevScannedObject = hitGameObject;
                }
                else
                {
                    Goods goodsInfo = hitGameObject.GetComponent<Goods>();
                    if (posSystem.currentState == POSSystem.EProceedState.Scanning)
                    {
                        posSystem.AddGoods(goodsInfo);
                    }

                    Debug.Log("Scanned Goods : " + goodsInfo.goodsName);
                    _prevScannedObject = hitGameObject;
                }
            }
            else
            {
                _prevScannedObject = null;
            }
        }
        else
        {
            scanLight.enabled = false;
            _prevScannedObject = null;
        }
    }
}