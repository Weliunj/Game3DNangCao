using Unity.VisualScripting;
using UnityEngine;

public class EnviromentManager : MonoBehaviour
{
    [Header("Cycle Settings")]
    [Range(0, 1)] public float timeValue; 
    public float dayCycleSpeed = 0.1f;    
    public bool isLooping = true;         

    [Header("Skybox & Sun")]
    public Material skyboxMaterial; 
    public Light sunLight; 

    private void Start()
    {
        if (skyboxMaterial != null)
        {
            // Tạo bản sao để không ảnh hưởng file gốc
            skyboxMaterial = new Material(skyboxMaterial);
            RenderSettings.skybox = skyboxMaterial;
        }
    }

    private void Update()
    {
        if (isLooping)
        {
            AutoLoopTime();
        }
        UpdateEnvironment();
    }

    private void AutoLoopTime()
    {
        // Chạy liên tục từ 0 -> 1 rồi reset về 0
        timeValue += Time.deltaTime * dayCycleSpeed;
        if (timeValue >= 1f) 
        { 
            timeValue = 0f; 
        }
    }

    private void UpdateEnvironment()
    {
        // 1. Xoay mặt trời 360 độ
        if (sunLight != null)
        {
            // Nhân timeValue với 360 để quay đủ một vòng tròn
            // Trục X quay từ 0 -> 360
            sunLight.transform.rotation = Quaternion.Euler(timeValue * 360f, -90f, 0f);

            // 2. Cường độ mặt trời (Tự động tắt khi mặt trời lặn xuống dưới đất)
            // Mặt trời sẽ sáng khi góc quay từ 0 đến 180 độ
            if (timeValue > 0f && timeValue < 0.5f) 
            {
                // Bình minh đến hoàng hôn (tăng dần rồi giảm dần)
                float sunIntensity = Mathf.Sin(timeValue * Mathf.PI); 
                sunLight.intensity = Mathf.Lerp(0f, 1f, sunIntensity);
            }
            else
            {
                sunLight.intensity = 0f; // Ban đêm tắt đèn
            }
        }

        // 3. Cường độ ánh sáng môi trường (Ambient)
        // Dùng hàm Sin để buổi trưa (0.25) là sáng nhất, ban đêm là tối nhất
        float ambientAlpha = Mathf.Sin(timeValue * Mathf.PI);
        // Nếu alpha < 0 (ban đêm) thì lấy 0
        ambientAlpha = Mathf.Max(0, ambientAlpha);
        RenderSettings.ambientIntensity = Mathf.Lerp(0.25f, 1.5f, ambientAlpha);

        // 4. Độ sáng Skybox (Nếu có)
        if (skyboxMaterial != null)
        {
            // Tính toán giá trị V: Đêm 0.1 (tối) -> Ngày 0.8 (sáng)
            // Bạn có thể thay đổi số 10f và 55f cũ bằng khoảng 0.1f đến 0.8f
            float vValue = Mathf.Lerp(0.1f, 0.55f, ambientAlpha);

            // Tạo màu mới từ HSV (Giữ nguyên Hue và Saturation là 0 để có màu xám/trắng)
            // Nếu muốn Skybox có màu, hãy thay số 0 đầu tiên bằng Hue bạn muốn
            Color finalSkyColor = Color.HSVToRGB(0, 0, vValue);

            // Tên thuộc tính màu trong Shader Skybox thường là "_Tint" hoặc "_SkyColor"
            // Kiểm tra Shader của bạn để dùng đúng tên
            if (skyboxMaterial.HasProperty("_Tint"))
                skyboxMaterial.SetColor("_Tint", finalSkyColor);
            else if (skyboxMaterial.HasProperty("_SkyColor"))
                skyboxMaterial.SetColor("_SkyColor", finalSkyColor);
        }
    }
}