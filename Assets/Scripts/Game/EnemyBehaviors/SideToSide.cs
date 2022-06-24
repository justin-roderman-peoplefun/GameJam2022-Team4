using UnityEngine;
using Random = System.Random;

public class SideToSide : MonoBehaviour
{
    private float t = 0;
    private float animationTimer = 0;
    [SerializeField] private Transform enemyBody;
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Sprite firstSwimSprite;
    [SerializeField] private Sprite secondSwimSprite;
    
    public float verticalSpeed = 1;
    public float horizontalSpeed = 1;
    public float horizontalRange = 1;
    
    Vector3 Position => transform.position;

    private void Start()
    {
        t = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(Position.x, Position.y + (verticalSpeed / 10f), Position.z);
        
        if (enemyBody != null)
        {
            t += Time.deltaTime * horizontalSpeed;
            animationTimer += Time.deltaTime;
            enemyBody.position = new Vector3(transform.position.x + (Mathf.Cos(t) * horizontalRange), transform.position.y, transform.position.z);

            enemyBody.localScale = new Vector3(Mathf.Abs(enemyBody.localScale.x) * Mathf.Sign(Mathf.Cos(t)), enemyBody.localScale.y, 1f);
        }
    }
}
