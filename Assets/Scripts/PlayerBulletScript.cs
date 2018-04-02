using UnityEngine;
using System.Collections;

public class PlayerBulletScript : MonoBehaviour {

    // SwitchWeapon reference
    private SwitchWeapon SW;
    public float fl_bulletSpeed = 2000f;

	// Use this for initialization
	void Start () {

        // Assigns reference to SwitchWeapon script child object
        SW = GameObject.Find("Player").GetComponent<SwitchWeapon>();
        // Checks for the type of weapon in use
        WeaponCheck();        
    }	

    void OnCollisionEnter(Collision coll)
    {
        // Calls the Destroy Bullet method on collision with another game object
        DestroyBullet();
    }

    private void WeaponCheck()
    {
        // Switch statement for the weapon in use
        switch (SW.in_selectedWeapon)
        {
            case 1:
                // Adds force at the forward transform of weapon 1
                this.GetComponent<Rigidbody>().AddForce(SW.GO_Weapon1.transform.forward * fl_bulletSpeed);
                break;
            case 2:
                // Adds force at the forward transform of weapon 2
                this.GetComponent<Rigidbody>().AddForce(SW.GO_Weapon2.transform.forward * fl_bulletSpeed);
                break;
            case 3:
                // Adds force at the forward transform of weapon 3
                this.GetComponent<Rigidbody>().AddForce(SW.GO_Weapon3.transform.forward * fl_bulletSpeed);
                break;
            default:
                // Adds force as a default forward transform
                this.GetComponent<Rigidbody>().AddForce(transform.forward * fl_bulletSpeed);
                break;

        }

        // Calls the Destroy Bullet method after 2 seconds
        Invoke("DestroyBullet", 2f);
    }

    private void DestroyBullet()
    {
        // Destroys the game object
        Destroy(gameObject);
    }
}
