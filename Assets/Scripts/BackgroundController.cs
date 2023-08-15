using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private Transform _mainCameraTf;

    private Transform[] _bgLayerList;

    private float[] _bgScrollList;

    private void Start(){
        _mainCameraTf = Camera.main.transform;
    }

    private void Update(){
        for (var i = 0; i < _bgScrollList.Length; i++)
        {
            
        }
    }
}
