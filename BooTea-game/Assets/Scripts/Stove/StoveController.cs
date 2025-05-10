using UnityEngine;

public class Stove : MonoBehaviour, IInteractable
{
    public GameObject stovePanel;
    private bool isPanelOpen = false;
    public float closeDistance = 1f;
    private Transform player;
    public Animator anim;

    public bool canInteract()
    {
        return true;
    }

    public void Interact()
    {
        isPanelOpen = !isPanelOpen;
        stovePanel.SetActive(isPanelOpen);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isPanelOpen && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > closeDistance)
            {
                stovePanel.SetActive(false);
                isPanelOpen = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
        }
    }
}
