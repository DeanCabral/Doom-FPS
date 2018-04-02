using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // Public game objects for the pause screen and damage overlay reference
    public GameObject pauseScreen;
    public GameObject damageOverlay;
    // Private text objects for the win and death screen references
    private Text winScreen;
    private Text deathScreen;
    // SwitchWeapon script variable
    private SwitchWeapon SW;
    // Text variables for displaying weapon info on the UI
    private Text weaponText;
    public Text weaponAmmo;    
    // Text and string variable for storing general info on the UI
    private Text generalInfo;
    public string st_info = "";
    // Text and integer variables for grenade count
    private Text grenadeCount;
    public int in_grenades;
    // Text variable for the FPS counter
    private Text FPScount;
    // Sliders for handling player health and armour
    public Slider armourBar1;
    public Slider armourBar2;
    public Slider armourBar3;
    public Slider healthBar;    
    // A string and integer variable(s) that store the raw data of weapon ammo
    private string st_weapon1ammo = "Unlimited";
    public int in_weapon2ammo;
    public int in_weapon3ammo;
    // Floats that store the logic for frames per second values
    private float fl_frameCount = 0;
    private float fl_nextUpdate = 0;
    private float fl_fps = 0;
    // Boolean that determines the players death status
    public bool BL_Dead = false;
    // Boolean variable for paused state
    public bool BL_Paused;
    // Float that holds the lerp time for the death screen
    private float lerpTime = 0;

    // Use this for initialization
    void Start () {

        // Assigns all game objects in the scene heirachy to their created variables
        SW = GameObject.Find("Player").GetComponent<SwitchWeapon>();
        generalInfo = GameObject.Find("GeneralInfo").GetComponent<Text>();
        weaponText = GameObject.Find("WeaponText").GetComponent<Text>();
        grenadeCount = GameObject.Find("GrenadeText").GetComponent<Text>();
        FPScount = GameObject.Find("FPSText").GetComponent<Text>();
        winScreen = GameObject.Find("VictoryMenu").GetComponent<Text>();
        deathScreen = GameObject.Find("DeathMenu").GetComponent<Text>();

        // Used as a starting time reference
        fl_nextUpdate = Time.time;

        // Sets the default UI and game screen state
        pauseScreen.gameObject.SetActive(false);
        damageOverlay.SetActive(false);
        QualitySettings.vSyncCount = 1;
        BL_Paused = true;
        TogglePause();

        // Default weapon ammo
        in_weapon2ammo = 50;
        in_weapon3ammo = 80;
        in_grenades = 3;
    }
	
	// Update is called once per frame
	void Update () {

        CheckPause();        
        UpdateUI();
	}

    private void UpdateUI()
    {
        // Switch statement for cycling through weapon types and displaying their ammo count
        switch (SW.in_selectedWeapon)
        {
            case 1:
                // Unlimited ammo for the standard pistol
                weaponText.text = "Standard Pistol";
                weaponAmmo.text = st_weapon1ammo;
                break;
            case 2:
                weaponText.text = "Combat Shotgun";
                // Checks for an empty magazine before displaying string
                if (in_weapon2ammo == 0)
                {
                    weaponAmmo.text = "No ammo";
                }
                else
                {
                    // Displays the number of bullets remaining
                    weaponAmmo.text = in_weapon2ammo.ToString() + " bullets";
                }                
                break;
            case 3:
                weaponText.text = "Assault Rifle";
                // Checks for an empty magazine before displaying string
                if (in_weapon3ammo == 0)
                {
                    weaponAmmo.text = "No ammo";
                }
                else
                {
                    // Displays the number of bullets remaining
                    weaponAmmo.text = in_weapon3ammo.ToString() + " bullets";
                }                
                break;
        }

        // Updates the grenade count
        grenadeCount.text = in_grenades + "x GRENADES";
        // Updates the general information text
        generalInfo.text = st_info;
        // Displays the frames per second in the pause screen
        DisplayFPS();        
        
    }

    private void DisplayFPS()
    {
        // Incremenents frame counter
        fl_frameCount++;
        // Checks if unscaled time is greater than the initial next update value set at the start (unscaled time is used as it is not affected by the frozen timescale during pause)
        if (Time.unscaledTime > fl_nextUpdate)
        {
            // The next update is equal to itself plus 1 divided by 4 (the update rate is hardcoded at 4 a second)
            fl_nextUpdate += 1.0f / 4;
            // The fps float is equal to the frame counter multiplied by the update rate
            fl_fps = fl_frameCount * 4;
            // The frame count is then set to 0
            fl_frameCount = 0;
        }

        // Assigns the constantly updating frames per second float to a text game object
        FPScount.text = fl_fps.ToString() + " FPS";
    }

    private void CheckPause()
    {
        // Constant input check for pausing the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Game can only be paused if the player is not dead
            if (!BL_Dead)
            {
                // Toggles the pause state
                TogglePause();
            }            
        }
    }

    private void TogglePause()
    {
        // Switches pause boolean whenever method is called
        BL_Paused = !BL_Paused;

        // Checks if the game is paused
        if (BL_Paused)
        {
            // Freezes the timescale (disables time)
            Time.timeScale = 0;
            // Removes the cursor lock and displays the pause screen game object
            Cursor.lockState = CursorLockMode.None;
            pauseScreen.gameObject.SetActive(true);
        }
        else
        {
            // Resumes the timescale (enables time)
            Time.timeScale = 1;
            // Locks the cursor in the center of the screen and hides the pause screen
            Cursor.lockState = CursorLockMode.Locked;
            pauseScreen.gameObject.SetActive(false);
        }
    }

    public void DisplayMessage(string message, float delay)
    {
        // Displays the message through a coroutine
        StartCoroutine(ShowMessage(message, delay));
    }

    public void PlayerDamage()
    {
        StartCoroutine("DamagePlayer");
    }

    public void PlayerVictory()
    {
        // Kills player movement
        BL_Dead = true;
        // Removes the cursor lock and displays the win screen game object
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine("ShowWinScreen");
    }

    public void PlayerDeath()
    {        
        // Removes the cursor lock and displays the death screen game object
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine("ShowDeathScreen");        
    }    

    public void ResumeGame()
    {
        // UI button that handles resuming the game in the pause state
        TogglePause();
    }

    public void RestartGame()
    {
        // UI button that handles reloading of the game
        SceneManager.LoadScene("Scene");
    }

    public void ExitGame()
    {
        // UI button that handles game exit
        Application.Quit();
    }

    public void OnValueChanged(int val)
    {
        // Switches VSync index whenever dropdown menu is called
        switch (val)
        {
            case 0:
                // Enables Vsync
                QualitySettings.vSyncCount = 1;
                break;
            case 1:
                // Disables Vsync
                QualitySettings.vSyncCount = 0;
                break;
        }
    }

    IEnumerator ShowMessage(string message, float delay)
    {
        // Sets the message 
        st_info = message;
        // Waits for a specified delay time 
        yield return new WaitForSeconds(delay);
        // Hides the message
        st_info = "";
    }

    IEnumerator DamagePlayer()
    {
        // Shows the damage overlay
        damageOverlay.SetActive(true);
        // Waits for a specified delay time 
        yield return new WaitForSeconds(0.2f);
        // Hides the damage overaly
        damageOverlay.SetActive(false);
    }

    IEnumerator ShowWinScreen()
    {
        // While lerptime is less than one
        while (lerpTime < 1)
        {
            // Increment lerp time by delta time multiplied by 2 (twice as fast)
            lerpTime += Time.deltaTime * 2;
            // Lerp the win screen text object onto the UI screen
            winScreen.GetComponent<RectTransform>().localPosition = Vector3.Lerp(new Vector3(-199, -357, 0), new Vector3(-199, 76, 0), lerpTime);

            yield return null;
        }
        // Checks if lerp time has reached 1
        if (lerpTime >= 1)
        {
            // Freezes the timescale (disables time)
            Time.timeScale = 0;
        }
    }

    IEnumerator ShowDeathScreen()
    {
        // While lerptime is less than one
        while (lerpTime < 1)
        {
            // Increment lerp time by delta time multiplied by 2 (twice as fast)
            lerpTime += Time.deltaTime * 2;
            // Lerp the death screen text object onto the UI screen
            deathScreen.GetComponent<RectTransform>().localPosition = Vector3.Lerp(new Vector3(-199, -199, 0), new Vector3(-199, 76, 0), lerpTime);

            yield return null;
        }
        // Checks if lerp time has reached 1
        if (lerpTime >= 1)
        {
            // Shows the damage overlay
            damageOverlay.SetActive(true);
            // Freezes the timescale (disables time)
            Time.timeScale = 0;
        }
    }
}
