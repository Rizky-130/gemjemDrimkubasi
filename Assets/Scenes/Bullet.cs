using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}