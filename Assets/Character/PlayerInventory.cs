using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public int fragments = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddFragments(int amount)
    {
        fragments += amount;
        Debug.Log("Nhận " + amount + " mảnh. Tổng: " + fragments);
    }
}