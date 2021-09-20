using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    //체력 처리
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool isDead = false;

    //마우스 움직임 처리
    [SerializeField] private bool shouldCameraFreeze = false;
    [SerializeField] private float sensitivityX = 2f;
    [SerializeField] private float sensitivityY = 2f;
    [SerializeField] private Camera cam;
    Quaternion camRotation;
    Quaternion bodyRotation;

    //싱글톤 처리
    static Player instance;
    public static Player Instance { get { Init(); return instance; } }

    static void Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.FindWithTag("Player");
            if (go == null)
            {
                Debug.LogError("Player not found");
            }

            instance = go.GetComponent<Player>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();

        sensitivityX = PlayerPrefs.GetFloat("XSensitivityValue", 2f);
        sensitivityY = PlayerPrefs.GetFloat("YSensitivityValue", 2f);

        currentHealth = maxHealth;
        cam = Camera.main;
        SetCursorLockState(CursorLockMode.Locked);
        camRotation = cam.transform.localRotation;
        bodyRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return; //사망시 조작 불가
        SetCamera(); //마우스 처리
    }

    public void SetCursorLockState(CursorLockMode mode)
    {
        Cursor.lockState = mode;
    }

    void SetCamera()
    {
        if (shouldCameraFreeze) return;

        sensitivityX = PlayerPrefs.GetFloat("XSensitivityValue", 2f);
        sensitivityY = PlayerPrefs.GetFloat("YSensitivityValue", 2f);

        float yRotation = Input.GetAxis("Mouse X") * sensitivityX;
        float xRotation = Input.GetAxis("Mouse Y") * sensitivityY;

        camRotation *= Quaternion.Euler(-xRotation, 0f, 0f);
        camRotation = ClampRotationAroundXAxis(camRotation);
        bodyRotation *= Quaternion.Euler(0f, yRotation, 0f);

        cam.transform.localRotation = camRotation;
        transform.localRotation = bodyRotation;
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, -90f, 90f);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    public void Damage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }
}
