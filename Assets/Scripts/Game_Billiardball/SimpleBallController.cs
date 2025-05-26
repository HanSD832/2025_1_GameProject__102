using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBallController : MonoBehaviour
{
    [Header("�⺻ ����")]
    public float power = 10f;                           //Ÿ�� ��
    public Sprite arrowSprite;                          //ȭ��ǥ �̹��� 

    private Rigidbody rb;                               //���� ����
    private GameObject arrow;                           //ȭ��ǥ ������Ʈ
    private bool isDragging = false;                    //�巡�� ������
    private Vector3 startPos;                           //�巡�� ���� ��ġ 

    //�� ������ ���� ���� ����(��� ���� ����)
    static bool isAnyBallPlaying = false;                           //� ���̶� �� ������
    static bool isAnyBallMoveing = false;                           //� ���̶� �����̴��� 

    // Start is called before the first frame update
    void Start()
    {
        SetupBall();                    //������ �� �ʱ�ȭ
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateArrow();
    }

    void SetupBall()                                    //�� �����ϱ�
    {
        rb = GetComponent<Rigidbody>();                 //���� ������Ʈ �������� 
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();      //���� ��� �ٿ��ش�. 

        //���� ����
        rb.mass = 1;
        rb.drag = 1;
    }

    public bool IsMoving()                      //���� �����̰� �ִ��� Ȯ��
    {
        return rb.velocity.magnitude > 0.2f;    //���� �ӵ��� ������ ������ �����δٰ� �Ǵ� 
    }

    void HandleInput()                      //�Է� ó���ϱ�
    {
        //�� �Ŵ����� ������� ������ ���� �Ұ� (���� �ٸ� ��Ʈ �����ϸ� ���� ����)
        if (!SimpleTurnManager.canPlay) return;
        //다른 공이움직일 때
        if (SimpleTurnManager.anyBallMoving) return;
        //���� �����̰� ������ ���� �Ұ�
        if (IsMoving()) return;

        if (Input.GetMouseButtonDown(0))                    //���콺 Ŭ�� ����
        {
            StartDrag();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)            //���콺 ��ư ����
        {
            Shoot();
        }
    }

    void StartDrag()                    //�巡�� ���� �Լ� 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                startPos = Input.mousePosition;
                CreateArrow();
                Debug.Log("드래그 시작");
            }
        }
    }

    void Shoot()                    //�� �߻� �ϱ� 
    {
        Vector3 mouseDelta = Input.mousePosition - startPos;        //���콺 �̵� �Ÿ��� �� ��� 
        float force = mouseDelta.magnitude * 0.01f * power;

        if (force < 5) force = 5;                                    //�ּ� �� ����

        Vector3 direction = new Vector3(-mouseDelta.x, 0, -mouseDelta.y).normalized;                //���� ��� 

        rb.AddForce(direction * force, ForceMode.Impulse);                              //���� �� ����

        //�� �Ŵ������� ���� �ƴٰ� �˸�(���� ����)
        SimpleTurnManager.OnBallHit();

        //���� 

        isDragging = false;
        Destroy(arrow);
        arrow = null;

        Debug.Log("발사! 힘 : " + force);

    }

    void CreateArrow()          //ȭ��ǥ ����� 
    {
        if (arrow != null)                       //���� ȭ��ǥ ����
        {
            Destroy(arrow);
        }

        arrow = new GameObject("Arrow");                                //�� ȭ��ǥ �����
        SpriteRenderer sr = arrow.AddComponent<SpriteRenderer>();

        sr.sprite = arrowSprite;                                        //ȭ��ǥ �̹��� ���� 
        sr.color = Color.green;
        sr.sortingOrder = 10;


        arrow.transform.position = transform.position + Vector3.up;             //ȭ��ǥ ��ġ (�� ����)
        arrow.transform.localScale = Vector3.one;
    }

    void UpdateArrow()              //ȭ��ǥ ������Ʈ
    {
        if (!isDragging || arrow == null) return;

        Vector3 mouseDelta = Input.mousePosition - startPos;                //���콺 �̵� �Ÿ� ��� 
        float distance = mouseDelta.magnitude;

        float size = Mathf.Clamp(distance * 0.01f, 0.5f, 2f);               //ȭ��ǥ ũ�� ����(���� ����)
        arrow.transform.localScale = Vector3.one * size;

        SpriteRenderer sr = arrow.GetComponent<SpriteRenderer>();           //ȭ��ǥ ���� ���� (�ʷ� -> ����)
        float colorRatio = Mathf.Clamp01(distance * 0.005f);
        sr.color = Color.Lerp(Color.green, Color.red, colorRatio);

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);

        if (distance > 10f)      //�ּ� �Ÿ� �̻� �巡�� ���� �� 
        {
            Vector3 direction = new Vector3(-mouseDelta.x, 0, -mouseDelta.y);
            //2D ��� (������ �� ����) ���� direction ���Ͱ� ����Ű�� ������ ������ ��ȯ 
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;        //������ ��� �ִ� ���� (������ ������ ������ ���ϴ� �ڵ�)
            arrow.transform.rotation = Quaternion.Euler(90, angle, 0);                  //ȭ��ǥ ���� ���� 
        }

    }


}