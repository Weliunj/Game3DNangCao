using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "Inventory System/Items/Food")]
public class FoodObject : ItemObject
{
    void Awake()
    {
        type = ItemType.Food;
    }
    public int restoreHungerValue;
    public float staminaRegenBoost;
    public float duration; 
}
