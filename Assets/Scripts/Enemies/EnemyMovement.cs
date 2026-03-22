using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float patrolDistance = 2f;
    [SerializeField] private LayerMask obstacleLayer;


    private EnemySensor _sensor;
    private float _leftLimit;
    private float _rightLimit;
    private bool _movingRight = false;

    private void Awake()
    {
        _sensor = GetComponentInChildren<EnemySensor>();
    }

    private void Start()
    {
        _leftLimit = transform.position.x - patrolDistance;
        _rightLimit = transform.position.x + patrolDistance;
    }

    private void Update()
    {
        if (_sensor.PlayerDetected)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (IsHittingWall() || 
            (_movingRight && transform.position.x >= _rightLimit) ||
            (!_movingRight && transform.position.x <= _leftLimit))
        {
            Flip();
        }

        MoveFoward();
    }

    private void Chase()
    {
        var canMoveForward = !(_movingRight && transform.position.x >= _rightLimit);
        if (!_movingRight && transform.position.x <= _leftLimit) canMoveForward = false;
        if (IsHittingWall()) canMoveForward = false;
        
        if (canMoveForward) MoveFoward();
    }

    private void MoveFoward()
    {
        if (_movingRight)
            transform.Translate(Time.deltaTime * speed * Vector2.right);
        else
            transform.Translate(Time.deltaTime * speed * Vector2.left);
    }

    private void Flip()
    {
        _movingRight = !_movingRight;
        float localScaleX =  transform.localScale.x;
        localScaleX *= -1;
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private bool IsHittingWall()
    {
        Vector2 direction = _movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer);
        
        return hit.collider is not null;
    }
}
