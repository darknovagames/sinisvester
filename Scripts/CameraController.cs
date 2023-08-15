using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTf;
    [SerializeField] private Vector3 _defaultCameraOffset;
    [SerializeField] private float _smoothTime = 0.1f;

    private Vector3 _cameraVelocity;

    private void Update(){
        transform.position = Vector3.SmoothDamp(transform.position, _playerTf.position + _defaultCameraOffset ,ref _cameraVelocity,_smoothTime);
    }

}
