using UnityEngine;
using System.Collections.Generic;

//A class that constantly checks if the object is touching the ground or/and the walls 
public class Collision : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private float _collisionDetectionSphereRadius = 0.1f;

    //Transforms which their positions will be used to do the checks for collisions.
    [SerializeField] private Transform _bottomCollisionTf;
    [SerializeField] private Transform _leftCollisionTf;
    [SerializeField] private Transform _rightCollisionTf;
    [SerializeField] private Transform _topCollisionTf;
    [SerializeField, Range(0f,1f)] private float _maxSlope = 0.1f;

    private bool _isCollidingBottom = false;
    private bool _isCollidingLeft = false;
    private bool _isCollidingRight = false;
    private bool _isCollidingTop = false;
    private bool _isStandingOnSlope = false;


    public bool IsGrounded { get => _isCollidingBottom;} 
    public bool IsCollidingRight { get => _isCollidingRight;} 
    public bool IsCollidingLeft { get => _isCollidingLeft;} 
    public bool IsCollidingTop { get => _isCollidingTop;} 
    public bool IsStandingOnSlope { get => _isCollidingBottom;}





    public void RunCollisionChecks(){
        //_isCollidingBottom  = Physics2D.OverlapCircle(_bottomCollisionTf.position, _collisionDetectionSphereRadius, _groundLayer);
        _isCollidingRight  = Physics2D.OverlapCircle(_rightCollisionTf.position, _collisionDetectionSphereRadius, _groundLayer);
        _isCollidingLeft  = Physics2D.OverlapCircle(_leftCollisionTf.position, _collisionDetectionSphereRadius, _groundLayer);
        _isCollidingTop  = Physics2D.OverlapCircle(_topCollisionTf.position, _collisionDetectionSphereRadius, _groundLayer);
    }

    void FixedUpdate(){

            _isCollidingBottom = false;
    }


    void OnCollisionStay2D(Collision2D other) {
            for (var i = 0; i < other.contactCount; i++)
            {
                if(other.GetContact(i).normal.y > _maxSlope){;
                    _isCollidingBottom = true;
                    break;
                }
                
            }
            
    }


}
