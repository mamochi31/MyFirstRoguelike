using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 12f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("敵にぶつかった！");
            // 例：敵を消す
            // Destroy(collision.gameObject);

            // または：敵の色を変える
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

}
