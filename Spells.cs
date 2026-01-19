using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{

    public PlayerMovement pm;
    public LayerMask obstacleMask;

    [Header("Spells")]
    public GameObject spellSpawn;

    [Header("Ice Attack")]
    public GameObject iceParticles;

    [Header("Fire Attack")]
    public List<GameObject> fireAttack;
    public GameObject fireAttackCollision;
    
    [Header("Swipe Attack")]
    public GameObject swipeCollision;

    private IEnumerator toUnfreeze;
    GameObject copy;



    private IEnumerator UnfreezeEnemy(GameObject target)
    {
        Debug.Log("Received");

        // Boss gets frozen for half the time
        if (target.GetComponent<HealthManager>().isBoss) {yield return new WaitForSeconds(Upgrades.iceSpellDuration / 2f);}
        else {yield return new WaitForSeconds(Upgrades.iceSpellDuration);}

        Debug.Log("Attempting to unfreeze...");

        try {
        target.transform.GetComponent<DummyMovement>().isFrozen = false;
        } catch {}

        try {
        target.transform.GetComponent<BossHandler>().isFrozen = false;
        } catch {}

    }



    public void ShootSwipeAttack() {
        Debug.Log("Swipe attack");
        
        // Calculate the position and direction for the damage sphere
        Vector3 sphereCenter = transform.position + new Vector3(0, 1f, 0) + transform.forward + transform.right;
        Vector3 direction = transform.forward;

        pm.SlashSound.Play();
        GameObject copy = Instantiate(pm.SlashParticles, swipeCollision.transform.position, swipeCollision.transform.rotation);
        Destroy(copy, 2f);
        GameObject copy1 = Instantiate(pm.SlashParticles, swipeCollision.transform.position, swipeCollision.transform.rotation);
        Destroy(copy1, 2f);
        GameObject copy2 = Instantiate(pm.SlashParticles, swipeCollision.transform.position, swipeCollision.transform.rotation);
        Destroy(copy2, 2f);
        
        // whats in range to be damaged
        Collider[] hits = Physics.OverlapSphere(swipeCollision.transform.position, 2f);
        
        // loop through every target
        foreach (var hit in hits) {
            Debug.Log("Collider name: " + hit.GetComponent<Collider>().gameObject.name);
            
            // If they have health, deal damage
            HealthManager healthManager = hit.GetComponent<Collider>().GetComponent<HealthManager>();
            if (healthManager != null) {
                healthManager.TakeDamage(55f);
                Debug.Log("Damage applied to: " + hit.GetComponent<Collider>().gameObject.name);
            }
        }
    }

    // Reset after swipe attack
    public void FinishSwipeAttack() {
        pm.canActivateSpell = true;
        pm.state = 0;
    }



    public void ShootSnowAttack() {
        RaycastHit hit;

        // See if player is looking at an enemy
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 1000f, obstacleMask))
        {

            Debug.Log(hit.transform.name);
            if (hit.transform.tag == "Enemy") {
                Debug.Log(hit.transform.name);
                

                // If they are not frozen, freeze them in their handler
                if (
                    (hit.transform.GetComponent<DummyMovement>() != null && hit.transform.GetComponent<DummyMovement>().isFrozen == false) ||
                    (hit.transform.GetComponent<BossHandler>() != null && hit.transform.GetComponent<BossHandler>().isFrozen == false)
                ) 
                {

                    try {
                        hit.transform.GetComponent<DummyMovement>().isFrozen = true;
                    } catch {
                        hit.transform.GetComponent<BossHandler>().isFrozen = true; 
                    }

                    toUnfreeze = UnfreezeEnemy(hit.transform.gameObject);
                    StartCoroutine(toUnfreeze);

                    copy = Instantiate(iceParticles, hit.transform.position + new Vector3(0f, 1f, 0f), hit.transform.rotation);
                    Destroy(copy, 3f);
                    

                    Debug.Log("Called from first");
                }
            }
            else 
            {
                copy = Instantiate(iceParticles, hit.point, hit.transform.rotation);
                Destroy(copy, 3f);
            }
        }
    }

    // Reset after freeze attack
    public void FinishSnowAttack() {
        pm.canActivateSpell = true;
        pm.canMove = true;
        pm.state = 0;
    }



    // Spawns fire particles in front of player
    public void SpawnFireAttack() {
        int randomNum = Random.Range(0, fireAttack.Count);
        GameObject randomCopy = fireAttack[randomNum];
        GameObject fireAttackHitbox = fireAttackCollision;

        GameObject spawned = Instantiate(randomCopy, spellSpawn.transform.position, spellSpawn.transform.rotation);
        GameObject fireAttackCollisionCopy = Instantiate(fireAttackCollision, spellSpawn.transform.position, spellSpawn.transform.rotation);
        Destroy(spawned, 1f);
        Destroy(fireAttackCollisionCopy, 1f);
    }
}
