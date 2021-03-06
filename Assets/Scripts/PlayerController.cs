﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sprite;
    public Vector3 lastMoveDir;

    private bool canDash = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleMovement();
        HandleDash();
    }

    private void FixedUpdate()
    {
        if (GetComponent<CharacterStats>().heatlh <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void HandleMovement()
    {
        float speed = gameObject.GetComponent<CharacterStats>().speed;
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
        }

        Vector3 moveDir = new Vector3(moveX, moveY).normalized;

        //Animations
        anim.SetFloat("horizontal", moveDir.x);
        anim.SetFloat("vertical", moveDir.y);
        if (moveDir.x < 0 || (moveDir.x == 0 && anim.GetInteger("facing") == 4))
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }

        lastMoveDir = moveDir;

        if (lastMoveDir.x == 1)
        {
            anim.SetInteger("facing", 2);
        }
        if (lastMoveDir.x == -1)
        {
            anim.SetInteger("facing", 4);
        }
        if (lastMoveDir.y == 1)
        {
            anim.SetInteger("facing", 1);
        }
        if (lastMoveDir.y == -1)
        {
            anim.SetInteger("facing", 3);
        }

        transform.position += moveDir * speed * Time.deltaTime;
    }

    private void HandleDash()
    {
        if (canDash == false)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            float dashDistance = 2f;
            transform.position += lastMoveDir * dashDistance;
            canDash = false;
            StartCoroutine(dashCooldown());
        }
    }

    private IEnumerator dashCooldown()
    {
        yield return new WaitForSeconds(2f);
        canDash = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ExplosionArea")
        {
            Destroy(collision.gameObject);
            GetComponent<CharacterStats>().heatlh -= 100;
        }

        if (collision.gameObject.tag == "EnemyBullet")
        {
            if (collision.gameObject.GetComponent<bulletInfo>().bulletType == 4)
            {
                collision.gameObject.GetComponent<bulletDestroy>().explode();
                return;
            }

            GetComponent<CharacterStats>().heatlh -= collision.GetComponent<bulletInfo>().damage;
            Destroy(collision.gameObject);
        }
    }
}
