# Claude AI Development Rules - PixelIdleGame Unity 2D Project

> **Purpose**: This document provides detailed guidelines, examples, and patterns for AI-assisted development of the PixelIdleGame Unity 2D project. It emphasizes SOLID principles, Unity best practices, and project-specific architectural patterns.

---

## Table of Contents
1. [AI Development Guidelines](#ai-development-guidelines)
2. [SOLID Principles with Examples](#solid-principles-with-examples)
3. [Unity 2D Architecture Patterns](#unity-2d-architecture-patterns)
4. [State Machine Implementation](#state-machine-implementation)
5. [Component Design Patterns](#component-design-patterns)
6. [Code Review Checklist](#code-review-checklist)
7. [Refactoring Guidelines](#refactoring-guidelines)
8. [Common Pitfalls and Solutions](#common-pitfalls-and-solutions)
9. [Performance Optimization](#performance-optimization)
10. [Testing Strategies](#testing-strategies)
11. [Large File Modification Protocol](#large-file-modification-protocol)
12. [Shader Development Workflow](#shader-development-workflow)

---

## AI Development Guidelines

### Critical Rules for AI Assistants

#### ❌ DO NOT Create Documentation Files Without Request

**Never automatically generate:**
- README.md files
- Documentation files (*.md)
- Instruction files
- Tutorial files
- Guide files
- Design documents
- Architecture documents

**Only create documentation files when:**
1. User explicitly requests: "Create a README", "Write documentation", etc.
2. User specifically asks for a guide or tutorial
3. Project setup absolutely requires it (package.json, requirements.txt, etc.)

#### ✅ Focus on Code, Not Documentation

**Your primary role:**
- Write and modify C# scripts
- Implement Unity components
- Fix bugs and errors
- Refactor code
- Optimize performance
- Follow the architectural patterns defined in this project

**Communication:**
- Explain changes in your responses, not in generated files
- Provide inline code comments where necessary
- Use XML documentation for public APIs
- Discuss architecture in conversation, not in separate documents

#### Example Scenarios

**❌ Bad - Don't Do This:**
```
User: "I added a new enemy type"
AI: Creates EnemyDesignDoc.md, EnemyImplementationGuide.md, README_ENEMIES.md
```

**✅ Good - Do This:**
```
User: "I added a new enemy type"
AI: Responds in chat explaining the implementation, modifies code files only
```

---

## SOLID Principles with Examples

### Single Responsibility Principle (SRP)

#### ✅ Good Example from Project
```csharp
// AttackComponent.cs - ONLY handles attack logic
public class AttackComponent : MonoBehaviour, IBaseAttackComponent
{
    public float attackCooldown = 1f;
    public int damageAmount = 100;
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask damageableLayer;

    public void Attack()
    {
        if (CanAttack)
        {
            DetectDamageable();
        }
    }

    private void DetectDamageable() { ... }
}
```

**Why it's good**: 
- Focuses ONLY on attack mechanics
- Doesn't handle health, movement, or other concerns
- Clear, single purpose

#### ❌ Bad Example to Avoid
```csharp
// PlayerController.cs - Does EVERYTHING (BAD!)
public class PlayerController : MonoBehaviour
{
    // Movement
    void HandleMovement() { ... }
    
    // Attack
    void HandleAttack() { ... }
    
    // Health
    void HandleHealth() { ... }
    
    // UI Updates
    void UpdateUI() { ... }
    
    // Inventory
    void ManageInventory() { ... }
}
```

**Why it's bad**: 
- Too many responsibilities
- Hard to maintain and test
- Changes to one feature risk breaking others

#### 🔧 Refactoring Solution
Split into focused components:
- `MoveComponent` - handles movement
- `AttackComponent` - handles attacks
- `HealthComponent` - manages health
- `PlayerUIController` - updates UI
- `InventoryComponent` - manages inventory

---

### Open/Closed Principle (OCP)

#### ✅ Good Example - Interface-Based Extension
```csharp
// IDamageable.cs - Open for extension
namespace Component.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float damage, Vector2 hitDirection);
        void Die();
        void OnHit();
    }
}

// Multiple implementations without modifying the interface
public class PlayerHealth : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        // Player-specific damage handling
        health -= damage;
        PlayHitAnimation();
        ApplyKnockback(hitDirection);
    }
}

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        // Enemy-specific damage handling
        health -= damage;
        DropLoot();
    }
}
```

**Why it's good**:
- New damage types don't require modifying existing code
- Easy to add new entities that can take damage
- Closed to modification, open to extension

#### ✅ Good Example - ScriptableObject Configuration
```csharp
// AttackSo.cs - Configuration without code modification
[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack")]
public class AttackSo : ScriptableObject
{
    public int damageAmount;
    public float attackRange;
    public float cooldown;
    public AttackType type;
}

// Usage - extend by creating new ScriptableObject assets, not code
public class AttackComponent : MonoBehaviour
{
    [SerializeField] private AttackSo attackData;
    
    public void Attack()
    {
        // Use attackData configuration
        int damage = attackData.damageAmount;
    }
}
```

---

### Liskov Substitution Principle (LSP)

#### ✅ Good Example - Proper State Inheritance
```csharp
// BaseState.cs - Establishes contract
public abstract class BaseState<T, TEvent> where TEvent : Enum
{
    protected T owner;
    protected StateMachine<T, TEvent> stateMachine;

    public virtual void Initialize(T owner, StateMachine<T, TEvent> stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}

// PlayerIdleState.cs - Maintains contract
public class PlayerIdleState : BaseState<Player, PlayerEvent>
{
    public override void Enter()
    {
        base.Enter(); // Respects base behavior
        // Add idle-specific logic
    }

    public override void Update()
    {
        base.Update(); // Maintains expected behavior
        // Add idle-specific updates
    }
}
```

**Why it's good**:
- Derived states can replace base state without breaking functionality
- Base contract is maintained
- Predictable behavior across all states

#### ❌ Bad Example to Avoid
```csharp
// Breaking the contract
public class BrokenState : BaseState<Player, PlayerEvent>
{
    public override void Initialize(T owner, StateMachine<T, TEvent> stateMachine)
    {
        // DON'T DO THIS - breaks expected behavior
        throw new NotImplementedException("This state doesn't support initialization");
    }
}
```

---

### Interface Segregation Principle (ISP)

#### ✅ Good Example - Focused Interfaces
```csharp
// Small, focused interfaces from the project
namespace Component.Interfaces
{
    // Only damage-related methods
    public interface IDamageable
    {
        void TakeDamage(float damage, Vector2 hitDirection);
        void Die();
        void OnHit();
    }

    // Only health-related methods
    public interface IHealthComponent
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
    }

    // Only attack-related methods
    public interface IBaseAttackComponent
    {
        bool CanAttack { get; }
        void Attack();
    }
}
```

**Why it's good**:
- Classes only implement what they need
- No forced implementation of unused methods
- Clear, specific contracts

#### ❌ Bad Example to Avoid
```csharp
// Fat interface - forces unnecessary implementations
public interface IGameEntity
{
    // Movement (not all entities move)
    void Move(Vector2 direction);
    
    // Combat (not all entities attack)
    void Attack();
    void TakeDamage(float damage);
    
    // Inventory (not all entities have inventory)
    void AddItem(Item item);
    void RemoveItem(Item item);
    
    // AI (not all entities have AI)
    void UpdateAI();
}
```

---

### Dependency Inversion Principle (DIP)

#### ✅ Good Example - Depend on Abstractions
```csharp
// AttackComponent depends on IDamageable interface, not concrete class
public class AttackComponent : MonoBehaviour
{
    private void DetectDamageable()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRange, damageableLayer);

        foreach (Collider2D hitObject in hitObjects)
        {
            // Depends on interface, not concrete implementation
            IDamageable damageable = hitObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount, hitObject.transform.position);
            }
        }
    }
}
```

**Why it's good**:
- AttackComponent works with ANY IDamageable implementation
- No coupling to specific classes like Player or Enemy
- Easy to test with mock implementations

#### ❌ Bad Example to Avoid
```csharp
// Depending on concrete implementations (BAD!)
public class AttackComponent : MonoBehaviour
{
    private void DetectEnemy()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(...);
        
        foreach (Collider2D hitObject in hitObjects)
        {
            // Tightly coupled to Enemy class
            Enemy enemy = hitObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.health -= damageAmount;
            }
            
            // Need to duplicate for Player, Boss, etc.
            Player player = hitObject.GetComponent<Player>();
            if (player != null)
            {
                player.health -= damageAmount;
            }
        }
    }
}
```

---

## Unity 2D Architecture Patterns

### Component-Based Architecture

The project follows Unity's component-based pattern. Here's the philosophy:

```
GameObject (Player)
├── MoveComponent        → Handles movement
├── JumpComponent        → Handles jumping
├── DashComponent        → Handles dashing
├── AttackComponent      → Handles attacks
├── HealthComponent      → Manages health
└── AnimationComponent   → Manages animations
```

#### Design Guidelines

1. **Each component has ONE job**
2. **Components communicate via interfaces or events**
3. **Components are reusable across different entities**
4. **Components can be enabled/disabled independently**

#### Example Component Structure
```csharp
using Component.Interfaces;
using UnityEngine;

namespace Component
{
    /// <summary>
    /// Handles character jumping mechanics.
    /// Implements interface for cross-component communication.
    /// </summary>
    public class JumpComponent : MonoBehaviour, IJumpComponent
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        
        private Rigidbody2D rb;
        private bool isGrounded;
        
        public bool CanJump => isGrounded;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void FixedUpdate()
        {
            CheckGround();
        }
        
        public void Jump()
        {
            if (CanJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
        
        private void CheckGround()
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position, 
                groundCheckRadius, 
                groundLayer
            );
        }
        
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
```

---

## State Machine Implementation

### Generic State Machine Pattern

The project uses a generic state machine: `BaseState<T, TEvent>`

#### State Machine Structure
```
StateMachine<Player, PlayerEvent>
├── PlayerIdleState
├── PlayerMoveState
├── PlayerJumpState
├── PlayerAttackState
├── PlayerDashState
└── PlayerHitState
```

#### Creating a New State

```csharp
using UnityEngine;
using Utils;

/// <summary>
/// Handles player idle behavior.
/// </summary>
[StateDescription("Player is standing still")]
[StateDebugColor(0.5f, 0.5f, 1.0f)] // Light blue
public class PlayerIdleState : BaseState<Player, PlayerEvent>
{
    public override void Enter()
    {
        base.Enter();
        // Set idle animation
        owner.Animator.Play("Idle");
    }

    public override void Update()
    {
        base.Update();
        
        // Transition conditions
        if (owner.InputComponent.MoveInput.magnitude > 0.1f)
        {
            stateMachine.TransitionTo<PlayerMoveState>();
        }
        
        if (owner.InputComponent.JumpPressed)
        {
            stateMachine.TransitionTo<PlayerJumpState>();
        }
        
        if (owner.InputComponent.AttackPressed)
        {
            stateMachine.TransitionTo<PlayerAttackState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Cleanup if needed
    }
}
```

#### State Machine Best Practices

1. **States should be self-contained**
   - Don't access other states directly
   - Transition through the state machine

2. **Use Enter() for initialization**
   ```csharp
   public override void Enter()
   {
       base.Enter();
       StartAnimation();
       ResetTimers();
       SetupPhysics();
   }
   ```

3. **Use Exit() for cleanup**
   ```csharp
   public override void Exit()
   {
       base.Exit();
       StopEffects();
       ResetFlags();
   }
   ```

4. **Use Update() for transitions**
   ```csharp
   public override void Update()
   {
       base.Update();
       CheckTransitions();
       UpdateAnimation();
   }
   ```

5. **Use FixedUpdate() for physics**
   ```csharp
   public override void FixedUpdate()
   {
       base.FixedUpdate();
       ApplyMovement();
       ApplyPhysicsForces();
   }
   ```

---

## Component Design Patterns

### Pattern 1: Component with Interface

```csharp
// 1. Define interface
namespace Component.Interfaces
{
    public interface IHealthComponent
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        event System.Action OnDeath;
    }
}

// 2. Implement component
namespace Component
{
    public class HealthComponent : MonoBehaviour, IHealthComponent, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;
        
        public event System.Action OnDeath;
        
        private void Awake()
        {
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(float damage, Vector2 hitDirection)
        {
            if (!IsAlive) return;
            
            currentHealth -= damage;
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Die()
        {
            currentHealth = 0;
            OnDeath?.Invoke();
        }
        
        public void OnHit()
        {
            // Visual feedback, sound, etc.
        }
        
        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }
    }
}
```

### Pattern 2: Component with ScriptableObject Configuration

```csharp
// 1. Create ScriptableObject
[CreateAssetMenu(fileName = "NewMovement", menuName = "Character/Movement")]
public class MovementSO : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    
    [Header("Air Movement")]
    public float airControl = 0.7f;
    public float maxFallSpeed = 20f;
}

// 2. Use in component
namespace Component
{
    public class MoveComponent : MonoBehaviour
    {
        [SerializeField] private MovementSO movementData;
        private Rigidbody2D rb;
        private Vector2 moveInput;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }
        
        private void FixedUpdate()
        {
            ApplyMovement();
        }
        
        private void ApplyMovement()
        {
            float targetSpeed = moveInput.x * movementData.moveSpeed;
            float speedDif = targetSpeed - rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) 
                ? movementData.acceleration 
                : movementData.deceleration;
            
            float movement = speedDif * accelRate;
            rb.AddForce(movement * Vector2.right);
        }
    }
}
```

### Pattern 3: Event-Driven Communication

```csharp
// 1. Define events
namespace Constant.Events
{
    public static class PlayerEvents
    {
        public static event System.Action<float> OnHealthChanged;
        public static event System.Action OnPlayerDeath;
        public static event System.Action<int> OnScoreChanged;
        
        public static void TriggerHealthChanged(float newHealth)
        {
            OnHealthChanged?.Invoke(newHealth);
        }
        
        public static void TriggerPlayerDeath()
        {
            OnPlayerDeath?.Invoke();
        }
    }
}

// 2. Trigger events
public class HealthComponent : MonoBehaviour
{
    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        PlayerEvents.TriggerHealthChanged(currentHealth);
        
        if (currentHealth <= 0)
        {
            PlayerEvents.TriggerPlayerDeath();
        }
    }
}

// 3. Listen to events
public class PlayerUIController : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerEvents.OnHealthChanged += UpdateHealthBar;
        PlayerEvents.OnPlayerDeath += ShowGameOver;
    }
    
    private void OnDisable()
    {
        PlayerEvents.OnHealthChanged -= UpdateHealthBar;
        PlayerEvents.OnPlayerDeath -= ShowGameOver;
    }
    
    private void UpdateHealthBar(float health) { ... }
    private void ShowGameOver() { ... }
}
```

---

## Code Review Checklist

### Before Submitting Code

#### SOLID Compliance
- [ ] Each class has a single, well-defined responsibility
- [ ] New functionality uses extension (inheritance/interfaces) not modification
- [ ] Derived classes can substitute base classes without issues
- [ ] Interfaces are small and focused
- [ ] Dependencies are on interfaces, not concrete classes

#### Unity Best Practices
- [ ] Components are cached in Awake() or Start()
- [ ] No GetComponent() calls in Update() or FixedUpdate()
- [ ] Physics logic is in FixedUpdate()
- [ ] Input and game logic is in Update()
- [ ] Proper cleanup in OnDestroy() or OnDisable()

#### Code Quality
- [ ] XML documentation for all public methods and classes
- [ ] Null safety checks before using references
- [ ] Proper use of SerializeField for inspector values
- [ ] No magic numbers (use constants or SerializeField)
- [ ] Debug visualization with Gizmos where appropriate

#### Performance
- [ ] LayerMask used for Physics2D queries
- [ ] No excessive memory allocations in Update()
- [ ] String operations minimized (especially concatenation)
- [ ] Appropriate use of object pooling for frequent instantiation

#### Namespace and Naming
- [ ] Proper namespace matching folder structure
- [ ] Interfaces prefixed with `I`
- [ ] Components suffixed with `Component`
- [ ] ScriptableObjects suffixed with `So`
- [ ] Managers suffixed with `Manager`
- [ ] States suffixed with `State`

#### Project Specific
- [ ] Follows component-based architecture
- [ ] Uses state machine pattern where appropriate
- [ ] Data in ScriptableObjects when possible
- [ ] Events used for cross-system communication
- [ ] Consistent with existing code style

---

## Refactoring Guidelines

### When to Refactor

1. **File exceeds 500 lines** → Split into multiple files
2. **Class has multiple responsibilities** → Extract classes
3. **Duplicate code in multiple places** → Create shared component/utility
4. **Hard to test** → Introduce interfaces and dependency injection
5. **Hard to understand** → Simplify and document

### Refactoring Process

#### Step 1: Identify Code Smells
```csharp
// CODE SMELL: God Class
public class PlayerController : MonoBehaviour
{
    // 50+ fields for different responsibilities
    // 1000+ lines of code
    // Handles movement, combat, inventory, UI, audio, etc.
}
```

#### Step 2: Plan the Refactoring
Break down responsibilities:
- Movement → `MoveComponent`
- Combat → `AttackComponent`, `HealthComponent`
- Inventory → `InventoryComponent`
- UI → `PlayerUIController`
- Audio → `AudioManager` (singleton)

#### Step 3: Extract Interfaces First
```csharp
public interface IMoveComponent
{
    void Move(Vector2 direction);
    float MoveSpeed { get; }
}

public interface IHealthComponent
{
    float CurrentHealth { get; }
    void TakeDamage(float damage);
}
```

#### Step 4: Implement Components
```csharp
public class MoveComponent : MonoBehaviour, IMoveComponent
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    
    public float MoveSpeed => moveSpeed;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Move(Vector2 direction)
    {
        rb.velocity = direction * moveSpeed;
    }
}
```

#### Step 5: Update References
```csharp
public class Player : MonoBehaviour
{
    // Instead of doing everything
    private IMoveComponent moveComponent;
    private IHealthComponent healthComponent;
    private IAttackComponent attackComponent;
    
    private void Awake()
    {
        moveComponent = GetComponent<IMoveComponent>();
        healthComponent = GetComponent<IHealthComponent>();
        attackComponent = GetComponent<IAttackComponent>();
    }
    
    // Delegate to components
    public void Move(Vector2 direction) => moveComponent.Move(direction);
    public void TakeDamage(float damage) => healthComponent.TakeDamage(damage);
}
```

### Refactoring Patterns

#### Pattern 1: Extract Component
**Before:**
```csharp
public class Player : MonoBehaviour
{
    private float health = 100f;
    
    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
    }
    
    void Die()
    {
        Destroy(gameObject);
    }
}
```

**After:**
```csharp
// HealthComponent.cs
public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        Destroy(gameObject);
    }
}

// Player.cs
public class Player : MonoBehaviour
{
    // Much cleaner - health is handled by component
}
```

#### Pattern 2: Extract ScriptableObject
**Before:**
```csharp
public class Enemy : MonoBehaviour
{
    // Hardcoded values
    private float moveSpeed = 3f;
    private int health = 50;
    private int damage = 10;
}
```

**After:**
```csharp
// EnemyDataSo.cs
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy/Data")]
public class EnemyDataSo : ScriptableObject
{
    public float moveSpeed = 3f;
    public int health = 50;
    public int damage = 10;
    public Sprite sprite;
    public RuntimeAnimatorController animator;
}

// Enemy.cs
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyDataSo enemyData;
    
    private void Awake()
    {
        // Use data from ScriptableObject
        GetComponent<SpriteRenderer>().sprite = enemyData.sprite;
    }
}
```

---

## Common Pitfalls and Solutions

### Pitfall 1: GetComponent in Update

#### ❌ Problem
```csharp
void Update()
{
    // Called every frame - VERY SLOW!
    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    GetComponent<Animator>().SetBool("Moving", true);
}
```

#### ✅ Solution
```csharp
private Rigidbody2D rb;
private Animator animator;

void Awake()
{
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
}

void Update()
{
    rb.velocity = Vector2.zero;
    animator.SetBool("Moving", true);
}
```

### Pitfall 2: No Null Checks

#### ❌ Problem
```csharp
public void Attack()
{
    // Crashes if attackPoint is null!
    Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, range);
}
```

#### ✅ Solution (from AttackComponent)
```csharp
public void Attack()
{
    if (attackPoint == null)
    {
        attackPoint = transform;
        Debug.LogWarning("Attack point not set. Defaulting to transform.");
    }
    
    Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, range);
}
```

### Pitfall 3: Missing LayerMask

#### ❌ Problem
```csharp
void DetectEnemies()
{
    // Hits EVERYTHING including walls, ground, UI, etc.
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
}
```

#### ✅ Solution
```csharp
[SerializeField] private LayerMask enemyLayer;

void DetectEnemies()
{
    // Only hits objects on enemy layer - MUCH faster!
    Collider2D[] hits = Physics2D.OverlapCircleAll(
        transform.position, 
        range, 
        enemyLayer
    );
}
```

### Pitfall 4: String Concatenation in Update

#### ❌ Problem
```csharp
void Update()
{
    // Creates garbage every frame!
    Debug.Log("Health: " + health + "/" + maxHealth);
}
```

#### ✅ Solution
```csharp
// Option 1: Use conditional compilation
void Update()
{
    #if UNITY_EDITOR
    Debug.Log($"Health: {health}/{maxHealth}");
    #endif
}

// Option 2: Only log on changes
private float lastLoggedHealth;

void TakeDamage(float damage)
{
    health -= damage;
    if (health != lastLoggedHealth)
    {
        Debug.Log($"Health: {health}/{maxHealth}");
        lastLoggedHealth = health;
    }
}
```

### Pitfall 5: Not Using Interfaces

#### ❌ Problem
```csharp
void AttackEnemy(Enemy enemy)
{
    enemy.health -= damage;
}

void AttackPlayer(Player player)
{
    player.health -= damage;
}

void AttackBoss(Boss boss)
{
    boss.health -= damage;
}
// Need a method for every type!
```

#### ✅ Solution
```csharp
void Attack(IDamageable target)
{
    target.TakeDamage(damage);
}
// Works with ANY damageable entity!
```

---

## Performance Optimization

### Unity-Specific Optimizations

#### 1. Component Caching
```csharp
public class OptimizedComponent : MonoBehaviour
{
    // Cache all components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform cachedTransform;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cachedTransform = transform; // Even transform should be cached!
    }
}
```

#### 2. Object Pooling
```csharp
public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 20;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        
        return Instantiate(projectilePrefab);
    }
    
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

#### 3. Efficient Physics Queries
```csharp
public class OptimizedDetection : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float detectionInterval = 0.2f; // Don't check every frame
    
    private Collider2D[] results = new Collider2D[10]; // Reuse array
    private float nextDetectionTime;
    
    private void Update()
    {
        if (Time.time >= nextDetectionTime)
        {
            DetectTargets();
            nextDetectionTime = Time.time + detectionInterval;
        }
    }
    
    private void DetectTargets()
    {
        // Use NonAlloc version to avoid garbage
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            detectionRadius,
            results,
            targetLayer
        );
        
        for (int i = 0; i < count; i++)
        {
            ProcessTarget(results[i]);
        }
    }
}
```

#### 4. Avoid Allocations in Hot Paths
```csharp
public class AllocationOptimized : MonoBehaviour
{
    // Reuse these instead of creating new ones
    private Vector2 moveDirection;
    private Collider2D[] hitResults = new Collider2D[10];
    private List<IDamageable> damageableTargets = new List<IDamageable>(10);
    
    private void Update()
    {
        // Reuse existing Vector2
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");
        
        // No new allocation
        Move(moveDirection);
    }
}
```

---

## Testing Strategies

### Unit Testing Components

```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class HealthComponentTests
{
    private GameObject testObject;
    private HealthComponent healthComponent;
    
    [SetUp]
    public void Setup()
    {
        testObject = new GameObject();
        healthComponent = testObject.AddComponent<HealthComponent>();
    }
    
    [TearDown]
    public void Teardown()
    {
        Object.Destroy(testObject);
    }
    
    [Test]
    public void TakeDamage_ReducesHealth()
    {
        // Arrange
        float initialHealth = healthComponent.CurrentHealth;
        float damage = 10f;
        
        // Act
        healthComponent.TakeDamage(damage, Vector2.zero);
        
        // Assert
        Assert.AreEqual(initialHealth - damage, healthComponent.CurrentHealth);
    }
    
    [Test]
    public void TakeDamage_WhenHealthReachesZero_TriggersDeathEvent()
    {
        // Arrange
        bool deathEventFired = false;
        healthComponent.OnDeath += () => deathEventFired = true;
        
        // Act
        healthComponent.TakeDamage(healthComponent.MaxHealth, Vector2.zero);
        
        // Assert
        Assert.IsTrue(deathEventFired);
        Assert.IsFalse(healthComponent.IsAlive);
    }
}
```

### Integration Testing

```csharp
[UnityTest]
public IEnumerator AttackComponent_DamagesTargetWithIDamageable()
{
    // Arrange
    GameObject attacker = new GameObject();
    AttackComponent attackComp = attacker.AddComponent<AttackComponent>();
    
    GameObject target = new GameObject();
    target.AddComponent<CircleCollider2D>();
    HealthComponent healthComp = target.AddComponent<HealthComponent>();
    
    float initialHealth = healthComp.CurrentHealth;
    
    // Position target in range
    target.transform.position = attacker.transform.position + Vector3.right;
    
    // Act
    attackComp.Attack();
    
    yield return new WaitForFixedUpdate();
    
    // Assert
    Assert.Less(healthComp.CurrentHealth, initialHealth);
    
    // Cleanup
    Object.Destroy(attacker);
    Object.Destroy(target);
}
```

---

## Large File Modification Protocol

### When a File Exceeds 500 Lines

#### Step 1: Assess the File
```
File: PlayerController.cs
Lines: 847
Sections:
- Movement (lines 50-200)
- Combat (lines 201-400)
- Inventory (lines 401-600)
- UI Updates (lines 601-750)
- Audio (lines 751-847)
```

#### Step 2: Inform User
```
⚠️ LARGE FILE WARNING

File: PlayerController.cs
Current Lines: 847 lines (exceeds 500 line threshold)

Requested Modification: Add new dash ability

Analysis:
- This file handles 5 distinct responsibilities
- Violates Single Responsibility Principle
- High risk of bugs when modifying

Options:
1. Make minimal change to existing file (quick, technical debt)
2. Refactor into separate components first (recommended, cleaner)
   - MoveComponent (movement + dash)
   - CombatComponent (attack logic)
   - InventoryComponent (inventory management)
   - PlayerUIController (UI updates)
   - AudioManager (audio handling)

Recommendation: Option 2 - Refactor for long-term maintainability

How would you like to proceed?
```

#### Step 3: Execute Based on User Choice

**Option A: Minimal Change**
```csharp
// Add only what's necessary
// Document technical debt
// TODO: Refactor PlayerController - see issue #123
public void Dash()
{
    // Dash implementation
}
```

**Option B: Proper Refactoring**
1. Extract DashComponent
2. Update PlayerController to use component
3. Test thoroughly
4. Document changes

---

## Shader Development Workflow

### Before Writing Any Shader Code

1. **Search Unity Documentation**
   - ShaderLab Reference: https://docs.unity3d.com/Manual/SL-Reference.html
   - Shader Graph for 2D: https://docs.unity3d.com/Manual/shader-graph-2d.html
   - Built-in Functions: https://docs.unity3d.com/Manual/SL-BuiltinFunctions.html

2. **Check for Built-in Solutions**
   - URP/2D Renderer features
   - Shader Graph nodes
   - Post-processing effects

3. **Plan the Shader**
   - What visual effect is needed?
   - What properties should be adjustable?
   - Performance requirements?

### Shader Template for 2D Games

```hlsl
/*
 * Shader: Sprite Dissolve Effect
 * Purpose: Dissolves sprites using a noise texture
 * Unity Docs: https://docs.unity3d.com/Manual/SL-Shader.html
 * Target: Universal Render Pipeline 2D
 * Properties:
 *   - _DissolveAmount: Controls dissolution progress (0-1)
 *   - _NoiseTex: Noise texture for dissolve pattern
 *   - _EdgeColor: Color of dissolve edge
 *   - _EdgeWidth: Width of colored edge
 * Performance: Optimized for mobile 2D
 */

Shader "Custom/2D/SpriteDissolve"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Dissolve Settings)]
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1, 0.5, 0, 1)
        _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.1
        
        [Header(Unity Sprite Settings)]
        [Toggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            fixed4 _Color;
            fixed4 _EdgeColor;
            float _DissolveAmount;
            float _EdgeWidth;
            fixed4 _RendererColor;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;
                
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Sample sprite texture
                fixed4 c = tex2D(_MainTex, IN.texcoord);
                
                // Sample noise texture
                fixed noise = tex2D(_NoiseTex, IN.texcoord).r;
                
                // Calculate dissolve
                fixed dissolveEdge = _DissolveAmount + _EdgeWidth;
                fixed dissolveValue = step(noise, _DissolveAmount);
                fixed edgeValue = step(noise, dissolveEdge) * (1 - dissolveValue);
                
                // Apply edge color
                c.rgb = lerp(c.rgb, _EdgeColor.rgb, edgeValue);
                
                // Apply dissolve (clip pixels)
                c.a *= (1 - dissolveValue);
                
                // Apply tint
                c *= IN.color;
                
                // Premultiply alpha
                c.rgb *= c.a;
                
                return c;
            }
            ENDCG
        }
    }
}
```

### Shader Usage in C#

```csharp
using UnityEngine;

/// <summary>
/// Controls sprite dissolve effect shader.
/// Shader Reference: Custom/2D/SpriteDissolve
/// </summary>
public class DissolveEffect : MonoBehaviour
{
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveSpeed = 1f;
    
    private SpriteRenderer spriteRenderer;
    private float currentDissolve = 0f;
    
    // Shader property IDs (cached for performance)
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");
    private static readonly int EdgeColorID = Shader.PropertyToID("_EdgeColor");
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Create instance of material
        spriteRenderer.material = new Material(dissolveMaterial);
    }
    
    private void Update()
    {
        if (currentDissolve < 1f)
        {
            currentDissolve += Time.deltaTime * dissolveSpeed;
            
            // Use property ID for performance
            spriteRenderer.material.SetFloat(DissolveAmountID, currentDissolve);
        }
    }
    
    public void StartDissolve()
    {
        currentDissolve = 0f;
    }
    
    private void OnDestroy()
    {
        // Clean up material instance
        if (spriteRenderer.material != null)
        {
            Destroy(spriteRenderer.material);
        }
    }
}
```

---

## Summary

This document provides comprehensive guidelines for developing the PixelIdleGame Unity 2D project with AI assistance. Key principles:

1. **SOLID principles** are mandatory for all code
2. **Component-based architecture** for modularity
3. **State machines** for complex behaviors
4. **ScriptableObjects** for data management
5. **Interface-driven** design for flexibility
6. **Performance-conscious** implementations
7. **Proper documentation** and code quality
8. **Large file warnings** at 500+ lines
9. **Unity documentation** reference for shaders

Always prioritize:
- Code quality over speed
- Maintainability over cleverness
- Unity best practices over shortcuts
- Clear communication about technical decisions

When in doubt, refer to this document and ask for clarification before proceeding.

