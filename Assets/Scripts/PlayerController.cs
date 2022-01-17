using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator ac;
    public GameManager GM;
    CapsuleCollider selfColider;
    Rigidbody rg;

    public float JumpSpeed = 12;

    int laneNumber = 1; // номер текущей линии
    int lanesCount = 2; // количество линии (их у нас 3: 0,1,2)

    public float FirstLanePosition, // позиция нулевой линии
                 LaneDistance, // растояние между линиями
                 SideSpeed; // скорость перемещения с одной линии на другую

    bool isRolling = false;

    Vector3 ccCenterNorm = new Vector3(0, .96f, 0),
            ccCenterRoll = new Vector3(0, .4f, 0);

    float ccHeightNorm = 2,
          ccHeightRoll = .4f;

    bool wannaJump = false;
    Vector3 StartPosition;

    // Start is called before the first frame update
    void Start()
    {
        ac = GetComponent<Animator>();
        selfColider = GetComponent<CapsuleCollider>();
        rg = GetComponent<Rigidbody>();

        StartPosition = transform.position;

    }

    private void FixedUpdate()
    {
        rg.AddForce(new Vector3(0, Physics.gravity.y * 4, 0), ForceMode.Acceleration);

        if (wannaJump && isGrounded())
        {
            ac.SetTrigger("Jumping");
            rg.AddForce(new Vector3(0, JumpSpeed, 0), ForceMode.Impulse);
            wannaJump = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isGrounded())
        {
            if(GM.CanPlay)
            {
                if(!isRolling)
                {
                    if (Input.GetAxisRaw("Vertical") > 0)
                        wannaJump = true;
                    else if (Input.GetAxisRaw("Vertical") < 0)
                        StartCoroutine(DoRoll());
                }
            }
        }
        else if (rg.velocity.y < - 8) {
            ac.SetTrigger("Falling");
            
        }
        //Debug.DrawRay(transform.position, Vector3.down * 0.05f, Color.red);

        CheckInput();

        Vector3 newPosition = transform.position;
        newPosition.z = Mathf.Lerp(newPosition.z, FirstLanePosition + (laneNumber * LaneDistance), Time.deltaTime * SideSpeed);
        transform.position = newPosition;

    }

    bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.05f);
    }

    void CheckInput()
    {
        int sign = 0;

        if (!GM.CanPlay || isRolling)
            return;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            sign = -1;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            sign = 1;
        else
            return;

        laneNumber += sign;
        laneNumber = Mathf.Clamp(laneNumber, 0, lanesCount);
    }
    IEnumerator DoRoll()
    {
        Debug.Log("rolling");
        isRolling = true;
        ac.SetBool("Rolling", true);

        selfColider.center = ccCenterRoll;
        selfColider.height = ccHeightRoll;

        yield return new WaitForSeconds(1.5f); // через 1,5 секунды
        isRolling = false;
        ac.SetBool("Rolling", false);
        Debug.Log("not rolling");


        selfColider.center = ccCenterNorm;
        selfColider.height = ccHeightNorm;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) // при столкновениями с другими колайдорами
    {
        if (!hit.gameObject.CompareTag("Trap") || !GM.CanPlay)
            return;

        StartCoroutine(Death());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Trap") || !GM.CanPlay)
            return;
        StartCoroutine(Death());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin"))
            return;

        GM.AddCoins(1);
        Destroy(other.gameObject);
    }

    IEnumerator Death()
    {
        GM.CanPlay = false;
        ac.SetTrigger("Death");
        yield return new WaitForSeconds(2);
        ac.ResetTrigger("Death");

        GM.ShowResult();
    }

    public void ResetPosition()
    {
        transform.position = StartPosition;
        laneNumber = 1;
    }
}
