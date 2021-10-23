using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarcodeScanner : MonoBehaviour
{
    [SerializeField] private Transform laserPoint;
    [SerializeField] private POSSystem posSystem;
    [SerializeField] private Light scanLight;
    private bool _isGrabbed = false;
    private InputManager.Controller _grabbedHand = InputManager.Controller.RTouch;
    private GameObject prevScannedObject = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Get(InputManager.Button.IndexTrigger, InputManager.Controller.RTouch))
        {
            Debug.DrawRay(laserPoint.position, laserPoint.forward * 100.0f, Color.red, 1.0f);

            scanLight.enabled = true;
            RaycastHit hit;
            if (Physics.Raycast(laserPoint.position, laserPoint.forward, out hit, 3, LayerMask.GetMask("Barcode")))
            {
                Debug.Log("Hello");
                Goods goodsInfo = hit.collider.GetComponent<Goods>();
                if (prevScannedObject == hit.collider.gameObject)
                {
                    return;
                }

                posSystem.AddGoods(goodsInfo);
                Debug.Log(goodsInfo.goodsName);
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