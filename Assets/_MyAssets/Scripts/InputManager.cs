//#define PC
#define Oculus

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
#if PC
    public enum ButtonTarget
    {
        Fire1,
        Fire2,
        Fire3,
        Jump,
    }
#endif

    public enum Button
    {
#if PC
        One = ButtonTarget.Fire1,
        Two = ButtonTarget.Jump,
        Thumbstick = ButtonTarget.Fire1,
        IndexTrigger = ButtonTarget.Fire3,
        HandTrigger = ButtonTarget.Fire2
#elif Oculus
        One = OVRInput.Button.One,
        Two = OVRInput.Button.Two,
        Thumbstick = OVRInput.Button.PrimaryThumbstick,
        IndexTrigger = OVRInput.Button.PrimaryIndexTrigger,
        HandTrigger = OVRInput.Button.PrimaryHandTrigger
#endif
    }

    public enum  Controller
    {
#if PC
        LTouch,
        RTouch
#elif Oculus
        LTouch = OVRInput.Controller.LTouch,
        RTouch = OVRInput.Controller.RTouch
#endif
    }

    //��Ʈ�ѷ��� Ư�� ��ư�� ������ �ִ� ���� true ��ȯ
    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        //vitrtualMask�� ���� ���� ButtonTarget Ÿ������ ��ȯ�� �����Ѵ�.
        return Input.GetButton(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.Get((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }

    //��Ʈ�ѷ��� Ư�� ��ư�� ������ �� true ��ȯ
    public static bool GetDown(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        return Input.GetButtonDown(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetDown((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }

    //��Ʈ�ѷ��� Ư�� ��ư�� ������ �� true ��ȯ
    public static bool GetUp(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        return Input.GetButtonUp(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetUp((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }

    //��Ʈ�ѷ��� Axis �Է��� ��ȯ
    //axis: Horizontal, Vertical ���� ���´�
    public static float GetAxis(string axis, Controller hand = Controller.LTouch)
    {
#if PC
        return Input.GetAxis(axis);
#elif Oculus
        if (axis == "Horizontal")
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)hand).x;
        }
        else if (axis == "Vertical")
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)hand).y;
        }
        return 0;
#endif
    }

    //ī�޶� �ٶ󺸴� ������ �������� ���͸� ��´�
    public static void Recenter()
    {
#if Oculus
        OVRManager.display.RecenterPose();
#endif
    }

    // ���ϴ� �������� Ÿ���� ���͸� ����
    public static void Recenter(Transform target, Vector3 direction)
    {
        target.forward = target.rotation * direction;
    }

#if Oculus
    static Transform rootTransform;

    static Transform GetTransform()
    {
        if (rootTransform == null)
        {
            rootTransform = GameObject.Find("TrackingSpace").transform;
        }
        return rootTransform;
    }
#endif

    //���� ��Ʈ�ѷ�
    static Transform lHand;

    //���� ��ϵ� ���� ��Ʈ�ѷ��� ã�� ��ȯ
    public static Transform LHand
    {
        get
        {
            if (lHand == null)
            {
#if PC
                //LHand��� �̸����� ���� ������Ʈ�� �����
                GameObject handObj = new GameObject("LHand");
                //������� ��ü�� Ʈ�������� lHand�� �Ҵ�
                lHand = handObj.transform; ;
                //��Ʈ�ѷ��� ī�޶��� �ڽ� ������Ʈ�� ���
                lHand.parent = Camera.main.transform;
#elif Oculus 
                lHand = GameObject.Find("LeftControllerAnchor").transform;
#endif
            }
            return lHand;
        }
    }

    public static Vector3 LHandPosition
    {
        get
        {
#if PC
            //���콺�� ��ũ�� ��ǥ ������
            Vector3 pos = Input.mousePosition;
            //z���� 0.7m�� ����
            pos.z = 0.7f;

            //��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
            pos = Camera.main.ScreenToWorldPoint(pos);
            LHand.position = pos;
            return pos;
#elif Oculus
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            pos = GetTransform().TransformPoint(pos); //rootTransform�� TransformPoint�Լ�(����->����) �̿�
            return pos;
#endif
        }
    }

    public static Vector3 LHandDirection
    {
        get
        {
#if PC
            Vector3 direction = LHandPosition - Camera.main.transform.position;
            LHand.forward = direction;
            return direction;
#elif Oculus
            Vector3 direction = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
            return direction;
#endif
        }
    }

    //������ ��Ʈ�ѷ�
    static Transform rHand;

    //���� ��ϵ� ������ ��Ʈ�ѷ��� ã�� ��ȯ
    public static Transform RHand
    {
        get
        {
            if (rHand == null)
            {
#if PC
                GameObject handObj = new GameObject("RHand");
                rHand = handObj.transform; ;
                rHand.parent = Camera.main.transform;
#elif Oculus
                lHand = GameObject.Find("RightControllerAnchor").transform;
#endif
            }
            return rHand;
        }
    }

    public static Vector3 RHandPosition
    {
        get
        {
#if PC
            Vector3 pos = Input.mousePosition;
            pos.z = 0.7f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            RHand.position = pos;
            return pos;
#elif Oculus
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            pos = GetTransform().TransformPoint(pos); //rootTransform�� TransformPoint�Լ�(����->����) �̿�
            return pos;
#endif
        }
    }

    public static Vector3 RHandDirection
    {
        get
        {
#if PC
            Vector3 direction = RHandPosition - Camera.main.transform.position;
            RHand.forward = direction;
            return direction;
#elif Oculus
            Vector3 direction = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
            return direction;
#endif
        }
    }

    //��Ʈ�ѷ��� ���� ȣ���ϱ�
    public static void PlayVibration(float duration, float frequency, float amplitude, Controller hand)
    {
#if Oculus
        if (CoroutineInstance.coroutineInstance == null)
        {
            GameObject coroutineObj = new GameObject("CoroutineInstane");
            coroutineObj.AddComponent<CoroutineInstance>();
        }
        //�̹� �÷������� ���� �ڷ�ƾ�� ����
        CoroutineInstance.coroutineInstance.StopAllCoroutines();
        CoroutineInstance.coroutineInstance.StartCoroutine(VibrationCoroutine(duration, frequency, amplitude, hand));
#endif
    }

    public static void PlayVibration(Controller hand)
    {
#if Oculus
        PlayVibration(0.06f, 1, 1, hand);
#endif
    }

#if Oculus
    static IEnumerator VibrationCoroutine(float duration, float frequency, float amplitude, Controller hand)
    {
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            OVRInput.SetControllerVibration(frequency, amplitude, (OVRInput.Controller)hand);
            yield return null;
        }
        //���� ����
        OVRInput.SetControllerVibration(0, 0, (OVRInput.Controller)hand);
    }
#endif
}

// InputManager Ŭ�������� ����� �ڷ�ƾ ��ü(�̱��� ����)
class CoroutineInstance : MonoBehaviour
{
    public static CoroutineInstance coroutineInstance = null;
    private void Awake()
    {
        if (coroutineInstance == null)
        {
            coroutineInstance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
