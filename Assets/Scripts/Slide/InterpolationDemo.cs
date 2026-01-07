using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock.LowLevel;

public class InterpolationDemo : MonoBehaviour
{
    // 1. Khai báo cái "khuôn" danh sách (thường viết hoa chữ cái đầu cho đúng chuẩn)
    public enum InterpolationType { Lerp, SmoothDamp, Slerp, MoveTowards } 

    [Header("Settings")]
    public InterpolationType selectedMethod;
    public float speed = 5f;
    public float smoothTime = 0.3f;
    private Vector3 currentVelocity = Vector3.zero; // Bắt buộc phải có cho SmoothDamp

    [Header("Points")]
    public GameObject pointA;
    public GameObject pointB;
    private Vector3 targetPos;
    public float threshold = 0.1f;
    private bool iswaiting = false;
    
    [Header("Other")]
    public Material piston;
    public bool isfading = false;
    void Start()
    {
        transform.position = pointA.transform.position;
        // Bắt đầu bằng việc đi tới điểm B
        if (pointB != null) targetPos = pointB.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (pointA == null || pointB == null) return;

        if (!iswaiting) 
        {
            MoveHandle();

            float distance = Vector3.Distance(transform.position, targetPos);
            if (distance < threshold)
            {
                StartCoroutine(WaitAndSwitchTarget());
            }
        }
        //Fade
        if(Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!isfading)
            {
                StartCoroutine(Fadeblock());
            }
        }
    }
    public IEnumerator WaitAndSwitchTarget()
    {
        iswaiting = true;       //Da den target
        yield return new WaitForSeconds(1f);

        targetPos = (targetPos == pointB.transform.position) 
            ? pointA.transform.position : pointB.transform.position;
        iswaiting = false;
    }
    public IEnumerator Fadeblock()
    {
        isfading = true;
        float duration = 5f;

        Color startColor = piston.color;
        while(duration >= 0f)
        {
            duration -= Time.deltaTime;
            float alpha = Mathf.MoveTowards(100f, 0f, 5* Time.deltaTime);
            piston.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        // Đảm bảo kết thúc là biến mất hoàn toàn
        piston.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        isfading = false;
        Debug.Log("Đã làm mờ xong!");
    }
    void MoveHandle()
    {
        switch(selectedMethod)
        {
            case InterpolationType.Lerp:
                transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
                break;
            case InterpolationType.SmoothDamp:
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothTime);
                break;
            case InterpolationType.Slerp:
                transform.position = Vector3.Slerp(transform.position, targetPos, speed * Time.deltaTime);
                break;
            case InterpolationType.MoveTowards:
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
}
