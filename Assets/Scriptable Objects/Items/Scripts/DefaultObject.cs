using UnityEngine;

[CreateAssetMenu(fileName = "New default Object", menuName = "Inventory System/Items/Default")]
public class DefaultObject : ItemObject
{
    void Awake()
    {
        type = ItemType.Default;
    }
}
