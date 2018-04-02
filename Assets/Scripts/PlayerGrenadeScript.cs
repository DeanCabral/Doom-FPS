using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrenadeScript : MonoBehaviour {

    // The force at which the grenade is thrown
    public float fl_grenadeForce = 1500f;

    // Use this for initialization
    void Start()
    {
        // Throws grenade
        LaunchGrenade();
    }

    private void LaunchGrenade()
    {
        // Adds force at the forward transform of the camera
        this.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * fl_grenadeForce);

        // Calls the Explode Grenade method after 3 seconds
        Invoke("ExplodeGrenade", 3f);
    }

    private void ExplodeGrenade()
    {
        // Temporary variables for the blast radius and its power
        float fl_radius = 15.0F;
        float fl_power = 10000.0F;

        // Vector3 that stores the position of the grenade
        Vector3 explosionPos = transform.position;
        // An array of colliders which is collected from the Physics.OverlaySphere function
        Collider[] colliders = Physics.OverlapSphere(explosionPos, fl_radius);

        // Loop that runs through each collider that has been detected in the array
        foreach (Collider hit in colliders)
        {
            // Checks if the colliders are of type Enemy
            if (hit.tag == "Enemy")
            {
                // Gets the rigidbody from the hit collider
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Adds an explosion force using the appropriate parameters
                    rb.AddExplosionForce(fl_power, explosionPos, fl_radius, 3.0F);
                }

                // Damages the enemy with the blast
                hit.GetComponent<EnemyBehaviour>().in_enemyHealth -= 80;
                // Checks if enemy has not seen player
                if (!hit.GetComponent<EnemyBehaviour>().BL_SeenPlayer)
                {
                    // Flags enemy's surprised state boolean
                    hit.GetComponent<EnemyBehaviour>().BL_Surprised = true;
                }                
            }    
            else if(hit.tag == "Player")
            {
                // Gets the player behaviour script from the player
                PlayerBehaviour PB = hit.GetComponent<PlayerBehaviour>();
                // Twice the damage because of the grenade's explosive nature
                PB.DecreaseLife();
                PB.DecreaseLife();
            }                 
        }

        // Destroys the game object
        Destroy(gameObject);
    }
}
