using UnityEngine;

public class BatAttackTrigger : MonoBehaviour
{
    public int damage = 25;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Bat HIT Player!");

            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                // ⭐ TRUYỀN VỊ TRÍ CỦA DƠI
                pm.TakeDamage(damage, transform.position);
            }
        }
    }
}
