using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform sawA, sawB;
    public GameObject posChangeEffect;
    public Animator cameraShakeAnim;
    public Animator rageAnim;

    [HideInInspector]
    public float spinSpeed;
    [HideInInspector]
    public float radius = 0.1f;
    [HideInInspector]
    public int bonus = 0;
    [HideInInspector]
    private int direction;
    private float resizeSpeed = 1;
    private bool isChangingRadius = true;
    private bool stopSpinning = false;
    [HideInInspector]
    public Transform currentSaw;
    [SerializeField]
    private LineRenderer rope;
    [SerializeField]
    private Transform ropeTransform;
    private CameraFollow myCamera;

    private void Start()
    {
        currentSaw = sawA;
    }

    public void Spawn()
    {
        myCamera = FindObjectOfType<CameraFollow>();
        rageAnim.SetBool("isRage", false);

        isChangingRadius = true;
        stopSpinning = false;

        sawA.localPosition = Vector3.zero;
        sawB.localPosition = Vector3.zero;

        currentSaw = sawA;

        direction = 1;
        radius = 2;
        resizeSpeed = 1;
        bonus = 0;

        posChangeEffect.transform.position = currentSaw.position;
        posChangeEffect.GetComponent<ParticleSystem>().Play();

        cameraShakeAnim.SetTrigger("Shake");
    }

    private void Update()
    {
        if (isChangingRadius)
            ChangeRadius();

        Movement();
        DrawRope();
    }

    private void Movement()
    {
        if (stopSpinning)
            return;

        transform.RotateAround(currentSaw.position, transform.forward, spinSpeed * Time.deltaTime);

        if (isChangingRadius)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            spinSpeed *= -1;

            if (currentSaw == sawA && CheckPosition(sawB.position))
                currentSaw = sawB;
            else if (currentSaw == sawB && CheckPosition(sawA.position))
                currentSaw = sawA;
            else
                GameManager.instance.PlayerLost();

            posChangeEffect.transform.position = currentSaw.position;
            posChangeEffect.GetComponent<ParticleSystem>().Play();
        }
      

        myCamera.target = currentSaw;
    }

    private void DrawRope()
    {
        rope.SetPosition(0, sawA.position);
        rope.SetPosition(1, sawB.position);
    }

    private bool CheckPosition(Vector2 pos)
    {
        if (Physics2D.OverlapCircle(pos, 0.1f, 1 << 8))
            return true;

        return false;
    }

    private void ChangeRadius()
    {
        float currentRadius = Vector3.Distance(sawA.position, sawB.position);

        ropeTransform.localScale = new Vector3(0.1f, -(currentRadius), 1f);

        if (direction > 0)
        {
            if (currentRadius > radius)
            {
                isChangingRadius = false;
                stopSpinning = false;
                return;
            }
        }
        else if (direction < 0)
        {
            if (currentRadius < radius)
            {
                isChangingRadius = false;
                stopSpinning = false;
                return;
            }
        }

        if (currentSaw == sawA)
        {
            sawB.Translate(Vector3.up * -direction * resizeSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            sawA.Translate(Vector3.up * direction * resizeSpeed * Time.deltaTime, Space.Self);
        }    
    }

    private void ActivateBonus()
    {
        rageAnim.SetBool("isRage", true);

        bonus = 0;

        radius = 4;
        direction = 1;

        isChangingRadius = true;
        stopSpinning = true;

        myCamera.viewSize = 9;

        Invoke("DeactivateBonus", 10);
    }

    private void DeactivateBonus()
    {
        rageAnim.SetBool("isRage", false);

        radius = 2;
        direction = -1;

        isChangingRadius = true;
        stopSpinning = true;

        myCamera.viewSize = 7;
    }

    public void Close()
    {
        CancelInvoke("DeactivateBonus");
        DeactivateBonus();
    }

    public void AddBonus()
    {
        cameraShakeAnim.SetTrigger("Shake");

        bonus++;

        Vibration.Vibrate(10);

        if (bonus >= 3)
            ActivateBonus();
    }
}
