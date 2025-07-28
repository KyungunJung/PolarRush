using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class Magnet : MonoBehaviour
{
    public Polarity Polarity;           // ±Ø¼º ¼³Á¤

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (spriteRenderer == null) return;

        if (Polarity == Controller.Polarity.North)
            spriteRenderer.color = Color.blue;
        else
            spriteRenderer.color = Color.red;
    }

}