
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{

    public NavMeshAgent agent;

    public LayerMask groundMask, playerMask;
    public GameObject projectileModel;
    public int attackDelay;
    public float pointRange, sightRange, attackRange;

    private Transform player;
    private Vector3 walkPoint;
    private bool isSetPoint, playerInSight, playerInAttack, isAttackPerformed, isDead = false;
    private int health = 100;


    void Awake()
    {
        player = GameObject.Find("Capsule Collider").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            transform.localScale *= 0.999f;
            return;
        }

        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttack = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (!playerInSight && !playerInAttack) Patrol();
        if (playerInSight && !playerInAttack) MoveToPlayer();
        if (playerInAttack && playerInSight) AttackPlayer();
    }



    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            agent.enabled = false;
            transform.Find("Halo")?.gameObject.SetActive(false);
            Destroy(this.gameObject, 10f);
        }
    }

    private void AttackPlayer()
    {

        agent.SetDestination(transform.position);

        transform.LookAt(player.position + Vector3.up);

        if (!isAttackPerformed)
        {

            GameObject projectile = Instantiate(projectileModel, transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 3f, ForceMode.Impulse);


            isAttackPerformed = true;
            Invoke(nameof(ResetAttack), attackDelay);
            Destroy(projectile, 5f);
        }
    }

    private void ResetAttack()
    {
        isAttackPerformed = false;
    }

    private void Patrol()
    {
        if (!isSetPoint)
        {
            float randomZ = Random.Range(-pointRange, pointRange);
            float randomX = Random.Range(-pointRange, pointRange);

            walkPoint = new Vector3(transform.position.x + randomX,
                                    transform.position.y,
                                    transform.position.z + randomZ);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(walkPoint, out hit, 2f, NavMesh.AllAreas))
            {
                walkPoint = hit.position;
                isSetPoint = true;
            }
        }
        else if (isSetPoint)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToPoint = walkPoint - transform.position;

        if (distanceToPoint.magnitude < 1.0f)
        {
            isSetPoint = false;
        }

    }

    private void MoveToPlayer()
    {
        agent.SetDestination(player.position);
    }



}
