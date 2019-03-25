using UnityEngine;

public class Saw : MonoBehaviour
{
    public GameObject grassCutEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bonus")
            FindObjectOfType<Player>().AddBonus();

        grassCutEffect.transform.position = collision.transform.position;
        grassCutEffect.GetComponent<ParticleSystem>().Play();

        Destroy(collision.gameObject);

        Vibration.Vibrate(10);

        GameManager.instance.AddScore();
    }
}
