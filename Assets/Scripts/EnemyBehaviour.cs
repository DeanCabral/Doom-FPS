using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour {

    // Transform position of the player character
    private Transform PC;
    // Reference to Player Behaviour script
    private PlayerBehaviour PB;
    // Enemy weapon game object prefab
    public GameObject GO_Weapon;
    // Game objects for the bullet prefab and bullet spawn location
    public GameObject GO_bullet;
    public GameObject bulletSpawn;
    // Public materials that hold the coloured damage stages for the enemy
    public Material damageStage1;
    public Material damageStage2;
    public Material damageStage3;
    // Vector3 variable for player's last known location from a surprise attack
    private Vector3 lastKnownPosition;
    // Boolean variable for the type of enemy (static or patrol)
    public bool BL_Static = false;
    // Boolean variables that handle the logic for a surprise attack on a patrolling enemy
    public bool BL_Surprised = false;
    private bool BL_Attacked = false;    
    private bool BL_LastKnowPos = false;
    // Boolean for flagging if the enemy has left patrol to chase player
    public bool BL_SeenPlayer = false;
    // Boolean variable for checking if the enemy can engage in combat
    private bool BL_CanEngage = false;
    // Boolean for determining if the enemy is alive
    private bool BL_IsAlive = true;
    // Integer health variable
    public int in_enemyHealth = 100;
    // Public variables for the enemy move speed and min/max follow distance
    public int in_moveSpeed;
    public float fl_minFollow;
    public float fl_maxFollow;
    // Variables for the move direction and rotation of the enemy
    public float fl_moveRotation;
    public Vector3 moveDirection;    

    // Use this for initialization
    void Start () {

        // Retrieves the transform from the player game object
        PC = GameObject.Find("Player").GetComponent<CharacterController>().GetComponent<Transform>();
        // Retrieves the player behaviour script from the player game object
        PB = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
        // Checks for engaging in combat every 2 seconds
        InvokeRepeating("EngageCombat", 0, 2);
    }
	
	// Update is called once per frame
	void Update () {

        MovementAI();
        CheckDeath();
	}

    void OnCollisionEnter(Collision coll)
    {
        // Checks if the enemy collides against an enemy wall (patrol wall)
        if(coll.gameObject.tag == "Wall")
        {
            // Movement along the x axis
            if (moveDirection.x == 1 || moveDirection.x == -1)
            {
                // Switches the movement direction using a negative one multiplier
                moveDirection.x *= -1;
            }

            // Movement along the z axis
            if (moveDirection.z == 1 || moveDirection.z == -1)
            {
                // Switches the movement direction using a negative one multiplier
                moveDirection.z *= -1;
            }

            // Flips the transform's rotation to face the correct movement direction
            transform.rotation *= Quaternion.Euler(0, fl_moveRotation, 0);
        }
        // Checks if enemy collides with a player bullet
        else if(coll.gameObject.tag == "PlayerBullet")
        {
            // Decreases enemy health by 10
            in_enemyHealth -= 10;
            // Enemy has been surprise attacked
            if (!BL_SeenPlayer)
            {
                // Sets boolean to true
                BL_Surprised = true;               
            }

        }
    }

    private void MovementAI()
    {
        // Checks if the enemy is alive
        if (BL_IsAlive)
        {
            // Checks if enemy has been attacked without seeing the player
            if (BL_Surprised)
            {
                // Checks if the enemy does not know the player's last known position
                if (!BL_LastKnowPos)
                {
                    // Temporary transform from the players last known position
                    Transform generalDirection = PC.transform;
                    lastKnownPosition = generalDirection.position;
                    // Starts coroutine so that the enemy will approach the general direction with caution for a set amount of time
                    StartCoroutine("ApproachWithCaution");                
                    // Locks the if statement
                    BL_LastKnowPos = true;
                }
                else
                {                 
                    // Sets the enemy character and weapon to face the general direction from where they were attacked (excludes the y axis)
                    GO_Weapon.transform.LookAt(new Vector3(lastKnownPosition.x, transform.position.y, lastKnownPosition.z));
                    transform.LookAt(new Vector3(lastKnownPosition.x, transform.position.y, lastKnownPosition.z));
                    // Transforms the enemy position to the direction of the player's last known attack location
                    transform.position += transform.forward * (in_moveSpeed - 1) * Time.deltaTime;
                    // Checks if the enemy is in sight range of the player
                    if (Vector3.Distance(transform.position, PC.position) <= fl_maxFollow)
                    {
                        // Enemy has seen player and normal combat behaviour is resumed
                        BL_SeenPlayer = true;
                        BL_Surprised = false;
                    }
                }               
            }
            else
            {
                // Checks if the player is out of range from the enemy
                if (Vector3.Distance(transform.position, PC.position) >= fl_maxFollow)
                {
                    // Enemy can not engage
                    BL_CanEngage = false;
                    // Checks if the player has not been seen by the enemy
                    if (!BL_SeenPlayer)
                    {
                        // Checks if the enemy has been attacked previously
                        if (BL_Attacked)
                        {
                            // Enemy stands gaurd until aggitated by player
                            return;
                        }
                        else
                        {
                            // Checks if the enemy is static or if it patrols
                            if (BL_Static)
                            {
                                // Enemy stands gaurd until player is in range
                                return;
                            }
                            else
                            {
                                // Sets a predefined patrol for the enemy (movement speed is slowed to replicate a low alert status)
                                transform.position += moveDirection * Time.deltaTime * (in_moveSpeed - 1);
                            }                            
                        }                        
                    }
                }
                else if (Vector3.Distance(transform.position, PC.position) <= fl_maxFollow)
                {
                    // Enemy has seen player and has left their patrol
                    BL_SeenPlayer = true;
                    // Sets the enemy character and weapon to face the direction of the player transform
                    GO_Weapon.transform.LookAt(PC);
                    transform.LookAt(PC);

                    // Checks if the distance between the enemy position and player position is greater or equal to the minimum follow distance
                    if (Vector3.Distance(transform.position, PC.position) >= fl_minFollow)
                    {                        
                        // Transforms the enemy position forward (multiplied by the preset speed) after every frame
                        transform.position += transform.forward * in_moveSpeed * Time.deltaTime;

                        // Checks if the distance between the enemy position and player position is less than or equal to the maximum follow distance
                        if (Vector3.Distance(transform.position, PC.position) <= fl_maxFollow)
                        {
                            // Allows the enemy to engage the player
                            BL_CanEngage = true;
                        }
                    }
                }
            }            
        }        
    }

    private void EngageCombat()
    {
        // Checks if the enemy can engage the player
        if (BL_CanEngage)
        {
            // Temporary variables that hold the bullet spawn game object's position and rotation
            Vector3 spawnPos = bulletSpawn.transform.position;
            Quaternion spawnRot = bulletSpawn.transform.rotation;

            // Creates an enemy bullet prefab using a predefined location and rotation
            GameObject.Instantiate(GO_bullet, spawnPos, spawnRot);
        }  
    }

    private void CheckDeath()
    {
        // Checks if the enemy health is less than or equal to zero
        if (in_enemyHealth <= 0)
        {
            // Checks if the enemy is alive
            if (BL_IsAlive)
            {
                // Boolean and If statement are used as a lock so that the player's IncreaseLife function is only called once
                BL_IsAlive = false;
                // Increases players life
                PB.IncreaseLife();
            }            
            // Starts the destroy enemy IEnumerator
            StartCoroutine("DestroyEnemy");
            // Changes the colour of the enemy character to give visual feedback on damage input
            GetComponent<Renderer>().material = damageStage3;
        }
        else if(in_enemyHealth <= 50)
        {
            GetComponent<Renderer>().material = damageStage2;
        }
        else if (in_enemyHealth <= 75)
        {
            GetComponent<Renderer>().material = damageStage1;
        }
    }

    IEnumerator ApproachWithCaution()
    {
        // Approaches players last known location for 4 seconds
        yield return new WaitForSeconds(4);
        // Enemy is no longer surprised
        BL_Surprised = false;
        // Enemy no longer knows the player's last known position
        BL_LastKnowPos = false;
        // Boolean flagged to show that the enemy has been previously attacked
        BL_Attacked = true;
    }

    IEnumerator DestroyEnemy()
    {        
        // Destroys the enemy game object after one second
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }

}

