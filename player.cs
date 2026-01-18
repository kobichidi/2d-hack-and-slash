using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character, IDamageable
{ [Header("Input")]
    public KeyCode meleeAttackKey = KeyCode.Mouse0;
    public KeyCode rangedAttackKey = KeyCode.Mouse1;
    public KeyCode jumpKey = KeyCode.Space;
    public string xMoveAxis ="Horizontal";

[Header("Combat")]
public  Transform meleeAttackOrigin = null;
public Transform rangedAttackOrigin = null;
public GameObject projectile= null;
public float meleeAttackRadius = 0.6f;
public float meleeDamage =2f;
public float meleeAttackDelay = 1.1f;
public float rangedAttackDelay =0.3f;
public LayerMask enemyLayer = 8;

    
    private float moveIntentionX = 0;
    private bool attemptJump =false;
    private bool attemptMeleeAttack = false;
    private bool attemptRangedAttacked = false;

    private float timeUntilMeleeReadied = 0;
    private float timeUntilRangedReadied= 0;
    private bool isMeleeAttacking= false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    // Update is called once per frame
    void Update()
    {
       GetInput(); 

       HandleJump();
       HandleMeleeAttack();
       HandleRangedAttack();
       HandleAnimations();
    }
    void FixedUpdate()
    {
     HandleRun();
    }
    void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, -Vector2.up* groundedLeeway,  Color.green);
        if (meleeAttackOrigin !=null)
        {
            Gizmos.DrawWireSphere(meleeAttackOrigin.position, meleeAttackRadius);
        }
    }
    private void GetInput()
    {
        moveIntentionX = Input.GetAxis(xMoveAxis);
        attemptMeleeAttack = Input.GetKeyDown(meleeAttackKey);
        attemptRangedAttacked= Input.GetKeyDown(rangedAttackKey);
        attemptJump = Input.GetKeyDown(jumpKey);
    }

    private void HandleRun()
    {
     if(moveIntentionX > 0 && transform.rotation.y ==0)
    {
          transform.rotation= Quaternion.Euler(0, 180f, 0);
     }
     else if ( moveIntentionX < 0 && transform.rotation.y !=0)
     {
        transform.rotation = Quaternion.Euler(0, 0, 0);
     }
     Rb2D.linearVelocity= new Vector2(moveIntentionX* speed, Rb2D.linearVelocity.y);
    }
    private void HandleJump()
    {
        if(attemptJump && CheckGrounded())
        {
            Rb2D.linearVelocity = new Vector2(Rb2D.linearVelocity.x, jumpForce);
        }
    }
    private void HandleMeleeAttack()
    {
        if (attemptMeleeAttack && timeUntilMeleeReadied <= 0)
        {
            Debug.Log("Player:Attwmpting Melee Attack!");
            Collider2D[] overlappedColiders = Physics2D.OverlapCircleAll(meleeAttackOrigin.position, meleeAttackRadius,enemyLayer);
            for(int i=0; i< overlappedColiders.Length;i++)
            {
                IDamageable enemyAtrributes =overlappedColiders[i].GetComponent<IDamageable>();
                if( enemyAtrributes != null)
                {
                    enemyAtrributes.ApplyDamage(meleeDamage);
                }
            }
            timeUntilMeleeReadied = meleeAttackDelay;
        }
        else
        {
            timeUntilMeleeReadied -= Time.deltaTime;
        }
    }
    private void HandleRangedAttack()
    {
        
        if (attemptRangedAttacked&& timeUntilRangedReadied <= 0)
        {
            Debug.Log("Player:Attwmpting Ranged Attack!");
           Instantiate(projectile, rangedAttackOrigin.position, rangedAttackOrigin.rotation);
            timeUntilRangedReadied = rangedAttackDelay;
        }
        else
        {
            timeUntilRangedReadied -= Time.deltaTime;
        }
    }
    private void HandleAnimations()
    {
        Animator.SetBool("Grounded",CheckGrounded());
        if (attemptMeleeAttack)
        {
            if (!isMeleeAttacking)
            {
                Animator.SetTrigger("Attack");
            }
        }
        if( attemptJump && CheckGrounded() || Rb2D.linearVelocity.y> 1f)
        {
            if (!isMeleeAttacking)
            {
                 Animator.SetTrigger("Jump");
            } 
        }
    }
      public virtual void ApplyDamage(float amount)
    {
        CurrentHealth-=amount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

}
