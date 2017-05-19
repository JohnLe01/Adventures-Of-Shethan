using UnityEngine;
using System.Collections;

public class WanderingAI : MonoBehaviour {
	public const float baseSpeed = 3.0f;

	public float speed = 3.0f;
	public float obstacleRange = 5.0f;
		
	private bool _alive;

    private Animator animator;

	void Awake() {
		Messenger<float>.AddListener(GameEvent.SPEED_CHANGED, OnSpeedChanged);
	}
	void OnDestroy() {
		Messenger<float>.RemoveListener(GameEvent.SPEED_CHANGED, OnSpeedChanged);
	}

	void Start() {
		_alive = true;
        animator = GetComponent<Animator>();
	}
	
	void Update() {
		if (_alive) {
            animator.SetBool("Hop", true);
			transform.Translate(0, 0, speed * Time.deltaTime);
			
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hit;
			if (Physics.SphereCast(ray, 0.75f, out hit)) {
                if (hit.distance < obstacleRange || this.transform.position.x <= 80.0f) {
					float angle = Random.Range(-180, 180);
					transform.Rotate(0, angle, 0);
				}
			}
		}
	}

	public void SetAlive(bool alive) {
		_alive = alive;
	}

    public bool IsAlive() {
        return _alive;
    }

	private void OnSpeedChanged(float value) {
		speed = baseSpeed * value;
	}
}
