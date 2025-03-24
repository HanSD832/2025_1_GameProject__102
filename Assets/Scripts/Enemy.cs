using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int Health = 100;                                 //ü�� ����(integer)
    public float Timer = 1.0f;                                 //Ÿ�̸� ����(float)
    public int AttackPoint = 50;                                 //���ݷ� ����(integer)
    // ���� �������� ������Ʈ �Ǳ� "��" 1ȸ
    void Start()
    {
        Health += 100;                                         //Health�� 100 �� �÷��ش�
    }

    // �� ������
    void Update()
    {
        CharacterHealthUp();

        if (Input.GetKeyDown(KeyCode.Space))                      //�����̽� Ű�� ������ ��
        {
            Health -= AttackPoint;                               //ü���� ���ݷ¸�ŭ ����
        }

        CheckDeath();
    }

    public void CharacterHit(int Damage)                                 //����� �޴� �Լ� ����
    {
        Health -= Damage;                                          //���� ���ݷ¿� ���� ü���� ���ҽ�Ų��
    }

    void CheckDeath()
    {
        if (Health <= 0)                                         //ü���� 0�̶��
        {
            Destroy(gameObject);                                 //�� ������Ʈ �ı�
        }
    }

    void CharacterHealthUp()
    {
        Timer-= Time.deltaTime;
        if (Timer <= 0)
        {
            Timer = 1;
            Health += 10;
        }
    }

    
}