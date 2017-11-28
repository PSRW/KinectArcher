using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
    public Text scoreText;
    public float windRange;

    private float windSpeed = 0;

    private int score = 0;
	
	void Start () {
        scoreText.text = "Score: " + score;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	}

    public void SpawnTarget()
    {
        score += 200;
        scoreText.text = "Score: " + score;
        float xPos = Random.Range(targetSpawnZone.xMin, targetSpawnZone.xMax);
        float yPos = Random.Range(targetSpawnZone.yMin, targetSpawnZone.yMax);

        GameObject target = Instantiate(targetTemplate, new Vector3(xPos, yPos, targetTemplate.transform.position.z), this.transform.rotation);
        target.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(targetVelocityRange.xMin, targetVelocityRange.xMax), 
                                                                  Random.Range(targetVelocityRange.yMin, targetVelocityRange.yMax), 
                                                                  0);
    }

    public void decreaseScore()
    {
        score -= 50;
        scoreText.text = "Score: " + score;
    }


    public float generateWind()
    {
        windSpeed =  Random.Range(-windRange, windRange);
        return windSpeed;
    }

    public float getWind()
    {
        return windSpeed;
    }
}
