using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;           // Tên file lưu (ví dụ: /inventory.save)
    public ItemDatabaseObject database; // Cơ sở dữ liệu để tra cứu ID vật phẩm
    public Inventory Container;

// Hàm OnEnable: Chạy khi đối tượng rương đồ này được kích hoạt/nạp lên

    // Thêm vật phẩm vào rương
    public void AddItem(Item _item, int _amount)
    {
        for(int i = 0; i < Container.Items.Count; i++)
        {
            if (Container.Items[i].item == _item) 
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        // Khi tạo ô mới, lưu cả ID từ database để phục vụ việc Save/Load
        Container.Items.Add(new InventorySlot(_item.id, _item, _amount));
    }

    /* --- PHẦN SAVE/LOAD/CLEAR DỮ LIỆU --- */
    [ContextMenu("Save")] // Chuột phải vào Script trong Inspector để hiện nút Save
    public void Save()
    {
        /*
        // 1. Chuyển đối tượng hiện tại thành chuỗi JSON
        string saveData = JsonUtility.ToJson(this, true);       //true: định dạng dễ đọc
        // 2. Công cụ mã hóa nhị phân để bảo mật file
        BinaryFormatter bf = new BinaryFormatter();
        // 3. Tạo file tại đường dẫn lưu trữ cố định của hệ điều hành
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        // 4. Mã hóa chuỗi JSON và ghi vào file
        bf.Serialize(file, saveData);
        file.Close(); // Luôn đóng file sau khi ghi xong
        Debug.Log("Đã lưu tại: " + Application.persistentDataPath);
        */

        // Tạo bộ mã hóa nhị phân
        IFormatter formatter = new BinaryFormatter();
        // Tạo luồng dữ liệu (Stream) trỏ đến file cần tạo
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        // Mã hóa toàn bộ đối tượng Container và đẩy vào file
        formatter.Serialize(stream, Container);
        // Đóng luồng để hoàn tất ghi file
        stream.Close();
    }
    [ContextMenu("Load")]   
    public void Load()
    {
        string path = string.Concat(Application.persistentDataPath, savePath);
        // Kiểm tra xem file có tồn tại hay không
        if(File.Exists(path)){
            /*
            BinaryFormatter bf = new BinaryFormatter();
            // Mở file ra để đọc
            FileStream file = File.Open(path, FileMode.Open);
            // Giải mã nhị phân thành chuỗi và ghi đè dữ liệu vào rương đồ này
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
            */
            IFormatter formatter = new BinaryFormatter();
            // Mở luồng để đọc file hiện có
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            // Giải mã dữ liệu từ file và ép kiểu ngược lại thành lớp Inventory
            Container = (Inventory)formatter.Deserialize(stream);
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }
}

[System.Serializable]
public class Inventory
{
    public List<InventorySlot> Items = new List<InventorySlot>();
}
[System.Serializable]
public class InventorySlot
{
    public int ID;          // ID để lưu trữ (vì Unity không lưu được file Asset trực tiếp)
    public Item item; // File Asset vật phẩm (dùng để hiển thị trong game)
    public int amount;      // Số lượng
    
    public InventorySlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}