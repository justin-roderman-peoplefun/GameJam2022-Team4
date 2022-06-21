using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (circleCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
        else
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }
        
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, boxCollider.bounds.size);
        }
        else
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }
    }
#endif
}
