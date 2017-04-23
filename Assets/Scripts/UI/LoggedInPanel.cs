using UnityEngine;
using UnityEngine.UI;

public class LoggedInPanel : IPanel
{
    [SerializeField]
    private Session m_Session;

    [Space(5)]
    [Header("Required references")]
    [SerializeField]
    private Text m_Text;

    private void Awake()
    {
        Hide();
    }

    private void Start()
    {
        m_Session.LoginEvent += OnLogin;
        m_Session.LogoutEvent += OnLogout;
    }

    private void OnDestroy()
    {
        m_Session.LoginEvent -= OnLogin;
        m_Session.LogoutEvent -= OnLogout;
    }

    public void Logout()
    {
        m_Session.Logout();
    }

    //Events
    private void OnLogin(string userName)
    {
        m_Text.text = "Logged in as " + userName;
        Show();
    }

    private void OnLogout()
    {
        Hide();
    }
}
