using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    // Basic time manager that also displays the time 

    [SerializeField] float _elapsedTime;
    public float ElapsedTime
    { get { return _elapsedTime; } set { _elapsedTime = value; } }

    [SerializeField] TMP_Text _text;

    [SerializeField] bool _canTime;
    public bool CanTime
    { get { return _canTime; } set { _canTime = value; } }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (_canTime)
        {
            _elapsedTime += Time.deltaTime;
        }

        int minutes = (int)(_elapsedTime / 60f) % 60;
        int seconds = (int)(_elapsedTime % 60f);
        int milliseconds = (int)(_elapsedTime * 100f) % 100;
        _text.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}
