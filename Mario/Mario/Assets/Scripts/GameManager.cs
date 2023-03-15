using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public enum SpawnPoint { Left, Right }

    public struct SpawnData {
        public SpawnData(SpawnPoint _spawnPoint, float _timeInterval) {
            spawnPoint = _spawnPoint;
            timeInterval = _timeInterval;
        }
        public SpawnPoint spawnPoint;
        public float timeInterval;
    }

    public static GameManager instance;

    private int lifeCount = 4;

    public SpawnData[] spawnPlan = new SpawnData[] {
        new SpawnData(SpawnPoint.Right, 1.5f),
        new SpawnData(SpawnPoint.Left, 2f),
        new SpawnData(SpawnPoint.Right, 2.5f),
        new SpawnData(SpawnPoint.Left, 2f)
    };

    public GameObject turtlePrefab;
    public GameObject marioPrefab;

    public Vector3 leftSpawnPoint = new Vector3(-10f, 3.5f, 0f);
    public Vector3 rightSpawnPoint = new Vector3(10f, 3.5f, 0f);
    public Vector3 spawnPositionMario = new Vector3(0f, 4.5f, 0f);

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        if(turtlePrefab == null) {
            Debug.Log("GameManager. La variable turtlePrefab no está correctamente inicializada");
        } else if(marioPrefab == null) {
            Debug.Log("GameManager. La variable marioPrefab no está correctamente inicializada");
        } else  {

            StartCoroutine(SpawnCoroutine());
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    private IEnumerator SpawnCoroutine() {
        foreach(SpawnData spawnData in spawnPlan) {
            yield return new WaitForSeconds(spawnData.timeInterval);
            if(spawnData.spawnPoint == SpawnPoint.Left) {
                SpawnTurtle(leftSpawnPoint);
            } else {
                SpawnTurtle(rightSpawnPoint);
            }
        }
    }

    private void SpawnTurtle(Vector3 spawnPosition) {
        Instantiate(turtlePrefab, spawnPosition, Quaternion.identity);
    }

    private void SpawnMario() {
        GameObject marioGO = Instantiate(marioPrefab, spawnPositionMario, Quaternion.identity);
        marioGO.GetComponent<Mario>().StartInmunity();

        Camara camara = Camera.main.GetComponent<Camara>();
        camara.mario =  marioGO.transform;
    }

    public void MarioDead() {
        lifeCount--;
        if(lifeCount > 0) {
            Invoke("SpawnMario", 4f);
        }
    }
}




