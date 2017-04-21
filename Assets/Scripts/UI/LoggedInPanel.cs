using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoggedInPanel : IPanel
{
    [SerializeField]
    private LoginPanel m_LoginPanel;

    [Space(5)]
    [Header("Required references")]
    [SerializeField]
    private Text m_Text;

    public event Action LogoutEvent;

    private void Awake()
    {
        Hide();
    }

    private void Start()
    {
        m_LoginPanel.LoginEvent += OnLogin;
    }

    private void OnDestroy()
    {
        m_LoginPanel.LoginEvent -= OnLogin;
    }

    public void Logout()
    {
        if (LogoutEvent != null)
            LogoutEvent();

        Hide();
    }

    //Events
    private void OnLogin(string userName)
    {
        m_Text.text = "Logged in as " + userName;
        Show();
    }
}
