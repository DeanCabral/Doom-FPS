using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoors : MonoBehaviour {

    // Game manager object
    private GameManager GM;
    // Game object for vault door
    private GameObject GO_Vault;
    // Game objects that hold both left and right doors
    public GameObject GO_doorL;
    public GameObject GO_doorR;
    // Boolean for checking the open state of the doors
    private bool BL_Open = false;
    private bool BL_CanUnlock = false;
    // The duration of the opening doors
    private float fl_duration = 0.5f;

    void Start()
    {
        // Retrieves game manager component
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // Checks if the player can unlock the vault
        if (BL_CanUnlock)
        {
            // Calls vault unlock method
            UnlockVault();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        // Only works for the player character
        if (coll.tag == "Player")
        {
            // Checks if the player is able to activate the vault door
            if (this.gameObject.name == "VaultButton")
            {
                // Displays hint message if player is inside trigger area
                GM.st_info = "Press 'X' to unlock Vault";
                // Player is able to unlock the vault
                BL_CanUnlock = true;
            }
            else
            {
                // Checks if the door is closed
                if (!BL_Open)
                {
                    // Opens the doors through coroutine
                    StartCoroutine(OpenDoors());
                }
            }            
        }
           
    }

    void OnTriggerExit(Collider coll)
    {
        // Only works for the player character
        if (coll.tag == "Player")
        {
            // Checks if the player is not able to activate the vault door
            if (this.gameObject.name == "VaultButton")
            {
                // Clears the hint message
                GM.st_info = "";
                // Player is no longer able to unlock the vault
                BL_CanUnlock = false;                
            }
            else
            {
                // Checks if the door is open
                if (BL_Open)
                {
                    // Closes the doors through coroutine
                    StartCoroutine(CloseDoors());
                }
            }
            
        }            
    }

    private void UnlockVault()
    {
        // Checks for the 'X' key input from player
        if (Input.GetKey(KeyCode.X))
        {
            // Locates the vault door in the scene
            GO_Vault = GameObject.Find("VaultDoor");

            // Checks if the door is not null (activated for the first time)
            if (GO_Vault != null)
            {
                // Hides the vault door and gives the user text feedback
                GO_Vault.SetActive(false);
                GM.DisplayMessage("Unlocked Vault Door", 2f);
            }
            else
            {
                // Player has previously opened the vault and so message is displayed
                GM.DisplayMessage("Vault Already Unlocked", 2f);
            }           
            
            // Set to false to prevent being activated again without leaving the trigger area
            BL_CanUnlock = false;
        }
    }


    IEnumerator OpenDoors()
    {
        // Amount of time elapsed
        float fl_timeElapsed = 0;
        // Sets the state boolean to true
        BL_Open = true;

        // While the time elasped is less than one
        while (fl_timeElapsed < 1)
        {
            // Transforms the position of both doors individually after each frame
            GO_doorL.transform.position += -transform.right * 4 * Time.deltaTime;
            GO_doorR.transform.position += transform.right * 4 * Time.deltaTime;

            // Increments the time elapsed
            fl_timeElapsed += Time.deltaTime / fl_duration;
            yield return null;
        }

        if (fl_timeElapsed == 1)
        {            
            // Stops the coroutine when the time has reached its target
            StopCoroutine(OpenDoors());
        }
    }

    IEnumerator CloseDoors()
    {
        // Amount of time elapsed
        float fl_timeElapsed = 0;
        // Sets the state boolean to false
        BL_Open = false;

        // Delay for door close
        yield return new WaitForSeconds(1);

        // While the time elasped is less than one
        while (fl_timeElapsed < 1)
        {
            // Transforms the position of both doors individually after each frame
            GO_doorL.transform.position += transform.right * 4 * Time.deltaTime;
            GO_doorR.transform.position += -transform.right * 4 * Time.deltaTime;

            // Increments the time elapsed
            fl_timeElapsed += Time.deltaTime / fl_duration;
            yield return null;
        }

        if (fl_timeElapsed == 1)
        {
            // Stops the coroutine when the time has reached its target
            StopCoroutine(CloseDoors());
        }
    }
}
