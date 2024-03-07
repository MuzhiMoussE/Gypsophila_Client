using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    [SerializeField]private Slider _slider;
    [SerializeField] private Slider _slider_recording;
    private GameObject _player;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _player = gameObject;
        StateSystem.Instance.recordSlider = _slider;
        StateSystem.Instance.recordingTimeSlider = _slider_recording;
      
        _slider_recording.GameObject().SetActive(false);
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
    }
    private void Update()
    {
        StateSystem.Instance.InputListener(_player, _rigidbody);
    }
}
