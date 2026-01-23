using StarterAssets;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;
public class Player_Combat : MonoBehaviour
{
    private Animator anim;

    [Header("Atk, Combo")]
    public float cooldownTime = 2f;      // Thời gian chờ giữa các đợt combo lớn
    private float nextFireTime = 0f;     // Thời điểm tiếp theo được phép bấm nút
    public static int noOfClicks = 0;    // Biến tĩnh lưu số lần bấm chuột (dùng chung toàn game)
    float lastClickedTime = 0f;           // Thời điểm lần cuối cùng bấm chuột
    float maxComboDelay = 1f;             // Khoảng thời gian tối đa giữa 2 lần bấm để giữ combo
    public static bool isAttacking;

    [Header("Shield Block")]
    public static bool isBlock;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>(); // Lấy component Animator từ đối tượng
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
            if(!isBlock)
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
        // Kiểm tra xem có đang thực hiện bất kỳ đòn đánh nào không
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        isAttacking = stateInfo.IsName("hit1") || stateInfo.IsName("hit2") || stateInfo.IsName("hit3");

        // --- PHẦN 1: TỰ ĐỘNG RESET ANIMATION ---
        // Nếu đang chơi hit1 và đã chạy được hơn 70%, thì tắt bool hit1 để tránh lặp lại vô tận
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
        {
            anim.SetBool("atk1", false);
        }
        // Tương tự cho hit2
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("hit2"))
        {
            anim.SetBool("atk2", false);
        }
        // Nếu hit3 (đòn cuối) chạy được 70%, tắt bool và reset số lần click về 0 để bắt đầu combo mới
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("hit3"))
        {
            anim.SetBool("atk3", false);
            noOfClicks = 0;
        }

        // --- PHẦN 2: RESET COMBO THEO THỜI GIAN ---
        // Nếu thời gian chờ giữa 2 lần bấm quá lâu (vượt quá 1 giây), reset combo
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }

        // --- PHẦN 3: KIỂM TRA ĐẦU VÀO ---
        // Nếu đã hết thời gian hồi chiêu (Cooldown)
        if (Time.time > nextFireTime)
        {
            if (Input.GetMouseButtonDown(0)) // Nhấp chuột trái
            {
                OnClick();
            }
        }
    }
    void OnClick()
    {
        lastClickedTime = Time.time;    // Ghi lại thời điểm bấm chuột hiện tại
        noOfClicks++;                   // Tăng số lần đếm combo

        // Nếu là lần bấm đầu tiên, kích hoạt đòn đánh 1
        if (noOfClicks == 1)
        {
            anim.SetBool("atk1", true);
        }

        // Giới hạn số lần click trong khoảng 0 đến 3 bằng hàm Clamp
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

        // --- PHẦN 4: CHUYỂN TIẾP COMBO (CHAINING) ---
        // Nếu đã bấm 2 lần VÀ đòn đánh 1 đang diễn ra được hơn 70%, chuyển sang đòn đánh 2
        if (noOfClicks >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
        {
            anim.SetBool("atk1", false);
            anim.SetBool("atk2", true);
        }
        // Nếu đã bấm 3 lần VÀ đòn đánh 2 đang diễn ra được hơn 70%, chuyển sang đòn đánh 3
        if (noOfClicks >= 3 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("hit2"))
        {
            anim.SetBool("atk2", false);
            anim.SetBool("atk3", true);
        }
    }
}