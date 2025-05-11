using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [SerializeField] private HintManager _hintManager;
    [SerializeField] private string _hintText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _hintManager.ShowHint(_hintText);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _hintManager.HideHint();
        }
    }
}