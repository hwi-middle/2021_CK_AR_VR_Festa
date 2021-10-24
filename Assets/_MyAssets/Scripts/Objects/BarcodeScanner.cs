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
    private GameObject prevScannedObject = null;

    // Start is called before the first frame update
    void Start()
    {
        _ovrGrabbable = GetComponent<OVRGrabbable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_ovrGrabbable.grabbedBy == null) return;
        InputManager.Controller currentController = (InputManager.Controller) _ovrGrabbable.grabbedBy.Controller;

        if (InputManager.Get(InputManager.Button.IndexTrigger, currentController))
        {            
            Debug.DrawRay(laserPoint.position, laserPoint.forward * 100.0f, Color.red, 1.0f);

            scanLight.enabled = true;
            RaycastHit hit;
            if (Physics.Raycast(laserPoint.position, laserPoint.forward, out hit, scanLight.range))
            {            
                if (prevScannedObject == hit.collider.gameObject) return;
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Barcode"))
                {
                    prevScannedObject = hit.collider.gameObject;
                    return;
                }

                Goods goodsInfo = hit.collider.GetComponent<Goods>();
                posSystem.AddGoods(goodsInfo);
                Debug.Log("Scanned Goods : " + goodsInfo.goodsName);
                prevScannedObject = hit.collider.gameObject;
            }
            else
            {
                prevScannedObject = null;
            }
        }
        else
        {
            scanLight.enabled = false;
            prevScannedObject = null;
        }
    }
}