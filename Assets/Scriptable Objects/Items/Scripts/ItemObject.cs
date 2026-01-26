using UnityEngine;

// Định nghĩa các loại vật phẩm trong game để dễ phân loại
public enum ItemType
{
    Food,       // Thức ăn (hồi máu, năng lượng)
    Equipment,  // Trang bị (vũ khí, giáp)
    Default     // Vật phẩm cơ bản (nguyên liệu, rác)
}

// Lớp cha trừu tượng cho mọi vật phẩm, dùng ScriptableObject để lưu dữ liệu dưới dạng file
public abstract class ItemObject : ScriptableObject
{
    public int Id;
    public Sprite uiDisplay;    // Mô hình 3D/hình ảnh hiển thị của vật phẩm trong thế giới game
    public ItemType type;        // Loại vật phẩm (chọn từ danh sách enum ở trên)
    
    [TextArea(10, 15)]           // Tạo khung nhập văn bản rộng (tối thiểu 10 dòng, tối đa 15 dòng) trong Inspector
    public string description;   // Đoạn mô tả chi tiết về vật phẩm
}

[System.Serializable]
public class Item
{
    public string Name;
    public int id;
    public Item(ItemObject item)
    {
        Name = item.name;
        id = item.Id;
    }
}