using System.Collections.Generic;
using UnityEngine;

public class ThingsSpawner : MonoBehaviour
{
    [SerializeField]
    private BicycleController _bicycle;
    public List<DeliveryItem> items;

    [ContextMenu("Put Random")]
    public void Put()
    {
        if (items != null && items.Count > 0)
        {
            var item = items[Random.Range(0, items.Count)];
           items.Remove(item);
           _bicycle.PutOnRack(item);
        }
    }

    [ContextMenu("Take Random")]
    public void Take()
    {
        _bicycle.TakeFromRackById(Random.Range(0, 3).ToString());
    }
}
