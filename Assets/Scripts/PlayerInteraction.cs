using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InventoryObject inventory;
    public float interactionDistance = 3f; 
    public LayerMask interactableLayer;   
    public RectTransform Crosshair;

    [Header("Animation Settings")]
    public float animationSpeed = 10f; // Tăng tốc độ này lên để thấy hiệu ứng mượt hơn

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.Load();
        }
        // 1. Tạo tia Ray từ tâm màn hình
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Vẽ tia Ray trong cửa sổ Scene để bạn dễ debug (chỉ thấy khi đang chạy game)
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);

        // 2. Kiểm tra va chạm tia Ray
        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            var _item = hit.collider.GetComponent<ItemWorld>();

            if (_item != null)
            {
                // Gọi hiệu ứng phóng to
                ScaleCrosshair(1.1f);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    inventory.AddItem(new Item(_item.item), 1);

                    if (hit.collider.transform.parent != null)
                        hit.collider.transform.parent.gameObject.SetActive(false);
                    else
                        hit.collider.gameObject.SetActive(false);
                }
            }
            else
            {
                // Có chạm nhưng không phải Item
                ScaleCrosshair(0.7f);
            }
        }
        else
        {
            // KHÔNG chạm vào bất cứ thứ gì thuộc interactableLayer
            ScaleCrosshair(0.7f);
        }
    }
    private void ScaleCrosshair(float targetScale)
    {
        Vector3 target = new Vector3(targetScale, targetScale, targetScale);
        // Sử dụng tốc độ cao hơn (ví dụ: 10f) để thấy rõ sự thay đổi
        Crosshair.localScale = Vector3.Lerp(Crosshair.localScale, target, animationSpeed * Time.deltaTime);
    }
    private void OnApplicationQuit()    // Được gọi tự động khi bạn nhấn Stop (tắt game) hoặc thoát ứng dụng
    {
        inventory.Container.Items.Clear();
    }
}