using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 _direction;
    private Vector3 _velocity;
    [SerializeField]
    private float _speed;
    void Start()
    {
        
    }
    void Update()
    {
        Movment();
    }
    private void Movment()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.z = Input.GetAxisRaw("Vertical");
        if(_direction.x==0 && _direction.z == 0)
        {
            return;
        }
        Vector3.Normalize(_direction);
        transform.forward = _direction;
        _velocity = _direction.normalized * _speed * Time.deltaTime;
        transform.position += _velocity;
    }
    public Vector3 GetVelocity()
    {
        return _velocity;
    }
}
