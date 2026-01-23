using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController
{
    protected override void Update()
    {
        UpdateRawMove();   
        base.Update();
    }

    private void UpdateRawMove()
    {
        if (isDead || isHurt)
            return;

        speedMultiplier = isAttacking ? 0.5f : 1f;

        Vector2 rawMove = Vector2.zero;

        if (Keyboard.current.aKey.isPressed)
        {
            rawMove += Vector2.left;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            rawMove += Vector2.right;
        }

        desiredMove =rawMove;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            mustJump=true;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            mustPunch = true;
        }
    }

    public override void TakeDamage(int damage)
    {
        CameraShake.Instance.Shake(0.025f);
        base.TakeDamage(damage);
    }
    public override void Die()
    {
        Debug.Log("Player defeated!");

        GameController.instance.Lifes--;
        GameController.instance.UpdateLifes();

        if (GameController.instance.Lifes <= 0)
        {
            GameController.instance.ShowGameOverPanel(true);
            return;
        }
        else
        {
            GameController.instance.Respawn();
        }
    }

    public void ResetState()
    {
        isHurt = false;
        isDead = false;

        desiredMove = Vector2.zero;
        mustPunch = false;
        mustJump = false;

        rb2d.linearVelocity = Vector2.zero;
        rb2d.simulated = true;

        ani.Rebind();
        ani.Update(0f);
    }
}
