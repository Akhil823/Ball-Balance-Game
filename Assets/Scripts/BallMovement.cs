using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class BallMovement : MonoBehaviour
{
    private Rigidbody ballRb;
    private GameObject refObject, player;
    public Vector3 forceDirection, startPos;
    private float forceVal = 2.0f, upwardForce = 7.5f;
    private float forwardInput,horizontalInput;
    public bool inAir = false;
    public AudioClip jumpSound; 
    private AudioSource playerAudio;
    private FollowPlayer _followPlayerScript;
    private CoinsAndPowerups _coinsAndPowerupsScript;
    public ParticleSystem powerupParticle;
    public float yPos = 0.0f;
    public Button jumpButton;
    private int target;
    public FixedJoystick moveJoystick;
    void Start()
    {
        refObject = GameObject.Find("ReferenceGameObject");
        player = GameObject.Find("Player");
        ballRb = GameObject.Find("Ball").GetComponent<Rigidbody>();
        powerupParticle = GameObject.Find("Powerup Particle").GetComponent<ParticleSystem>();
        playerAudio = GetComponent<AudioSource>();
        _followPlayerScript = GameObject.Find("Player").GetComponent<FollowPlayer>();
        _coinsAndPowerupsScript = GameObject.Find("Ball").GetComponent<CoinsAndPowerups>();
        powerupParticle.Stop();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < _coinsAndPowerupsScript.xLeft)
            transform.position = new Vector3(_coinsAndPowerupsScript.xLeft, transform.position.y, transform.position.z);
        if (transform.position.x > _coinsAndPowerupsScript.xRight)
            transform.position = new Vector3(_coinsAndPowerupsScript.xRight, transform.position.y, transform.position.z);
        if (transform.position.z < _coinsAndPowerupsScript.zBack)
            transform.position = new Vector3(transform.position.x, transform.position.y, _coinsAndPowerupsScript.zBack);
        if (transform.position.z > _coinsAndPowerupsScript.zFront)
            transform.position = new Vector3(transform.position.x, transform.position.y, _coinsAndPowerupsScript.zFront);

        
        if(_coinsAndPowerupsScript.hasJumpPowerup)
            transform.position = new Vector3(transform.position.x,yPos,transform.position.z);
        
        if(! _followPlayerScript.gameOver && !_followPlayerScript.pauseFlag)
        {
            forceDirection = (transform.position - refObject.transform.position).normalized;
            
            if(Input.GetAxis("Vertical") != 0)
                forwardInput = Input.GetAxis("Vertical");
            else
                forwardInput = moveJoystick.Vertical;

            if(Input.GetAxis("Horizontal") != 0)
                horizontalInput = Input.GetAxis("Horizontal");
            else
                horizontalInput = moveJoystick.Horizontal;

            
            ballRb.AddForce(forceDirection * forwardInput * forceVal );
            ballRb.AddForce(Quaternion.AngleAxis(90,Vector3.up) * forceDirection * horizontalInput * forceVal);
            jumpButton.onClick.AddListener(MakeBallJump);
            
            if (Input.GetKeyDown(KeyCode.Space))
                MakeBallJump();
                
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        inAir = false;
        if (collision.gameObject.CompareTag("Game Item"))
            _coinsAndPowerupsScript.jumpPoweupFlag = false;
        if (collision.gameObject.CompareTag("Ground") && !_coinsAndPowerupsScript.jumpPoweupFlag)
            _followPlayerScript.gameOver = true;
    }


    public void MakeBallJump()
    {
        if (!inAir && !_followPlayerScript.gameOver && !_followPlayerScript.pauseFlag)
        {   
            ballRb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 0.35f);
            inAir = true;
            _coinsAndPowerupsScript.jumpPoweupFlag = false;
        }
    }

} 
