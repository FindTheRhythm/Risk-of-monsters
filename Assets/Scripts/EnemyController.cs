using UnityEngine;

/// <summary>
/// Базовый класс врага.
/// Отвечает за поведение, движение к игроку и нанесение урона.
/// Используется как родительский класс для всех типов врагов.
/// </summary>
public class EnemyController : MonoBehaviour, IDamageable
{
    /// <summary>Максимальное здоровье</summary>
    public float maxHealth = 50f;

    /// <summary>Текущее здоровье</summary>
    protected float currentHealth;

    /// <summary>Урон врага</summary>
    public float damage = 10f;

    /// <summary>Скорость передвижения</summary>
    public float moveSpeed = 3f;

    /// <summary>Дистанция атаки</summary>
    public float attackRange = 1.5f;

    /// <summary>Задержка между атаками</summary>
    public float attackCooldown = 1.5f;

    /// <summary>Ссылка на игрока</summary>
    protected Transform player;

    /// <summary>Время последней атаки</summary>
    protected float lastAttackTime;

    /// <summary>Физика</summary>
    protected Rigidbody2D rb;

    /// <summary>Аниматор</summary>
    protected Animator animator;

    /// <summary>
    /// Инициализация врага
    /// </summary>
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealth;
    }

    /// <summary>
    /// Основной цикл поведения
    /// </summary>
    protected virtual void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            MoveToPlayer();
        }
        else
        {
            Attack();
        }
    }

    /// <summary>
    /// Движение к игроку
    /// </summary>
    protected virtual void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    /// <summary>
    /// Атака игрока
    /// </summary>
    protected virtual void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        IDamageable damageable = player.GetComponent<IDamageable>();

        if (damageable != null)
        {
            Damage dmg = new Damage
            {
                value = damage,
                isCrit = false,
                source = gameObject
            };

            damageable.TakeDamage(dmg);
        }

        if (animator != null)
            animator.SetTrigger("Attack");
    }

    /// <summary>
    /// Получение урона
    /// </summary>
    /// <param name="damage">Структура урона</param>
    public virtual void TakeDamage(Damage damage)
    {
        currentHealth -= damage.value;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Смерть врага
    /// </summary>
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}