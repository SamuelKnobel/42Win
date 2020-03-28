using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

/// <summary>
///  Class to move an Object in different ways
/// </summary>
public class Mover : MonoBehaviour
{

    Rigidbody2D rb2D;
    float ColliderHalfeWith;


    private void Awake()
    {
        rb2D = this.GetComponent<Rigidbody2D>();
        switch (this.GetComponent<Collider2D>().name)
        {
            case "CapsuleCollider2D":
                ColliderHalfeWith = GetComponent<CapsuleCollider2D>().size.x / 2;
                break;
            case "CircleCollider2D":
                ColliderHalfeWith = GetComponent<CircleCollider2D>().radius;
                break;

            default:
                Debug.LogError("Not Defined Collider: " + this.GetComponent<Collider2D>().name);
                break;
        }
    }


    /// <summary>
    /// Moves Target horizontal
    /// Belonges into the Fixed Update Methode
    /// </summary>
    /// <param name="Axis"> movement axis defined in the Inputmanager</param>
    /// <param name="Speed">Speed of movement</param>
    public void MoveHorizontal(string Axis, float Speed)
    {
        if (Input.GetAxis(Axis) > 0)
        {
            rb2D.MovePosition(transform.position + transform.right * Time.deltaTime * Speed * CalculateClampedX("R"));
        }
        else if (Input.GetAxis(Axis) < 0)
        {
            rb2D.MovePosition(transform.position + -transform.right * Time.deltaTime * Speed * CalculateClampedX("L"));
        }
    }


    float CalculateClampedX(string LR)
    {
        float validX = 1;
        switch (LR)
        {
            case "L":
                if (transform.position.x - ColliderHalfeWith < ScreenUtils.ScreenLeft)
                    validX = 0;
                break;
            case "R":
                if (transform.position.x + ColliderHalfeWith > ScreenUtils.ScreenRight)
                    validX = 0;
                break;
            default:
                break;
        }
        return validX;
    }
}

