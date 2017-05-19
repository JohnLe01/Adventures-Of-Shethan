using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {
    public int enemyCount = 5;
	[SerializeField] private GameObject[] enemyPrefab;
    public GameObject[] _enemy;

    void Update() {
        for (int i = 0; i < enemyCount; i++) {
            if (_enemy[i] == null) {
                _enemy[i] = Instantiate (enemyPrefab[i]) as GameObject;
                float xValue = Random.Range (95, 250);
                float zValue = Random.Range(110, 250);
                _enemy[i].transform.position = new Vector3 (xValue, 60, zValue);
                float angle = Random.Range (0, 360);
                _enemy[i].transform.Rotate (0, angle, 0);
            }
        }
    }
}
