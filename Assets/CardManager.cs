using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private List<Card> Card;//카드 리스트
    private Card nowChoiceCard = null;//현재 선택되어 있는 카드
    private IEnumerator changeCo;//각 코루틴을 종료시키기 위한 변수들
    private IEnumerator choiceCo;//"
    private IEnumerator sizeupCo;//"
    private IEnumerator sizeDownCo;//"
    private IEnumerator cardLineCo;//"
    private bool isDrag = false;//드래그를 하여 카드 순서를 바꿨는지 확인
    private float dTime;//카드를 눌러서 드래그를 활성 시킬때 필요한 일정시간

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < Card.Count; i++)
            {
                Card[i].beforeTransform = Card[i].transform.position;
            }
                cardLineCo = CardLineUp();
            StartCoroutine(cardLineCo);
        }
    }
    //마우스를 눌렀을때 발생되는 함수
    public void CardMouseDown(Card card)
    {
        if (card.isChoice == false)
        {
            if (nowChoiceCard != null)
            {
                nowChoiceCard.isChoice = false;
                nowChoiceCard.transform.position =new Vector2(nowChoiceCard.beforeTransform.x,0-2.5f);
                nowChoiceCard.transform.localScale = new Vector3(5, 5, 1);
                for (int i = 0; i < Card.Count; i++)
                {
                    if (Card[i] != card)
                    {
                        sizeDownCo = CardSizeDown(Card[i]);
                        StartCoroutine(sizeDownCo);
                    }
                }
            }
            for (int i = 0; i < Card.Count; i++)
            {
                if (Mathf.Abs(card.sprite.sortingOrder - Card[i].sprite.sortingOrder) == 1)
                {
                    sizeupCo = CardSizeUp(Card[i]);
                    StartCoroutine(sizeupCo);
                }
            }
            nowChoiceCard = card;
            card.isChoice = true;
            card.beforeTransform = new Vector2(card.transform.position.x, 0 - 2.5f);
            choiceCo = CardChoice(card);
            StartCoroutine(choiceCo);
        }
        else
        {
            card.transform.localScale = new Vector3(5, 5, 1);
            card.transform.position = card.beforeTransform;
            card.isChoice = false;
            StopCoroutine(choiceCo);
            for (int i = 0; i < Card.Count; i++)
            {
                if (Card[i] != card)
                {
                    sizeDownCo = CardSizeDown(Card[i]);
                    StartCoroutine(sizeDownCo);
                }
            }
        }
    }
    //마우스를 뗐을때 발생되는 함수
    public void CardMouseUp(Card card)
    {
        dTime = 0f;

        if (isDrag == true)
        {
            card.transform.localScale = new Vector3(5, 5, 1);
            card.transform.position = new Vector2(card.beforeTransform.x, 0 - 2.5f);
            isDrag = false;
            card.isChoice = false;
            for (int i = 0; i < Card.Count; i++)
            {
                if (Card[i] != card)
                {
                    sizeDownCo = CardSizeDown(Card[i]);
                    StartCoroutine(sizeDownCo);
                }
            }
        }
    }
    //드래그를 할때 발생되는 함수
    public void CardMouseDrag(Card card)
    {
        dTime += Time.deltaTime;

        if (dTime > 0.5f)
        {

            if (card.isChoice == true)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                card.transform.position = new Vector2(objPosition.x, card.transform.position.y);

                for (int i = 0; i < Card.Count; i++)
                {
                    if (Mathf.Abs(card.sprite.sortingOrder - Card[i].sprite.sortingOrder) == 1)
                    {
                        sizeupCo = CardSizeUp(Card[i]);
                        StartCoroutine(sizeupCo);
                    }
                    else
                    {
                        if (Card[i] != card)
                        {
                            sizeDownCo = CardSizeDown(Card[i]);
                            StartCoroutine(sizeDownCo);
                        }
                    }
                    if (Vector2.Distance(new Vector2(Card[i].transform.position.x, 0), new Vector2(card.transform.position.x, 0)) <= 0.1f)
                    {
                        if (Card[i] != card)
                        {
                            if (Card[i].isChange == false)
                            {
                                isDrag = true;
                                Card[i].isChange = true;
                                changeCo = CardChange(card, Card[i]);
                                StartCoroutine(changeCo);
                            }
                        }
                    }
                }
            }
        }
    }
    //카드가 선택되었을 시 실행되어 위치를 바꾸는 코루틴
    IEnumerator CardChoice(Card card)
    {
        while (card.transform.position.y < 1.5f - 2.5f || card.transform.localScale != new Vector3(7, 7, 1))
        {
            card.transform.position = Vector2.MoveTowards(card.transform.position, new Vector2(card.transform.position.x, 1.5f - 2.5f), 0.1f);
            card.transform.localScale = Vector3.MoveTowards(card.transform.localScale, new Vector3(7, 7, 1), 0.1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    //카드의 거리를 비교해서 조건에 맞을시 실행시켜 위치를 바꾸는 코루틴
    IEnumerator CardChange(Card card, Card ChangeCard)
    {
        int tempLayer = 0;

        tempLayer = card.sprite.sortingOrder;
        card.sprite.sortingOrder = ChangeCard.sprite.sortingOrder;
        ChangeCard.sprite.sortingOrder = tempLayer;

        Vector2 temp = card.beforeTransform;
        card.beforeTransform = ChangeCard.transform.position;

        while (ChangeCard.transform.position.x != temp.x)
        {
            ChangeCard.transform.position = Vector2.MoveTowards(ChangeCard.transform.position, new Vector2(temp.x,0-2.5f), 0.1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ChangeCard.isChange = false;
        StopCoroutine(changeCo);
    }
    //카드의 사이즈를 늘리는 코루틴
    IEnumerator CardSizeUp(Card sizeCard)
    {
        while (sizeCard.transform.localScale != new Vector3(6, 6, 1))
        {
            sizeCard.transform.position = Vector2.MoveTowards(sizeCard.transform.position, new Vector2(sizeCard.transform.position.x, 0.7f - 2.5f), 0.05f);
            sizeCard.transform.localScale = Vector3.MoveTowards(sizeCard.transform.localScale, new Vector3(6, 6, 1), 0.05f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        StopCoroutine(sizeupCo);
    }
    //카드의 사이즈를 줄이는 코루틴
    IEnumerator CardSizeDown(Card sizeCard)
    {
        while (sizeCard.transform.localScale != new Vector3(5, 5, 1))
        {
            sizeCard.transform.position = Vector2.MoveTowards(sizeCard.transform.position, new Vector2(sizeCard.transform.position.x, 0 - 2.5f), 0.05f);
            sizeCard.transform.localScale = Vector3.MoveTowards(sizeCard.transform.localScale, new Vector3(5, 5, 1), 0.05f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        StopCoroutine(sizeDownCo);
    }
    //카드를 정렬하는 코루틴
    IEnumerator CardLineUp()
    {
        Vector2 tempTranse = new Vector2();
        int tempLayer = 0;

        for(int k = 0; k<2;k++)
        for (int i=0;i<Card.Count;i++)
        {
            for (int j = 0; j < Card.Count; j++)
            {
                if(Card[i].PriorityCardNum < Card[j].PriorityCardNum)
                {
                    if(Card[i].sprite.sortingOrder > Card[j].sprite.sortingOrder)
                    {
                        tempTranse = Card[i].beforeTransform;
                        Card[i].beforeTransform = Card[j].beforeTransform;
                        Card[j].beforeTransform = tempTranse;

                        tempLayer = Card[i].sprite.sortingOrder;
                        Card[i].sprite.sortingOrder = Card[j].sprite.sortingOrder;
                        Card[j].sprite.sortingOrder = tempLayer;
                    }
                }
                else if(Card[i].PriorityCardNum == Card[j].PriorityCardNum)
                {
                    if(Card[i].PriorityCardPattern < Card[j].PriorityCardPattern)
                    {
                        if (Card[i].sprite.sortingOrder > Card[j].sprite.sortingOrder)
                        {
                            tempTranse = Card[i].beforeTransform;
                            Card[i].beforeTransform = Card[j].beforeTransform;
                            Card[j].beforeTransform = tempTranse;

                            tempLayer = Card[i].sprite.sortingOrder;
                            Card[i].sprite.sortingOrder = Card[j].sprite.sortingOrder;
                            Card[j].sprite.sortingOrder = tempLayer;
                        }
                    }
                }         

                else if (Card[i].PriorityCardNum > Card[j].PriorityCardNum)
                {
                    if (Card[i].sprite.sortingOrder < Card[j].sprite.sortingOrder)
                    {
                        tempTranse = Card[i].beforeTransform;
                        Card[i].beforeTransform = Card[j].beforeTransform;
                        Card[j].beforeTransform = tempTranse;

                        tempLayer = Card[i].sprite.sortingOrder;
                        Card[i].sprite.sortingOrder = Card[j].sprite.sortingOrder;
                        Card[j].sprite.sortingOrder = tempLayer;
                    }
                }
            }    
        }

        float dLineTime = 0;

        while (dLineTime <= 10f)
            {
                dLineTime += Time.deltaTime;
                for (int i = 0; i < Card.Count; i++)
                {
                Card[i].transform.position = Vector2.MoveTowards(Card[i].transform.position, new Vector2(Card[i].beforeTransform.x, 0 - 2.5f), 0.1f);
                }
                yield return new WaitForSeconds(Time.deltaTime); 
            }
            StopCoroutine(cardLineCo);
        
    }
}