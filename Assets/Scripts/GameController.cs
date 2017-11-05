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

[System.Serializable]
public class TargetVelocityRange
{
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
}

public class GameController : MonoBehaviour {

    public GameObject arrow;
    public TargetSpawnZone targetSpawnZone;
    public TargetVelocityRange targetVelocityRange;
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

        GameObject target = Instantiate(targetTemplate, new Vector3(xPos, yPos, targetTemplate.transform.position.z), this.transform.rotation);
        target.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(targetVelocityRange.xMin, targetVelocityRange.xMax), 
                                                                  Random.Range(targetVelocityRange.yMin, targetVelocityRange.yMax), 
                                                                  0);
    }
}
