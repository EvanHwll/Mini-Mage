using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class DummyMovement : MonoBehaviour
{

    private NavMeshAgent agent;
    private Vector3 randomDestination;

    [Header("Movement Settings")]
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;

    private float timer;
    public float freezeAtStart;
    public bool canMove = false;
    public bool isMovingEnabled = false; 
    public bool isFrozen = false;

    [Header("References")]
    public GameObject player;
    public GameObject iceBlock;
    public Material normal;
    public Material attacking;

    [Header("Animations")]
    public Animator anim;
    private bool animationPaused = false;
    public float oldSpeed;
    
    [Header("Attacking")]
    public float attackCooldown = 2f;
    private float attackCooldownReal;
    private bool huntingPlayer;
    public bool isAttacking = false;

    private Rigidbody rb;

    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Set to default values
        attackCooldownReal = attackCooldown;
        huntingPlayer = false;
        timer = wanderTimer + freezeAtStart;
        
        anim.SetBool("isWalking", false);

        if (isMovingEnabled) {
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            StartCoroutine(StartMovement());
        }
    }

    IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(freezeAtStart);
        canMove = true;

        if (isMovingEnabled) {
            agent.isStopped = false;
            anim.SetBool("isWalking", true);
        }

        GetNewRandomDestination();
    }



    void Update()
    {

        // Reduce attack cooldown each frame by time since last frame
        if (attackCooldownReal > 0) {
            attackCooldownReal -= Time.deltaTime;
            if (attackCooldownReal < 0) {attackCooldownReal = 0;}
        }

        
        if (isFrozen) {
            // If frozen then pause animation and stop being able to move.
            iceBlock.SetActive(true);

            if (animationPaused == false) {
                animationPaused = true;
                oldSpeed = anim.speed;
                anim.speed = 0f;
            }

        } else {

            // If unfreezing hide ice block and resume animation at the old anim speed.
            iceBlock.SetActive(false);

            if (animationPaused == true) {
                animationPaused = false;
                anim.speed = oldSpeed;
            }

        }


        if (canMove && isMovingEnabled) {

            Debug.Log($"moving is enabled! - {this.gameObject.name}");

            // If the enemy is close to an alive player, always walk towards them instead
            if (isMovingEnabled && Vector3.Distance(transform.position, player.transform.position) <= 50 && player.GetComponent<PlayerMovement>().isAlive) {
                agent.SetDestination(player.transform.position);
                huntingPlayer = true;
            } else {
                huntingPlayer = false;
            }

            // If they are next to an alive player, unfrozen, start attacking them
            if (isMovingEnabled && Vector3.Distance(transform.position, player.transform.position) <= 2 && !isFrozen && canMove && huntingPlayer) {
                anim.SetBool("attacking", true);
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                isAttacking = true;
                transform.LookAt(player.transform);

            } else { isAttacking = false; anim.SetBool("attacking", false); rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;}


            // Stop moving if they want to move but are frozen or attacking
            if (isMovingEnabled && (isFrozen || isAttacking)) {
                agent.isStopped = true;
            } else {
                agent.isStopped = false;
            }

            // Choose new destinations every few seconds or upon reaching current destination to avoid getting stuck on one
            timer -= Time.deltaTime;
            if (isMovingEnabled && timer <= 0f)
            {
                GetNewRandomDestination();
                timer = wanderTimer;
            }

            if (isMovingEnabled && agent.remainingDistance < 0.5f)
            {
                GetNewRandomDestination();
            }
        }
    }

    void GetNewRandomDestination()
    {

        if (huntingPlayer == false && isMovingEnabled) {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            randomDestination = hit.position;
            agent.SetDestination(randomDestination);
            timer = wanderTimer;
        }
    }
}
