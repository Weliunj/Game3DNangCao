using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    [Header("Cấu hình UI")]
    public GameObject inventoryPrefab; // Mẫu thiết kế của 1 ô đồ (Prefab)
    public InventoryObject inventory;  // File dữ liệu rương đồ (ScriptableObject)
    
    [Header("Trạng thái")]
    public bool toggleUI = false;      // Biến kiểm tra rương đang đóng hay mở

    // Dictionary giúp liên kết Dữ liệu (Slot) với Đối tượng hiển thị (GameObject)
    // Giúp cập nhật số lượng cực nhanh mà không cần tạo lại toàn bộ UI
    Dictionary<InventorySlot, GameObject> itemDisplayed = new Dictionary<InventorySlot, GameObject>();

    void Start()
    {
        // Khi game bắt đầu, vẽ rương đồ dựa trên dữ liệu có sẵn
        CreateDisplay(); 
    }

    void Update()
    {
        ToggleUI();      // Lắng nghe phím Tab để đóng/mở rương
        UpdateDisplay(); // Đồng bộ hóa dữ liệu từ ScriptableObject lên màn hình
    }

    // Hàm khởi tạo giao diện rương đồ lần đầu
    public void CreateDisplay()
    {   
        for(int i = 0; i < inventory.Container.Items.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];

            // 1. Sinh ra một ô vật phẩm UI từ mẫu Prefab làm con của đối tượng này (Panel)
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            
            // 2. Tìm hình ảnh trong Database dựa vào ID của vật phẩm và gán vào Icon của ô đồ
            // GetChild(0) thường là đối tượng chứa thành phần Image bên trong Prefab
            obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.id].uiDisplay;
            
            // 3. Hiển thị số lượng (amount) lên Text của ô đồ, định dạng "n0" để có dấu phẩy hàng nghìn
            obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            
            // 4. Lưu ô đồ vừa tạo vào từ điển để quản lý
            itemDisplayed.Add(slot, obj);
        }
    }

    // Hàm cập nhật rương đồ (Thêm ô mới hoặc cập nhật số lượng)
    public void UpdateDisplay()
    {
        for(int i = 0; i < inventory.Container.Items.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];

            // Nếu vật phẩm này ĐÃ được hiển thị trên UI rồi
            if (itemDisplayed.ContainsKey(slot))
            {
                // Chỉ cập nhật lại con số hiển thị cho đúng với dữ liệu mới nhất
                itemDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            }
            else
            {
                // Nếu đây là vật phẩm MỚI nhặt (chưa có ô trên UI): Tạo mới ô hiển thị
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                
                // Gán hình ảnh từ Database
                obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.id].uiDisplay;
                
                // Gán số lượng ban đầu
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
                
                // Đưa vào từ điển để theo dõi
                itemDisplayed.Add(slot, obj);
            }
        }
    }

    // Hàm xử lý logic đóng/mở giao diện và con trỏ chuột
    public void ToggleUI()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) // Kiểm tra phím Tab
        {
            toggleUI = !toggleUI;
            // Ở đây bạn có thể thêm: panelUI.SetActive(toggleUI); để ẩn/hiện cái bảng
        }

        if (toggleUI)
        {
            // MỞ RƯƠNG: Hiện chuột và cho phép di chuyển chuột tự do trên màn hình
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; 
        }
        else
        {
            // ĐÓNG RƯƠNG: Khóa chuột vào tâm màn hình và ẩn đi (dành cho game góc nhìn thứ nhất/thứ ba)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}