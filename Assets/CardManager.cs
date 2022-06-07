using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private List<Card> Card;//ī�� ����Ʈ
    private Card nowChoiceCard = null;//���� ���õǾ� �ִ� ī��
    private IEnumerator changeCo;//�� �ڷ�ƾ�� �����Ű�� ���� ������
    private IEnumerator choiceCo;//"
    private IEnumerator sizeupCo;//"
    private IEnumerator sizeDownCo;//"
    private IEnumerator cardLineCo;//"
    private bool isDrag = false;//�巡�׸� �Ͽ� ī�� ������ �ٲ���� Ȯ��
    private float dTime;//ī�带 ������ �巡�׸� Ȱ�� ��ų�� �ʿ��� �����ð�

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
    //���콺�� �������� �߻��Ǵ� �Լ�
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
    //���콺�� ������ �߻��Ǵ� �Լ�
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
    //�巡�׸� �Ҷ� �߻��Ǵ� �Լ�
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
    //ī�尡 ���õǾ��� �� ����Ǿ� ��ġ�� �ٲٴ� �ڷ�ƾ
    IEnumerator CardChoice(Card card)
    {
        while (card.transform.position.y < 1.5f - 2.5f || card.transform.localScale != new Vector3(7, 7, 1))
        {
            card.transform.position = Vector2.MoveTowards(card.transform.position, new Vector2(card.transform.position.x, 1.5f - 2.5f), 0.1f);
            card.transform.localScale = Vector3.MoveTowards(card.transform.localScale, new Vector3(7, 7, 1), 0.1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    //ī���� �Ÿ��� ���ؼ� ���ǿ� ������ ������� ��ġ�� �ٲٴ� �ڷ�ƾ
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
    //ī���� ����� �ø��� �ڷ�ƾ
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
    //ī���� ����� ���̴� �ڷ�ƾ
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
    //ī�带 �����ϴ� �ڷ�ƾ
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