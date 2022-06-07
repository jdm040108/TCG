using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
     public int PriorityCardNum;
     public int PriorityCardPattern;
     public Vector2 beforeTransform;
    [HideInInspector] public SpriteRenderer sprite;
    [HideInInspector] public bool isChange = false;
    [HideInInspector] public bool isChoice = false;
    CardManager cardManager;
    private void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
        sprite = GetComponent<SpriteRenderer>();
        beforeTransform = this.transform.position;
    }
    private void OnMouseDown()
    {
        cardManager.CardMouseDown(this);
    }
    private void OnMouseDrag()
    {
        cardManager.CardMouseDrag(this);
    }
    private void OnMouseUp()
    {
        cardManager.CardMouseUp(this);
    }
}
