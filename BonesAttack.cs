using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonesAttack : MonoBehaviour
{

    public BossHandler movement;
    public ThirdPersonCam playerCam;

    public GameObject bonesPrefab;
    public GameObject bonesSpawn1;
    public GameObject bonesSpawn2;

    [Header("Sounds")]
    public AudioSource LiftUpArms;
    public AudioSource BonesShoot;

    [Header("Damage Settings")]
    public float bonesDamage = 10f;
    public int projectileCount = 10;
    public float spreadAngle = 30f;
    public float verticalSpreadAngle = 30f;
    public float shotForce = 10f;
    public float blastRadius = 5f;


    public void StartBonesAttack()
    {
        movement.isAttacking = true;
    }

    public void PlayBonesAttack1()
    {
        LiftUpArms.Play();
    }
    public void PlayBonesAttack2()
    {
        BonesShoot.Play();
        playerCam.DoShake();
    }

    public void InitiateBonesAttack()
    {

        float horizontalSpreadAngle = 25f;
        float verticalSpreadAngle = 25f;


        // Left Hand
        for (int i = 0; i < projectileCount; i++)
        {
            // Spawn bonePrefab at the specified position and rotation
            GameObject bone = Instantiate(bonesPrefab, bonesSpawn1.transform.position, bonesSpawn1.transform.rotation);

            // Calculate random deviation angles for horizontal and vertical spread
            float horizontalDeviation = Random.Range(-horizontalSpreadAngle, horizontalSpreadAngle);
            float verticalDeviation = Random.Range(-verticalSpreadAngle, verticalSpreadAngle);

            // Calculate direction to launch the bone with random deviation
            Vector3 launchDirection = Quaternion.Euler(verticalDeviation, horizontalDeviation, 0f) * transform.forward;

            // Apply force to the bone's rigidbody
            Rigidbody boneRigidbody = bone.GetComponent<Rigidbody>();
            if (boneRigidbody != null)
            {
                Debug.Log("Made bone");
                boneRigidbody.AddForce(launchDirection * shotForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Rigidbody component not found on bonesPrefab.");
            }
        }


        // Right Hand
        for (int i = 0; i < projectileCount; i++)
        {
            // Spawn bonePrefab at the specified position and rotation
            GameObject bone = Instantiate(bonesPrefab, bonesSpawn2.transform.position, bonesSpawn2.transform.rotation);

            // Calculate random deviation angles for horizontal and vertical spread
            float horizontalDeviation = Random.Range(-horizontalSpreadAngle, horizontalSpreadAngle);
            float verticalDeviation = Random.Range(-verticalSpreadAngle, verticalSpreadAngle);

            // Calculate direction to launch the bone with random deviation
            Vector3 launchDirection = Quaternion.Euler(verticalDeviation, horizontalDeviation, 0f) * transform.forward;

            // Apply force to the bone's rigidbody
            Rigidbody boneRigidbody = bone.GetComponent<Rigidbody>();
            if (boneRigidbody != null)
            {
                Debug.Log("Made bone");
                boneRigidbody.AddForce(launchDirection * shotForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Rigidbody component not found on bonesPrefab.");
            }
        }

    }

    public void FinishBonesAttack()
    {
        movement.isAttacking = false;
    }
}
