using UnityEngine;
using System.Collections;

public class CollectAmmo : MonoBehaviour {

    // Player behaviour script reference
    private PlayerBehaviour PB;
    // Integer variables for the ammo type and amount
    private int in_ammoType = 0;
    private int in_ammoAmount;
    // Boolean for checking if the ammo has been collected
    private bool BL_AmmoCollected = false;
	// Use this for initialization
	void Start () {

        // Assigns the player behaviour script to the variable
        PB = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
        // Default amount of ammo to collect
        in_ammoAmount = 20;
	}
	
	// Update is called once per frame
	void Update () {

        CheckAmmoType();
	}

    void OnTriggerEnter(Collider coll)
    {
        // Checks if there is a trigger collision with the player game object
        if (coll.gameObject.name == "Player")
        {
            // Checks for the type of ammo using tags
            if (gameObject.tag == "ShotgunAmmo")
            {
                // Assigns a unique value to the ammo type
                in_ammoType = 1;                
            }
            else if (gameObject.tag == "RifleAmmo")
            {
                in_ammoType = 2;
            }
            else if (gameObject.tag == "GrenadeAmmo")
            {
                in_ammoType = 3;
            }

        }       
    }

    private void CheckAmmoType()
    {
        // Checks if the ammo has been collected
        if (!BL_AmmoCollected)
        {
            switch (in_ammoType)
            {
                case 1:
                    // Calls the ammo collection method
                    PB.AmmoCollection(in_ammoType, in_ammoAmount);
                    // Locks the method so that it is only called once
                    BL_AmmoCollected = true;
                    break;
                case 2:
                    // Calls the ammo collection method
                    PB.AmmoCollection(in_ammoType, in_ammoAmount);
                    // Locks the method so that it is only called once
                    BL_AmmoCollected = true;
                    break;
                case 3:
                    // Calls the ammo collection method
                    PB.AmmoCollection(in_ammoType, 1);
                    // Locks the method so that it is only called once
                    BL_AmmoCollected = true;
                    break;
            }            
        }    
        else
        {
            // Destroys the game object
            Destroy(gameObject);
        }

    }
}
