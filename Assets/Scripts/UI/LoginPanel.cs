using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : IPanel
{
    [SerializeField]
    private Session m_Session;

    [Space(5)]
    [Header("Required references")]
    [SerializeField]
    private Text m_ErrorText;

    [SerializeField]
    private InputField m_NameInputField;

    [SerializeField]
    private InputField m_PasswordInputField;

    private void Awake()
    {
        Show();
    }

    private void Start()
    {
        m_Session.LoginEvent += OnLogin;
        m_Session.LogoutEvent += OnLogout;
        m_Session.RegisterEvent += OnRegister;
        m_Session.ErrorEvent += OnError;
    }

    private void OnDestroy()
    {
        m_Session.LoginEvent -= OnLogin;
        m_Session.LogoutEvent -= OnLogout;
        m_Session.RegisterEvent -= OnRegister;
        m_Session.ErrorEvent -= OnError;
    }

    public void Login()
    {
        m_Session.Login(m_NameInputField.text, m_PasswordInputField.text);
    }

    public void Register()
    {
        m_Session.Register(m_NameInputField.text, m_PasswordInputField.text);
    }

    //IPanel
    public override void Show()
    {
        base.Show();
        m_ErrorText.text = "";
        m_NameInputField.text = "";
        m_PasswordInputField.text = "";
    }

    //Events
    private void OnLogin(string userName)
    {
        Hide();
    }

    private void OnLogout()
    {
        Show();
    }

    private void OnRegister(string userName)
    {
        m_ErrorText.text = "Registered " + userName;
    }

    private void OnError(string error)
    {
        m_ErrorText.text = error;
    }
}
