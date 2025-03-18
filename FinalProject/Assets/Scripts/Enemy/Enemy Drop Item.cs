using UnityEngine;

public class EnemyDropItem : MonoBehaviour
{
    public GameObject[] items;  // List items enemy sẽ drop khi chết
    public float[] dropChances = { 0.3f, 0.3f }; // List tỉ lệ rơi ra tương ứng
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropItem()
    {
        if (items.Length == 0 || dropChances.Length != items.Length)
        {
            Debug.LogWarning("Items or dropChances list is invalid!");
            return;
        }

        //Random chọn 1 item trong list items[]
        int randomIndex = Random.Range(0, items.Length);
        GameObject selectedItem = items[randomIndex];
        float selectedDropChance = dropChances[randomIndex];
        Debug.Log("Selected item to drop: " +  selectedItem.name);


        //Random để quyết định có drop hay không
        float rand = Random.value; // Random value từ 0 -> 1
        if (rand <= selectedDropChance)
        {
            Vector3 spawnPos = transform.position; // Spawnpos = enemy pos

            Instantiate(selectedItem, spawnPos, Quaternion.identity);
        }
    }
}
