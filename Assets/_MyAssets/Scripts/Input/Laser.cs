using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Laser : MonoBehaviour
{
    private LineRenderer _laser;
    private RaycastHit _collidedObject;
    private GameObject _currentObject;

    [SerializeField] private InputManager.Controller curController;
    [SerializeField] private float raycastDistance = 100f;

    private readonly Color _defaultColor = new Color(0, 195, 255, 0.5f);
    private readonly Color _activatedColor = new Color(255, 255, 255, 0.5f);
    
    // Start is called before the first frame update
    private void Start()
    {
        _laser = GetComponent<LineRenderer>();

        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"))
        {
            color = _defaultColor
        };
        _laser.material = material;
        _laser.positionCount = 2;
        _laser.startWidth = 0.008f;
        _laser.endWidth = 0.008f;
    }

    // Update is called once per frame
    void Update()
    {
        _laser.SetPosition(0, transform.position);
        //Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green, 0.5f);

        if (Physics.Raycast(transform.position, transform.forward, out _collidedObject, raycastDistance))
        {
            _laser.SetPosition(1, _collidedObject.point);

            if (_collidedObject.collider.gameObject.CompareTag("Button"))
            {
                if (InputManager.GetDown(InputManager.Button.IndexTrigger))
                {
                    _collidedObject.collider.gameObject.GetComponent<Button>().onClick.Invoke();
                }

                else
                {
                    _collidedObject.collider.gameObject.GetComponent<Button>().OnPointerEnter(null);
                    _currentObject = _collidedObject.collider.gameObject;
                }
            }
        }

        else
        {
            _laser.SetPosition(1, transform.position + (transform.forward * raycastDistance));

            if (_currentObject != null)
            {
                _currentObject.GetComponent<Button>().OnPointerExit(null);
                _currentObject = null;
            }
        }
    }

    private void LateUpdate()
    {
        if (InputManager.GetDown(InputManager.Button.IndexTrigger, curController))
        {
            _laser.material.color = _activatedColor;
        }

        else if (InputManager.GetUp(InputManager.Button.IndexTrigger, curController))
        {
            _laser.material.color = _defaultColor;
        }
    }
}
