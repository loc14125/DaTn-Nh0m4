using UnityEngine;

public class PortalSpawn : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        // Khi bắt đầu game, chạy animation mở
        animator.Play("Open");
    }
}
