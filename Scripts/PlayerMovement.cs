using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Collision _collision;
    private SpriteController _spriteController;


[Header("Horizontal Monvement")]
    [SerializeField] private float _maxSpeed  = 6;
    [SerializeField] private float _speed = 6;
    [SerializeField] private float _acceleration = 3;
    [SerializeField] private float _stoppingSpeed = 5;
    [SerializeField] private float _airAcceleration = 1.5f;
    private float _lastHorizontalInput = 0;
[Header("Jumping")]
    [SerializeField] private float _gravity = 3;
    [SerializeField] private float _jumpBoost = 0.2f;
    [SerializeField] private float _maxFallSpeed = 15;
    [SerializeField] private float _upwardsGravityModifier = 1;
    [SerializeField] private float _downwardsGravityModifier = 1.5f;
    [SerializeField] private float _jumpCooldownTime = 0.3f;
    [SerializeField] private bool _wasGroundedLastFrame = false;
    [SerializeField] private int _airJumpsLeft = 1;
    [SerializeField] private float _jumpSpeed = 5;
    [SerializeField] private int _maxJumpsInAir = 1;    
    [SerializeField] private float _bufferedJumpCountdown = 0.05f;
    [SerializeField] private float _coyoteJumpCountdown = 0.05f;
    private float _timeWhenLeftGround = -10;
    private float _timeWhenPressedJump = -10;
    private float _timeWhenJumped = -10;
    private bool _jumpBoosting = false;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collision = GetComponent<Collision>();
        _spriteController = GetComponentInChildren<SpriteController>();
    }

    private void Update()
    {
        HandleJumping();        
        HandleHorizontalMovement();
    }

    private void HandleHorizontalMovement(){

        //Set the horizontal movement vector.
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        //if player is not pressing any movement buttons it will slow down proportional to _stoppingSpeed instead
        if(horizontalInput == 0){
            if(_collision.IsGrounded){
                if(_speed > 0){
                    _speed -= _stoppingSpeed * Time.deltaTime;
                    _speed = Mathf.Max(0, _speed);
                }
                if(_speed < 0){
                    _speed += _stoppingSpeed * Time.deltaTime;;
                    _speed = Mathf.Min(0, _speed);
                }
            }
            else{
                if(_speed > 0){
                    _speed -= _airAcceleration * Time.deltaTime;
                    _speed = Mathf.Max(0, _speed);
                }
                if(_speed < 0){
                    _speed += _airAcceleration * Time.deltaTime;;
                    _speed = Mathf.Min(0, _speed);
                }
            }
        }
        //if player is pressing left movement button while still going right, slowdown the player first,
        if(horizontalInput < 0){
            if(_speed > 0){
                _speed -= _stoppingSpeed * Time.deltaTime;;
                _speed = Mathf.Min(0, _speed);
            }
        }

        if(horizontalInput > 0){
            if(_speed < 0){
                _speed += _stoppingSpeed * Time.deltaTime;;
                _speed = Mathf.Min(0, _speed);
            }
        }

        if(horizontalInput != 0){
            if(_collision.IsGrounded){
                _speed += _acceleration * horizontalInput * Time.deltaTime;;
            }
            else{
                _speed += _airAcceleration * horizontalInput * Time.deltaTime;;
            }
        }
        //Flip the sprite according to the speed
        if(_rigidbody.velocity.x < -0.1){
            _spriteController.Flip(-1);
        }
        if(_rigidbody.velocity.x > 0.1){
            _spriteController.Flip(1);
        }


        //if they're on the ground, set the vertical velocity to zero for some time so they don't hop every time they
        //change their direction while going up a slope.
        if((horizontalInput == -1 && _lastHorizontalInput == 1)||(horizontalInput == 1 && _lastHorizontalInput == -1)){
            if(_collision.IsGrounded && _wasGroundedLastFrame){
                ;//_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            }
        }

        _speed = Mathf.Clamp(_speed, -_maxSpeed, _maxSpeed);

        _lastHorizontalInput = horizontalInput;
        //Move the character
        HorizontalMove(_speed);
    }

    //Change the rigidbody velocity of the player to move
    private void HorizontalMove(float speed){
        _rigidbody.velocity = new Vector2(speed, _rigidbody.velocity.y);
    }

    private void HandleJumping(){
        if(!_collision.IsCollidingTop)
        {
            if((_collision.IsGrounded ||Time.time < _timeWhenLeftGround + _coyoteJumpCountdown) )
            {
                if(Input.GetKeyDown(KeyCode.Z) || Time.time < _timeWhenPressedJump + _bufferedJumpCountdown) 
                {
                    Jump();
                    _timeWhenJumped = Time.time;
                    _jumpBoosting = true;
                }
            } 
            else if(!_collision.IsGrounded && _airJumpsLeft > 0)
            {
                if(Input.GetKeyDown(KeyCode.Z)){
                    _airJumpsLeft--;
                    Jump();
                }
            }
            else
            {
                if(Input.GetKeyDown(KeyCode.Z))
                    _timeWhenPressedJump = Time.time;
            }
        }

        if(Input.GetKey(KeyCode.Z) && !_collision.IsGrounded){
            _jumpBoosting = true;
        }
    }

    private void Jump(){
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpSpeed);
    }

    private void JumpBoost(){
        _rigidbody.velocity += new Vector2(0, _jumpBoost);
    }

    private void FixedUpdate() {
        //Run collision checks from collision script. I originally put this into fixedupdate method of the that script
        //but I decided not to bother myself worrying about the script execution order. So I put it here.
        _collision.RunCollisionChecks(); 

        //Modify the gravity scale depending on if the player is going up or down.
        if(_rigidbody.velocity.y < 0)
        {
            _rigidbody.gravityScale = _gravity * _downwardsGravityModifier;
        }
        if(_rigidbody.velocity.y >= 0)
        {
            _rigidbody.gravityScale = _gravity * _upwardsGravityModifier;
        }

        //if game slowdowns, update method may not get the button pressed in time causing jump boosting to vary. Make another check in fixed update to make sure
        if(Input.GetKey(KeyCode.Z)  && !_collision.IsGrounded){
            _jumpBoosting = true;
        }

        // Handle jump boost
        if(_rigidbody.velocity.y > 0 && _jumpBoosting){
            JumpBoost();
            _jumpBoosting = false;
        }
        if(_collision.IsGrounded){
            _jumpBoosting = false;
        }
        


        //Limit the fall speed so the player doesn't fall too fast.
        if(_rigidbody.velocity.y < -_maxFallSpeed)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -_maxFallSpeed);
        }


        //If the player is colliding with the ground, set the gravityScale to 0 so they don't slide down the slopes.
        if(_collision.IsGrounded){
            _rigidbody.gravityScale = 0;
        }

        //if touched ground, reset jumps left
        if(!_wasGroundedLastFrame && _collision.IsGrounded)
        {
            _airJumpsLeft = _maxJumpsInAir;
            _spriteController.SquashSprite();
        }

        if(_wasGroundedLastFrame && !_collision.IsGrounded)
        {
            _timeWhenLeftGround = Time.time;
        }

        //set was grounded last frame so we can tell if the player left the ground or just landed. 
        //may replace this with built in onEnterCollision later on.
        _wasGroundedLastFrame = _collision.IsGrounded;
    }

    private void OnDrawGizmos()
    {
        
    }
}
