using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private GameObject counterIcon;


    private void Update()
    {
        if (counterIcon.activeSelf)
        {
            counterIcon.transform.LookAt(Camera.main.transform);
        }
    }

    public void SetCounterIconActive(bool isActive)
    {
        counterIcon.SetActive(isActive);
    }

}
