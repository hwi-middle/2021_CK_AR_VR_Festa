using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Laser : MonoBehaviour
{
    private LineRenderer laser;
    private RaycastHit Collided_object;
    private GameObject currentObject;

    [SerializeField] private InputManager.Controller curController;
    [SerializeField] private float raycastDistance = 100f;

    // Start is called before the first frame update
    void Start()
    {
        laser = GetComponent<LineRenderer>();

        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = new Color(0, 195, 255, 0.5f);
        laser.material = material;
        laser.positionCount = 2;
        laser.startWidth = 0.008f;
        laser.endWidth = 0.008f;
    }

    // Update is called once per frame
    void Update()
    {
        laser.SetPosition(0, transform.position);
        //Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green, 0.5f);

        if (Physics.Raycast(transform.position, transform.forward, out Collided_object, raycastDistance))
        {
            laser.SetPosition(1, Collided_object.point);

            if (Collided_object.collider.gameObject.CompareTag("Button"))
            {
                if (InputManager.GetDown(InputManager.Button.IndexTrigger))
                {
                    Collided_object.collider.gameObject.GetComponent<Button>().onClick.Invoke();
                }

                else
                {
                    Collided_object.collider.gameObject.GetComponent<Button>().OnPointerEnter(null);
                    currentObject = Collided_object.collider.gameObject;
                }
            }
        }

        else
        {
            laser.SetPosition(1, transform.position + (transform.forward * raycastDistance));

            if (currentObject != null)
            {
                currentObject.GetComponent<Button>().OnPointerExit(null);
                currentObject = null;
            }
        }
    }

    private void LateUpdate()
    {
        if (InputManager.GetDown(InputManager.Button.IndexTrigger, curController))
        {
            laser.material.color = new Color(255, 255, 255, 0.5f);
        }

        else if (InputManager.GetUp(InputManager.Button.IndexTrigger, curController))
        {
            laser.material.color = new Color(0, 195, 255, 0.5f);
        }
    }
}
