using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetSpawnZone
{
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
}

public class GameController : MonoBehaviour {

    public GameObject arrow;
    public TargetSpawnZone targetSpawnZone;
    public GameObject targetTemplate;
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	}

    public void SpawnTarget()
    {
        float xPos = Random.Range(targetSpawnZone.xMin, targetSpawnZone.xMax);
        float yPos = Random.Range(targetSpawnZone.yMin, targetSpawnZone.yMax);

        Instantiate(targetTemplate, new Vector3(xPos, yPos, 0), this.transform.rotation);
        
    }
}
