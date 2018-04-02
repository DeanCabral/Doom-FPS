using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

    // Character controller game object
    private CharacterController CC;
    // Game manager object
    private GameManager GM;
    // Vector that holds the direction the player moves along
    private Vector3 moveDirection = Vector3.zero;
    // Game objects that store the bullet prefab and bullet spawn location objects
    public GameObject GO_bullet;
    public GameObject bulletSpawn;
    // Game object for the grenade
    public GameObject GO_grenade;
    // Quaternion variable that stores the bullet's rotation
    public Quaternion bulletRotation;    
    // Float for setting the characters speed
    public float fl_movementSpeed = 5.0f;
    // Float for setting the sensitivity of looking around
    public float fl_mouseSensitivity = 5.0f;
    // Float variables for looking along the x and y axis
    private float fl_lookX = 0.0f;
    private float fl_lookY = 0.0f;
    // Float for storing the current x axis rotation of the camera
    private float fl_currentLookX;
    // Float for the height at which the player can jump
    private float fl_jumpHeight = 8f;
    // Boolean for enabling double jump
    private bool BL_doubleJump;
    // Float for the rate at which the player falls due to gravity
    private float fl_gravity = 20f;
    // Integer for storing the current weapon type    
    private int in_currentWeapon;   
    

    // Use this for initialization
    void Start () {

        // Retrieves character controller and game manager components
        CC = GetComponent<CharacterController>();
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Sets character controller height
        CC.height = 3.0f;
    }

    // Update is called once per frame
    void Update () {

        LookAround();
        Movement();
        UseWeapon();
        CheckDeath();         
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.name == "Vault")
        {
            GM.PlayerVictory();
        }
    }

    private void LookAround()
    {
        // Checks if the game is not paused and the player is not dead
        if (!GM.BL_Paused && !GM.BL_Dead)
        {
            // Stores the current location of the mouse's x axis
            fl_currentLookX = Input.GetAxis("Mouse X") * fl_mouseSensitivity;
            // Increments the x axis location from previous horizontal mouse movement (multiplied by sensitivity)
            fl_lookX += Input.GetAxis("Mouse X") * fl_mouseSensitivity;
            // Increments the y axis location from previous vertical mouse movement (multiplied by sensitivity)
            fl_lookY += Input.GetAxis("Mouse Y") * fl_mouseSensitivity;
            // Clamps the vertical movement to a range of -60 and 60 degrees
            fl_lookY = Mathf.Clamp(fl_lookY, -60, 60);

            // Rotates the character depeding on the position they are facing along the x axis
            transform.Rotate(0, fl_currentLookX, 0);
            // Sets the camera rotation variables using its eulerAngles (inverting the Y axis)
            Camera.main.transform.eulerAngles = new Vector3(-fl_lookY, fl_lookX, 0);

            // Locks the cursor to the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

    private void Movement()
    {
        // Checks if the game is not paused and the player is not dead
        if (!GM.BL_Paused && !GM.BL_Dead)
        {
            // Checks if the character controller is grounded against a collider
            if (CC.isGrounded)
            {
                // Float variables that hold the vertical and horizontal movement (yaw and pitch) multiplied by speed
                float vertical = Input.GetAxis("Vertical") * fl_movementSpeed;
                float horizontal = Input.GetAxis("Horizontal") * fl_movementSpeed;

                // Sets the double jump boolean to enabled
                BL_doubleJump = true;
                // Assigns the variables to their appropriate vector3 fields which will then be set to the move direction vector
                moveDirection = new Vector3(horizontal, 0, vertical);
                // Combines player movement and rotation
                moveDirection = transform.rotation * moveDirection;

                // Checks for jump input
                if (Input.GetKey(KeyCode.Space))
                {
                    // Sets the y position of the move direction vector equal to the jump height
                    moveDirection.y = fl_jumpHeight;                    
                }
            }
            else
            {
                // Checks for double jump mechanic through boolean and player input
                if (Input.GetKey(KeyCode.Space) && BL_doubleJump)
                {                    
                    moveDirection.y = fl_jumpHeight;
                    // Delays the boolean flag therefore only allowing the player to double jump during a certain time frame
                    Invoke("DisableDoubleJump", 0.5f);
                }
                else
                {
                    // Reduces the jump height by the gravity rate after each frame
                    moveDirection.y -= fl_gravity * Time.deltaTime;
                }
                
            }

            // Moves the character using the Move() method in the character controller (multiplied by delta time)
            CC.Move(moveDirection * Time.deltaTime);
        }        
    }

    private void DisableDoubleJump()
    {
        // Flags the double jump boolean to false
        BL_doubleJump = false;
    }
   
    private void UseWeapon()
    {
        // Checks if the game is not paused and the player is not dead
        if (!GM.BL_Paused && !GM.BL_Dead)
        {
            // Checks if player has left mouse clicked
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                // Temporary variables that hold the bullet spawn's position and rotation
                Vector3 spawnPos = bulletSpawn.transform.position;
                Quaternion spawnRot = bulletSpawn.transform.rotation;

                // Switch statement that handles the bullet spawning of different weapon types
                switch (in_currentWeapon)
                {
                    case 1:
                        // Single shot bullet
                        GameObject.Instantiate(GO_bullet, spawnPos, spawnRot);
                        break;
                    case 2:
                        if (GM.in_weapon2ammo - 3 >= 0)
                        {
                            // Triple shot bullets (via a horizontal offset)         
                            GameObject.Instantiate(GO_bullet, spawnPos, spawnRot);
                            GameObject.Instantiate(GO_bullet, spawnPos + (transform.right * 0.4f), spawnRot);
                            GameObject.Instantiate(GO_bullet, spawnPos + (transform.right * 0.8f), spawnRot);
                            // Reduces ammo count
                            GM.in_weapon2ammo -= 3;
                        }
                        else
                        {
                            // Sets ammo to zero therefore simulating an empty ammo clip
                            GM.in_weapon2ammo = 0;
                        }
                        break;
                    case 3:
                        if (GM.in_weapon3ammo - 4 >= 0)
                        {
                            // Quadruple shot bullets (via a forward facing offset)
                            GameObject.Instantiate(GO_bullet, spawnPos, spawnRot);
                            GameObject.Instantiate(GO_bullet, spawnPos + (Camera.main.transform.forward * 1.2f), spawnRot);
                            GameObject.Instantiate(GO_bullet, spawnPos + (Camera.main.transform.forward * 1.6f), spawnRot);
                            GameObject.Instantiate(GO_bullet, spawnPos + (Camera.main.transform.forward * 2.0f), spawnRot);
                            // Reduces ammo count
                            GM.in_weapon3ammo -= 4;
                        }
                        else
                        {
                            // Sets ammo to zero therefore simulating an empty ammo clip
                            GM.in_weapon3ammo = 0;
                        }
                        break;
                }
            }
            else if(Input.GetKeyDown(KeyCode.G))
            {
                // Temporary variables that hold the grenade spawn's position and rotation
                Vector3 spawnPos = bulletSpawn.transform.position;
                Quaternion spawnRot = bulletSpawn.transform.rotation;

                if (GM.in_grenades - 1 >= 0)
                {
                    // Single grenade throw using a height offset
                    GameObject.Instantiate(GO_grenade, new Vector3(spawnPos.x, spawnPos.y + 1, spawnPos.z), spawnRot);
                    // Reduces grenade count
                    GM.in_grenades -= 1;
                }              
            }
        }        
    }

    private void CheckDeath()
    {
        // Checks if the player's health bar has reached zero
        if (GM.healthBar.value <= 0)
        {
            // Checks if the player is not dead
            if (!GM.BL_Dead)
            {
                // Sets boolean to true to indicate that the player has died
                GM.BL_Dead = true;
                // Calls the player death function in the game manager
                GM.PlayerDeath();
            }
            
        }
    }

    public void SetWeapon(int weaponType)
    {
        // Takes a weapon type parameter and sets the local weapon type accordingly
        in_currentWeapon = weaponType;
    }    

    public void AmmoCollection(int ammoType, int ammoAmount)
    {
        // Temporary string message
        string st_message;

        // Switch statement that goes through the ammo type collected
        switch (ammoType)
        {
            case 1:
                // Sets custom message depeding on the ammo type collected
                st_message = "Collected +" + ammoAmount + " Shotgun rounds";
                // Sends message and delay time as parameters to display message function
                GM.DisplayMessage(st_message, 2);
                // Increments ammo count
                GM.in_weapon2ammo += ammoAmount;
                break;
            case 2:
                st_message = "Collected +" + ammoAmount + " Rifle rounds";
                GM.DisplayMessage(st_message, 2);
                GM.in_weapon3ammo += ammoAmount;
                break;
            case 3:
                st_message = "Collected +" + ammoAmount + " Grenade";
                GM.DisplayMessage(st_message, 2);
                GM.in_grenades += ammoAmount;
                break;
        }
       
    }

    public void DecreaseLife()
    {
        // Ensures the player's health bar value does not go below zero
        if (GM.healthBar.value >= 0)
        {
            // If statements that sequentially go through the players armour bars and finally to their health
            if (GM.armourBar3.value == 1 || GM.armourBar3.value == 0.5f)
            {
                // Sets value to zero
                GM.armourBar3.value = 0;
            }
            else if(GM.armourBar2.value == 1 || GM.armourBar2.value == 0.5f)
            {
                GM.armourBar2.value = 0;
            }
            else if (GM.armourBar1.value == 1 || GM.armourBar1.value == 0.5f)
            {
                GM.armourBar1.value = 0;
            }
            else
            {
                // Decreases health if all armour bars are depleted
                GM.healthBar.value -= 0.1f;
            }
        }

        GM.PlayerDamage();
    }

    public void IncreaseLife()
    {
        // Ensures the player's health bar value does not go above one
        if (GM.healthBar.value <= 1)
        {
            // If statements that sequentially go through the players armour bars as well as health 
            if (GM.healthBar.value < 1)
            {
                // Increases health if all armour bars are depleted
                GM.healthBar.value += 0.1f;
            }           
            else if (GM.armourBar1.value < 1)
            {
                // Increases value by half if value is more than or equal to zero
                if (GM.armourBar1.value >= 0)
                {
                    GM.armourBar1.value += 0.5f;
                }                
            }
            else if (GM.armourBar2.value < 1)
            {
                if (GM.armourBar2.value >= 0)
                {
                    GM.armourBar2.value += 0.5f;
                }
            }
            else if (GM.armourBar3.value < 1)
            {
                if (GM.armourBar3.value >= 0)
                {
                    GM.armourBar3.value += 0.5f;
                }
            }
        }
    }
}
