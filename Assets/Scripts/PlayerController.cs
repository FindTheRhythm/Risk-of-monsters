using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Основной класс игрока.
/// Отвечает за управление персонажем, обработку ввода,
/// взаимодействие с компонентами и хранение характеристик.
/// </summary>
public class PlayerController : MonoBehaviour, IDamageable
{
    /// <summary>Скорость передвижения</summary>
    public float moveSpeed = 5f;

    /// <summary>Сила прыжка</summary>
    public float jumpForce = 10f;

    /// <summary>Скорость рывка</summary>
    public float dashForce = 15f;

    /// <summary>Максимальное здоровье</summary>
    public float maxHealth = 100f;

    /// <summary>Текущие здоровье</summary>
    public float currentHealth;

    /// <summary>Базовый урон</summary>
    public float damage = 10f;

    /// <summary>Скорость атаки</summary>
    public float attackSpeed = 1f;

    /// <summary>Шанс критического удара</summary>
    public float critChance = 0.1f;

    /// <summary>Критический множитель</summary>
    public float critMultiplier = 2f;

    /// <summary>Список способностей</summary>
    public List<AbilityData> abilities;

    private Rigidbody2D rb;
    private bool isGrounded;

    private float horizontalInput;

    /// <summary>
    /// Инициализация компонентов
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Основной игровой цикл
    /// </summary>
    private void Update()
    {
        HandleInput();
        HandleAbilities();
    }

    /// <summary>
    /// Обработка пользовательского ввода
    /// </summary>
    private void HandleInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
    }

    /// <summary>
    /// Физическое движение
    /// </summary>
    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Перемещение персонажа
    /// </summary>
    private void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    /// <summary>
    /// Прыжок
    /// </summary>
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    /// <summary>
    /// Рывок
    /// </summary>
    private void Dash()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        rb.AddForce(new Vector2(direction * dashForce, 0f), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Обработка способностей
    /// </summary>
    private void HandleAbilities()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseAbility(i);
            }
        }
    }

    /// <summary>
    /// Использование способности
    /// </summary>
    /// <param name="index">Индекс способности</param>
    public void UseAbility(int index)
    {
        if (index < 0 || index >= abilities.Count)
            return;

        AbilityData ability = abilities[index];

        // TODO: добавить кулдаун и реальную логику
        float finalDamage = damage + ability.damage;

        if (Random.value < critChance)
        {
            finalDamage *= critMultiplier;
        }

        Debug.Log("Use ability: " + ability.name + " Damage: " + finalDamage);
    }

    /// <summary>
    /// Получение урона
    /// </summary>
    /// <param name="damage">Объект урона</param>
    public void TakeDamage(Damage damage)
    {
        currentHealth -= damage.value;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Смерть игрока
    /// </summary>
    private void Die()
    {
        Debug.Log("Player died");
        GameManager.instance.GameOver();
    }

    /// <summary>
    /// Проверка столкновения с землей
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}