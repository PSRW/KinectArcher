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

    public GameObject windArrow;
    public GameObject arrow;
    public TargetSpawnZone targetSpawnZone;
    public TargetVelocityRange targetVelocityRange;
    public GameObject targetTemplate;
    public Text scoreText;
    public Text windText;
    public float windRange = 3;
    private bool isWindArrowRotated;

    public static float windSpeed = 0;
    private int score = 0;
	
	void Start () {
        scoreText.text = "Score: " + score;
        generateWind();
        updateWind();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	}

    public void SpawnTarget()
    {
        increaseScore(200);
        generateWind();
        updateWind();

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

    public void increaseScore(int increseScoreValue)
    {
        score += increseScoreValue;
        scoreText.text = "Score: " + score;
    }

    private void updateWind()
    {
        if (isWindArrowRotated)
        {
            windArrow.transform.Rotate(0, 0, -180);
            isWindArrowRotated = false;
        }

        if(windSpeed < 0)
        {
            windArrow.transform.Rotate(0, 0, 180);
            isWindArrowRotated = true;
        }

        float windSpeedWithoutSign = Mathf.Abs(windSpeed);
        windText.text = windSpeedWithoutSign.ToString("n1");
    }

    private float generateWind()
    {
        windRange = 3;
        windSpeed =  Random.Range(-windRange, windRange);
        return windSpeed;
    }

    public float getWind()
    {
        return windSpeed;
    }
}
