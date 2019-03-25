using UnityEngine.UI;
using UnityEngine;

public class UIIslandDisplay : MonoBehaviour
{
    [System.Serializable]
    public enum State
    {
        inactive, 
        active,
        completed
    }
    public State state;

    private Animator fillAnim;

    private void Awake()
    {
        fillAnim = GetComponent<Animator>();
    }

    public void SetInfo(State newState)
    {
        if (fillAnim == null)
            fillAnim = GetComponent<Animator>();

        if (newState == State.active)
            fillAnim.SetTrigger("active");
        else if (newState == State.inactive)
            fillAnim.SetTrigger("inactive");
        else
            fillAnim.SetTrigger("completed");
    }
}
