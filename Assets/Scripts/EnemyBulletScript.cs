using UnityEngine;
using System.Collections;

public class EnemyBulletScript : MonoBehaviour {

    // Public float that holds the bullet speed
    public float fl_bulletSpeed = 2000f;
    // Reference to the enemy weapon game object
    public GameObject GO_EnemyWeapon;
    // Reference to the player behaviour script
    private PlayerBehaviour PB;

    // Use this for initialization
    void Start()
    {
        // Assigns player behaviour component to the created variable
        PB = GameObject.Find("Player").GetComponent<PlayerBehaviour>(); 
        // Adds a force to the bullet using a forward transform from a target game object
        this.GetComponent<Rigidbody>().AddForce(transform.forward * fl_bulletSpeed);
        // Calls the Destroy Bullet method after 2 seconds
        Invoke("DestroyBullet", 2f);
    }

    void OnCollisionEnter(Collision coll)
    {
        // Checks if the bullet collides with the player
        if (coll.gameObject.tag == "Player")
        {
            // Decreases the players health
            PB.DecreaseLife();
            // Calls the Destroy Bullet method on collision with another game object
            DestroyBullet();
        }        
    }

    private void DestroyBullet()
    {
        // Destroys the game object
        Destroy(gameObject);
    }
}
