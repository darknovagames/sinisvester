using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpriteController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private PlayerMovement _playerMovement;
    private Collision _collision;
    private Animator _animator;
    private Rigidbody2D _rigidbody;


    [SerializeField] private float _squashYrate = 0.8f;
    [SerializeField] private float _squashXrate = 1.2f;
    [SerializeField] private float _squashYMoveAmount = -0.9f;
    [SerializeField] private float _squashTimer = 0.1f;
    [SerializeField] private float _revertTimer = 0.15f;

    private bool _squashing = false;

    private void Start(){
        _playerMovement = GetComponentInParent<PlayerMovement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collision = GetComponentInParent<Collision>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponentInParent<Rigidbody2D>();
    }

    private void Update(){
        _animator.SetBool("isGrounded",_collision.IsGrounded);
        _animator.SetFloat("velocityY",_rigidbody.velocity.y);
        if(_rigidbody.velocity.x > 0.1f || _rigidbody.velocity.x < -0.1f)
        {
            _animator.SetBool("isWalking", true);  
        } 
        else
        {
            _animator.SetBool("isWalking", false);  
        }
        if(_rigidbody.velocity.x > 2f || _rigidbody.velocity.x < -2f)
        {
            _animator.SetBool("isMovingFast", true);  
        } 
        else
        {
            _animator.SetBool("isMovingFast", false);  
        }
    }

    public void Flip(int side){
        if(side == 1){
            _spriteRenderer.flipX = false;
        }
        else{
            _spriteRenderer.flipX = true;
        }
    }

    public void SquashSprite(){
        if(_squashing == false){
            transform.DOScaleY(_squashYrate, _squashTimer).SetEase(Ease.OutElastic);
            transform.DOScaleX(_squashXrate, _squashTimer).SetEase(Ease.InCirc);
            transform.DOLocalMoveY(-0.9f - (1 - _squashYrate)/2 , _squashTimer).SetEase(Ease.OutElastic).OnComplete(() => SquashRevert());
            _squashing = true;
        }
    }

    public void SquashRevert(){
            transform.DOScaleY(1, _revertTimer).SetEase(Ease.InCirc);
            transform.DOScaleX(1, _revertTimer).SetEase(Ease.InCirc);
            transform.DOLocalMoveY(-0.9f, _revertTimer).SetEase(Ease.InCirc).OnComplete(() => SquashEnds());
    }    

    private void SquashEnds(){
        _squashing = false;
    }
}
