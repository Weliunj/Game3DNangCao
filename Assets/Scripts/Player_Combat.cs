using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    private Animator anim;

    [Header("Atk, Combo")]
    public float cooldownTime = 0.5f;     // Giảm xuống một chút cho mượt
    private float nextFireTime = 0f;
    public static int noOfClicks = 0;           // Bỏ static nếu không cần dùng ở script khác
    float lastClickedTime = 0f;
    float maxComboDelay = 1f;
    public static bool isAttacking;

    [Header("Shield Block")]
    public static bool isBlock;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Atk();
        Block();
    }

    void Block()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (!isBlock)
            {
                anim.SetBool("Block", true);
                isBlock = true;
            }
        }
        else
        {
            anim.SetBool("Block", false);
            isBlock = false;
        }
    }

    void Atk()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        isAttacking = stateInfo.IsName("hit1") || stateInfo.IsName("hit2") || stateInfo.IsName("hit3");

        // --- PHẦN 1: LOGIC CHUYỂN COMBO (Update liên tục để bắt đúng thời điểm) ---
        
        // Nếu đang ở hit1, gần xong (70%) và người chơi đã bấm ít nhất 2 lần -> Sang hit2
        if (stateInfo.IsName("hit1") && stateInfo.normalizedTime > 0.7f)
        {
            anim.SetBool("atk1", false);
            if (noOfClicks >= 2) anim.SetBool("atk2", true);
        }

        // Nếu đang ở hit2, gần xong và người chơi đã bấm 3 lần -> Sang hit3
        if (stateInfo.IsName("hit2") && stateInfo.normalizedTime > 0.7f)
        {
            anim.SetBool("atk2", false);
            if (noOfClicks >= 3) anim.SetBool("atk3", true);
        }

        // Nếu đang ở hit3, gần xong -> Reset hết
        if (stateInfo.IsName("hit3") && stateInfo.normalizedTime > 0.7f)
        {
            anim.SetBool("atk3", false);
            noOfClicks = 0;
        }

        // --- PHẦN 2: RESET NẾU ĐỨNG YÊN QUÁ LÂU ---
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
            anim.SetBool("atk1", false);
            anim.SetBool("atk2", false);
            anim.SetBool("atk3", false);
        }

        // --- PHẦN 3: NHẬN INPUT ---
        if (Input.GetMouseButtonDown(0) && Time.time > nextFireTime && !isBlock)
        {
            OnClick();
        }
    }

    void OnClick()
    {
        lastClickedTime = Time.time;
        noOfClicks++;
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

        if (noOfClicks == 1)
        {
            anim.SetBool("atk1", true);
        }
    }
}