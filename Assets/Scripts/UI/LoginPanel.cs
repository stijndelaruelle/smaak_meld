using CryptSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : IPanel
{
    public delegate void UserDelegate(string name); //Will change to a with 

    [SerializeField]
    private LoggedInPanel m_LoggedInPanel;

    [Space(5)]
    [Header ("URL's")]
    [SerializeField]
    private string m_LoginCheckURL;

    [SerializeField]
    private string m_RegisterURL; 

    [Space(5)]
    [Header("Required references")]
    [SerializeField]
    private Text m_ErrorText;

    [SerializeField]
    private InputField m_NameInputField;

    [SerializeField]
    private InputField m_PasswordInputField;

    private string m_SecretKey = "afzlearvtoyuurimpeqlsddkfagmhejrk"; //flavourmeldkamer with salt in between

    //Events
    public event UserDelegate LoginEvent;
    public event UserDelegate RegisterEvent;

    private void Awake()
    {
        Show();
    }

    private void Start()
    {
        m_LoggedInPanel.LogoutEvent += OnLogout;
    }

    private void OnDestroy()
    {
        m_LoggedInPanel.LogoutEvent -= OnLogout;
    }

    private void Login()
    {
        string name = m_NameInputField.text;
        string password = m_PasswordInputField.text;

        StartCoroutine(LoginCheckRoutine(name, password));
    }

    private void Register()
    {
        string name = m_NameInputField.text;
        string password = m_PasswordInputField.text;

        StartCoroutine(RegisterRoutine(name, password));
    }

    private IEnumerator LoginCheckRoutine(string name, string password)
    {
        string passwordHash = Md5Encrypt(password);
        string hash = Md5Encrypt(name + m_SecretKey + passwordHash);
        string postUrl = m_LoginCheckURL + "?n=" + WWW.EscapeURL(name) + "&p=" + passwordHash + "&h=" + hash + "";

        WWW loginCheckPost = new WWW(postUrl);
        yield return loginCheckPost;

        if (loginCheckPost.error != null)
        {
            Debug.Log("There was an error atempting to log in: " + loginCheckPost.error);
        }
        else
        {
            string response = loginCheckPost.text;
            if (response != "")
            {
                m_ErrorText.text = response;
            }
            else
            {
                if (LoginEvent != null)
                    LoginEvent(name);

                Hide();
            }
        }
    }

    private IEnumerator RegisterRoutine(string name, string password)
    {
        string passwordHash = Md5Encrypt(password);
        string hash = Md5Encrypt(name + m_SecretKey + passwordHash);
        string postUrl = m_RegisterURL + "?n=" + WWW.EscapeURL(name) + "&p=" + passwordHash + "&h=" + hash + "";

        WWW registerPost = new WWW(postUrl);
        yield return registerPost;

        if (registerPost.error != null)
        {
            Debug.Log("There was an error atempting to register: " + registerPost.error);
        }
        else
        {
            string response = registerPost.text;
            if (response != "")
            {
                m_ErrorText.text = response;
            }
            else
            {
                if (RegisterEvent != null)
                    RegisterEvent(name);
            }
        }
    }


    private string BlowfishEncrypt(string stringToEncrypt)
    {
        return Crypter.Blowfish.Crypt(stringToEncrypt, new CrypterOptions
                                                        {
                                                            { CrypterOption.Variant, BlowfishCrypterVariant.Corrected },
                                                            { CrypterOption.Rounds, 10 }
                                                        });
    }

    //Taken from: http://wiki.unity3d.com/index.php?title=MD5
    public string Md5Encrypt(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    public string Md5EncryptCrypter(string strToEncrypt)
    {
        return Crypter.MD5.Crypt(strToEncrypt);

        /*
        , new CrypterOptions
                                                {
                                                    { CrypterOption.Variant, MD5CrypterVariant.Standard },
                                                    { CrypterOption.Rounds, 10 }
                                                }
                                                */
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
    private void OnLogout()
    {
        Show();
    }
}
