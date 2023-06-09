using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinsAndPowerups : MonoBehaviour
{
    public List<GameObject> powerups;
    public List<GameObject> coins;
    public bool isPowerup = false, hasPowerup=false, hasJumpPowerup = false, jumpPoweupFlag = false;
    private float startTime = 1.0f, spawnTime = 2.0f;
    private FollowPlayer _followPlayerScript;
    private BallMovement _ballMovementScript;
    private int coinCount = 0;
    private AudioSource specialAudio;
    public AudioClip coinSound, timeSound, flySound;
    private HashSet<Vector3> positions = new HashSet<Vector3>();
    
    public float xLeft,xRight,zFront,zBack,yPos;
    // Start is called before the first frame update
    void Start()
    {
        _followPlayerScript = GameObject.Find("Player").GetComponent<FollowPlayer>();
        _ballMovementScript = GameObject.Find("Ball").GetComponent<BallMovement>();
        specialAudio = GetComponent<AudioSource>();
        InvokeRepeating("SpawnCoins",startTime,spawnTime);
        InvokeRepeating("SpawnPowerups",startTime,spawnTime);
        xLeft = GameObject.Find("Bottom Left Corner").transform.position.x;
        xRight = GameObject.Find("Bottom Right Corner").transform.position.x;
        zBack = GameObject.Find("Bottom Left Corner").transform.position.z;
        zFront = GameObject.Find("Upper Left Corner").transform.position.z;
        yPos = 1.25f;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void SpawnCoins()
    {
        if (coinCount < 5 && !_followPlayerScript.gameOver && !_followPlayerScript.pauseFlag)
        {
            int ind = Random.Range(0, coins.Count);
            Vector3 spawnPos = new Vector3(Random.Range(xLeft, xRight), yPos, Random.Range(zBack, zFront));
            if(!positions.Contains(spawnPos))
            {
                Instantiate(coins[ind], spawnPos, coins[ind].transform.rotation);
                coinCount++;
                positions.Add(spawnPos);
            }
        }
        
    }

    void SpawnPowerups()
    { 
        if(!isPowerup && !hasPowerup && !_followPlayerScript.gameOver && !_followPlayerScript.pauseFlag)
        {
            int ind = Random.Range(0, powerups.Count);
            Vector3 spawnPos = new Vector3(Random.Range(xLeft, xRight), yPos, Random.Range(zBack, zFront));
            if (!positions.Contains(spawnPos))
            {
                Instantiate(powerups[ind], spawnPos, powerups[ind].transform.rotation);
                isPowerup = true;
                positions.Add(spawnPos);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Coins"))
        {
            coinCount--;
            positions.Remove(collision.gameObject.transform.position);
            specialAudio.PlayOneShot(coinSound);
            if (collision.gameObject.name == "GoldCoin(Clone)")
                _followPlayerScript.score += 10;
            else if (collision.gameObject.name == "SilverCoin(Clone)")
                _followPlayerScript.score += 5;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Powerup"))
        {
            isPowerup = false;
            hasPowerup = true;
            positions.Remove(collision.gameObject.transform.position);
            if (collision.gameObject.name == "Jump(Clone)")
            {
                _ballMovementScript.yPos = yPos;
                hasJumpPowerup = true;
                jumpPoweupFlag = true;
                specialAudio.PlayOneShot(flySound);
                _ballMovementScript.powerupParticle.Play();
            }
            else if (collision.gameObject.name == "Time(Clone)")
            {
                _followPlayerScript.time += 30;
                specialAudio.PlayOneShot(timeSound);
            }
            
            Destroy(collision.gameObject);
            StartCoroutine("RemovePowerup");
        }
    }

    IEnumerator RemovePowerup()
    {
        yield return new WaitForSeconds(15);
        _ballMovementScript.powerupParticle.Stop();
        hasPowerup = false;
        hasJumpPowerup = false;
    }
}
