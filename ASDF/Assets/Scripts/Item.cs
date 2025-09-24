[System.Serializable]
public class Item
{
    public string itemName;
    public int quantity;

    public Item(string name, int qty = 1)
    {
        itemName = name;
        quantity = qty;
    }
}
