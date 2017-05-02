using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IGameManager {
	public ManagerStatus status {get; private set;}

	public float currentHealth {get; set;}
	public float maxHealth {get; set;}

    public Slider healthBar;

	public void Startup() {
		Debug.Log("Player manager starting...");

		// any long-running startup tasks go here, and set status to 'Initializing' until those tasks are complete
		status = ManagerStatus.Started;
	}

    void Start() 
    {
        // these values could be initialized with saved data
        maxHealth = 100;

        currentHealth = maxHealth;

        healthBar.value = currentHealth / maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            ChangeHealth(-6);
    }

	public void ChangeHealth(float value) {
		currentHealth += value;
		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		} else if (currentHealth < 0) {
			currentHealth = 0f;
		}

        healthBar.value = (currentHealth / maxHealth);
	}
}
