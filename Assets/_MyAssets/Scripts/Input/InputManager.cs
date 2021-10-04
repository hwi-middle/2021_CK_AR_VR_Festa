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

    public enum Controller
    {
#if PC
        LTouch,
        RTouch
#elif Oculus
        LTouch = OVRInput.Controller.LTouch,
        RTouch = OVRInput.Controller.RTouch
#endif
    }

    //컨트롤러의 특정 버튼을 누르고 있는 동안 true 반환
    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        //virtualMask에 들어온 값을 ButtonTarget 타입으로 반환해 전달한다.
        return Input.GetButton(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.Get((OVRInput.Button) virtualMask, (OVRInput.Controller) hand);
#endif
    }

    //컨트롤러의 특정 버튼을 눌렀을 때 true 반환
    public static bool GetDown(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        return Input.GetButtonDown(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetDown((OVRInput.Button) virtualMask, (OVRInput.Controller) hand);
#endif
    }

    //컨트롤러의 특정 버튼을 떼었을 때 true 반환
    public static bool GetUp(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        return Input.GetButtonUp(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetUp((OVRInput.Button) virtualMask, (OVRInput.Controller) hand);
#endif
    }

    //컨트롤러의 Axis 입력을 변환
    //axis: Horizontal, Vertical 값을 갖는다
    public static float GetAxis(string axis, Controller hand = Controller.LTouch)
    {
#if PC
        return Input.GetAxis(axis);
#elif Oculus
        return axis switch
        {
            "Horizontal" => OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller) hand).x,
            "Vertical" => OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller) hand).y,
            _ => 0
        };
#endif
    }

    //카메라가 바라보는 방향을 기준으로 센터를 잡는다
    public static void Recenter()
    {
#if Oculus
        OVRManager.display.RecenterPose();
#endif
    }

    // 원하는 방향으로 타깃의 센터를 설정
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

    //왼쪽 컨트롤러
    private static Transform lHand;

    //씬에 등록된 왼쪽 컨트롤러를 찾아 반환
    public static Transform LHand
    {
        get
        {
            if (lHand == null)
            {
#if PC
                //LHand라는 이름으로 게임 오브젝트를 만든다
                GameObject handObj = new GameObject("LHand");
                //만들어진 객체의 트랜스폼을 lHand에 할당
                lHand = handObj.transform; ;
                //컨트롤러를 카메라의 자식 오브젝트로 등록
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
            //마우스의 스크린 좌표 얻어오기
            Vector3 pos = Input.mousePosition;
            //z값은 0.7m로 설정
            pos.z = 0.7f;

            //스크린 좌표를 월드 좌표로 변환
            pos = Camera.main.ScreenToWorldPoint(pos);
            LHand.position = pos;
            return pos;
#elif Oculus
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            pos = GetTransform().TransformPoint(pos); //rootTransform의 TransformPoint함수(로컬->월드) 이용
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

    //오른쪽 컨트롤러
    private static Transform rHand;

    //씬에 등록된 오른쪽 컨트롤러를 찾아 반환
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
            pos = GetTransform().TransformPoint(pos); //rootTransform의 TransformPoint함수(로컬->월드) 이용
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

    //컨트롤러에 진동 호출하기
    public static void PlayVibration(float duration, float frequency, float amplitude, Controller hand)
    {
#if Oculus
        if (CoroutineInstance.coroutineInstance == null)
        {
            GameObject coroutineObj = new GameObject("CoroutineInstane");
            coroutineObj.AddComponent<CoroutineInstance>();
        }

        //이미 플레이중인 진동 코루틴은 정지
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
            OVRInput.SetControllerVibration(frequency, amplitude, (OVRInput.Controller) hand);
            yield return null;
        }

        //진동 끄기
        OVRInput.SetControllerVibration(0, 0, (OVRInput.Controller) hand);
    }
#endif
}

// InputManager 클래스에서 사용할 코루틴 객체(싱글톤 적용)
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
