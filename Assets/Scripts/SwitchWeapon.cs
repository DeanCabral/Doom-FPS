using UnityEngine;
using System.Collections;

public class SwitchWeapon : MonoBehaviour {

    // PlayerBehaviour script reference
    private PlayerBehaviour PB;
    // Integer variable for the currently held weapon type
    public int in_selectedWeapon;
    // Game Objects that hold the different weapon objects
    public GameObject GO_Weapon1;
    public GameObject GO_Weapon2;
    public GameObject GO_Weapon3;

	// Use this for initialization
	void Start () {

        // Assigns reference to PlayerBehaviour script child object
        PB = GetComponent<PlayerBehaviour>();
        // Defaults the selected weapon to the first weapon
        in_selectedWeapon = 1;
        ShowWeapon();
        PB.SetWeapon(1);
    }
	
	// Update is called once per frame
	void Update () {

        SwapWeapon();
	}

    private void SwapWeapon()
    {
        // Input checks which handle weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Selected weapon is 1
            in_selectedWeapon = 1;
            ShowWeapon();            
            PB.SetWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Selected weapon is 2
            in_selectedWeapon = 2;
            ShowWeapon();
            PB.SetWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Selected weapon is 3
            in_selectedWeapon = 3;
            ShowWeapon();
            PB.SetWeapon(3);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            // Positively switches the weapons
            ToggleSwitch(1);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            // Negatively switches the weapons
            ToggleSwitch(-1);
        }
    }

    private void ToggleSwitch(int type)
    {
        // Checks the type of toggle (positivve or negative)
        if (type == 1)
        {
            // Increases selected weapon type if less than 3
            if (in_selectedWeapon < 3)
            {
                in_selectedWeapon++;
            }
            else
            {
                // Otherwise sets selected weapon to 1
                in_selectedWeapon = 1;
            }
            // Dislays and configures set weapon
            ShowWeapon();
            PB.SetWeapon(in_selectedWeapon);
        }
        else
        {
            // Decreases selected weapon type if more than 1
            if (in_selectedWeapon > 1)
            {
                in_selectedWeapon--;
            }
            else
            {
                // Otherwise sets selected weapon to 3
                in_selectedWeapon = 3;
            }
            // Dislays and configures set weapon
            ShowWeapon();
            PB.SetWeapon(in_selectedWeapon);
        }
    }

    private void ShowWeapon()
    {
        // Switch statement to display the selected weapon
        switch (in_selectedWeapon)
        {
            case 1:
                // Enables weapon 1 game object and disbales the others
                GO_Weapon1.gameObject.SetActive(true);
                GO_Weapon2.gameObject.SetActive(false);
                GO_Weapon3.gameObject.SetActive(false);
                break;
            case 2:
                // Enables weapon 2 game object and disbales the others
                GO_Weapon1.gameObject.SetActive(false);
                GO_Weapon2.gameObject.SetActive(true);
                GO_Weapon3.gameObject.SetActive(false);
                break;
            case 3:
                // Enables weapon 3 game object and disbales the others
                GO_Weapon1.gameObject.SetActive(false);
                GO_Weapon2.gameObject.SetActive(false);
                GO_Weapon3.gameObject.SetActive(true);
                break;
        }
    }

}
