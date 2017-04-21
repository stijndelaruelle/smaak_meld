using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Visuals;

    public virtual void Show()
    {
        m_Visuals.SetActive(true);
    }

    public virtual void Hide()
    {
        m_Visuals.SetActive(false);
    }

    public void Toggle()
    {
        if (m_Visuals.activeSelf)
            Hide();
        else
            Show();
    }
}
