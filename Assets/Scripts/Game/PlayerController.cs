using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject cursor;

    public float playerSpeed;
    public float maxPlayerSpeed;
    public float maxRotationAngle;

    public float doubleTapTime;
    public float boostTime;
    public float boostFactor;

    private Rigidbody2D _rb;

    private Vector2 _targetPos;
    private Vector2 _playerVelocity = Vector2.zero;

    private Coroutine _cursorFadeOut;

    private float _lastTap = Single.NegativeInfinity;
    private bool _boost;
    private float _lastBoostTime;

    private int life;
    private bool isShielded;

    bool CanAcceptInput
    {
        get
        {
            return (StageManager.IsStagePlaying && life > 0);
        }
    }

    public bool IsShielded => isShielded;

    public static PlayerController Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Debug.LogError("There was more than one player controller in the scene. Deleting the player named: <color=cyan>" + gameObject.name + "</color>.");
            Destroy(gameObject); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _targetPos = _rb.position;
        life = 3;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!StageManager.IsStagePlaying)
            return;
        
        if (other.gameObject.CompareTag("Enemy") && life > 0)
        {
            Instance.TakeDamage();
            Destroy(other.transform.parent.gameObject);
        }
        else if (other.gameObject.CompareTag("HeartResource") && life > 0)
        {
            if(GameManager.Instance != null)
                GameManager.Instance.EarnHearts(1);

            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("StageEnd") && life > 0)
        {
            if(StageManager.Instance != null)
                StageManager.Instance.StageComplete();
            
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("ShieldResource") && life > 0)
        {
            ShieldPlayer();
            
            Destroy(other.gameObject);
        }
    }

    private void InputMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Show the cursor and reset the color
            cursor.SetActive(true);
            cursor.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            if(_cursorFadeOut != null) StopCoroutine(_cursorFadeOut);

            // Handle boosting
            if (!_boost)
            {
                float time = Time.time;
                if (time - _lastTap <= doubleTapTime)
                {
                    _boost = true;
                    _lastBoostTime = time;
                    //GetComponent<SpriteRenderer>().color = Color.red;
                }
                _lastTap = time;                
            }
        }

        if (Input.GetMouseButton(0))
        {
            _targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            cursor.transform.position = _targetPos; // Make the cursor follow the mouse
        }

        if (Input.GetMouseButtonUp(0))
        {
            _cursorFadeOut = StartCoroutine(FadeOutCursor());
        }
    }

    private void Move()
    {
        float smoothTime = 1 / (playerSpeed * (_boost ? boostFactor : 1));
        float maxSpeed = maxPlayerSpeed * (_boost ? boostFactor : 1);
        // If we've hit the target position, move it up a bit so that we drift
        Vector2 targetPos = _targetPos + (Vector2.Distance(_rb.position, _targetPos) > 0.05f ? Vector2.zero : new Vector2(0.05f, 0));
        _rb.position = Vector2.SmoothDamp(_rb.position, targetPos, ref _playerVelocity, smoothTime, maxSpeed);

        float rot = maxRotationAngle * Mathf.Clamp(_playerVelocity.x, -1, 1);
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    private void UpdateBoostTime()
    {
        if (_boost && Time.time - _lastBoostTime >= boostTime)
        {
            //GetComponent<SpriteRenderer>().color = Color.white;
            _boost = false;
        }
    }

    private void Update()
    {
        if (CanAcceptInput)
        {
            InputMovement();
            Move();
        
            UpdateBoostTime(); 
        }
    }

    IEnumerator DeathAnimation()
    {
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        float timer = 0f;
        float cosTimer = (Mathf.PI / 2);
        float originalXPos = transform.position.x;
        float animationLength = 2f;

        Vector3 startRotation = transform.eulerAngles;
        Vector3 endRotation = new Vector3(0f, 0f, 180f);

        gameObject.AddComponent<ScrollUpwardsPlain>().speed = 0.1f;
        
        while (timer < animationLength)
        {
            timer += Time.deltaTime;
            playerSprite.material.SetFloat("_GrayscaleAmount", Mathf.Lerp(0f, 1f, (timer / animationLength)));
            transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, (timer / animationLength));
            
            cosTimer += Time.deltaTime * 2.5f;
            transform.position = new Vector3(originalXPos + (Mathf.Cos(cosTimer) * 0.15f), transform.position.y, transform.position.z);
            yield return null;
        }

        while (true)
        {
            cosTimer += Time.deltaTime * 2.5f;
            transform.position = new Vector3(originalXPos + (Mathf.Cos(cosTimer) * 0.15f), transform.position.y, transform.position.z);
            yield return null;
        }
        yield return null;
    }
    
    private IEnumerator FadeOutCursor()
    {
        Color cursorColor;
        do
        {
            cursorColor = cursor.GetComponent<SpriteRenderer>().color;
            cursorColor.a -= Time.deltaTime;
            cursor.GetComponent<SpriteRenderer>().color = cursorColor;
            yield return null;
        } while (cursorColor.a > 0);
        cursor.SetActive(false);
    }

    public void TakeDamage()
    {
        if (life <= 0)
            return;

        if (isShielded)
        {
            Debug.Log("Player was hit, but had a shield! Life remaining: <color=red>" + life + "</color>");
            isShielded = false;
            return;
        }
        
        life--;
        Debug.Log("Player has taken damage! Life remaining: <color=red>" + life + "</color>");
        if (life <= 0)
        {
            Debug.Log("Player has died! <color=red>Game Over</color>");
            StartCoroutine(DeathAnimation());
            // TODO Replace with death screen
            StartCoroutine(ReturnToMainMenu());
        }
    }

    public void ShieldPlayer()
    {
        Debug.Log("Player has gained a shield!");
        isShielded = true;
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(5);
        GameManager.Instance.BubbleTransitionScene("MainMenuScene");
    }
}
