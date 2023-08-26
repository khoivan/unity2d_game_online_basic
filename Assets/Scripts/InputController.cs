using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private float pingTimer = 0f; 
    private float currentTimer = 0f;
    private CharacterController _characterController;
    private float _moveSpeed = 0.75f;
    void Awake()
    {
        _characterController = gameObject.AddComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_characterController.isBlocked) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            _characterController.Attack();
            Connectivity.Send("Attack", transform.position);
            return; // nextFrame
        }
        if (Input.GetKey(KeyCode.S))
        {
            // move down
            _characterController.Move("MoveDown", transform.position += _moveSpeed * Time.deltaTime * Vector3.down);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _characterController.Move("MoveLeft", transform.position += _moveSpeed * Time.deltaTime * Vector3.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _characterController.Move("MoveRight", transform.position += _moveSpeed * Time.deltaTime * Vector3.right);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            _characterController.Move("MoveUp", transform.position += _moveSpeed * Time.deltaTime * Vector3.up);
        }
        else
        {
            _characterController.Idle();
        }
    }

    private void FixedUpdate()
    {
        // 0.02
        if(!_characterController.isBlocked)
            Connectivity.Send(_characterController.GetState(), transform.position);
    }
}
