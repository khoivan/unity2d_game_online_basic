using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterController : MonoBehaviour
{
    private Animator _animator;
    private string _animNameCurrent = "Idle";
    private string _lastAnimName = "MoveDown";
    public bool isBlocked = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = 0f;
    }

    public void Move(string animName, Vector3 position)
    {
        transform.position = position;
        if (_animNameCurrent == animName) return;
        _animNameCurrent = animName;
        _lastAnimName = animName;
        _animator.Play(animName);
        _animator.speed = 1f;
    }

    public void Idle()
    {
        if (_animator.speed == 0f) return;
        _animNameCurrent = "Idle";
        _animator.speed = 0f;
    }

    public void Attack() {
        isBlocked = true;
        _animator.Play(_lastAnimName, 0, 0f);
        StartCoroutine(_EndAttack());
    }

    IEnumerator _EndAttack() {
        yield return new WaitForSeconds(0.2f);
        isBlocked = false;
        _animator.Play(_lastAnimName, 0, 0.5f * 0.6f);
    }

    public string GetState() {
        return _animNameCurrent;
    }
}
