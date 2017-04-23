using CryptSharp;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class Session : MonoBehaviour
{
    public delegate void UserDelegate(string name); //Will change to a with 
    public delegate void VoidDelegate();
    public delegate void ErrorDelegate(string error);

    [Header("URL's")]
    [SerializeField]
    private string m_LoginCheckURL;

    [SerializeField]
    private string m_RegisterURL;

    private string m_SecretKey = "afzlearvtoyuurimpeqlsddkfagmhejrk"; //flavourmeldkamer with salt in between

    //Cached values (eventually becomes a struct)
    private string m_CurrentUsername;
    private string m_CurrentPasswordMD5;
    private string m_CurrentHash;

    private Coroutine m_ActiveCoroutine;

    //Events
    public event UserDelegate LoginEvent;
    public event VoidDelegate LogoutEvent;
    public event UserDelegate RegisterEvent;
    public event ErrorDelegate ErrorEvent;

    public void Login(string name, string password)
    {
        if (m_ActiveCoroutine != null)
            StopCoroutine(m_ActiveCoroutine);

        m_ActiveCoroutine = StartCoroutine(LoginCheckRoutine(name, password));
    }

    public void Register(string name, string password)
    {
        if (m_ActiveCoroutine != null)
            StopCoroutine(m_ActiveCoroutine);

        m_ActiveCoroutine = StartCoroutine(RegisterRoutine(name, password));
    }

    public void Logout()
    {
        if (LogoutEvent != null)
            LogoutEvent();
    }

    private IEnumerator LoginCheckRoutine(string name, string password)
    {
        string passwordHash = Md5Encrypt(password);
        string hash = Md5Encrypt(name + m_SecretKey + passwordHash);
        string postUrl = m_LoginCheckURL + "?n=" + WWW.EscapeURL(name) + "&p=" + passwordHash + "&h=" + hash;

        WWW loginCheckPost = new WWW(postUrl);
        yield return loginCheckPost;

        if (loginCheckPost.error != null)
        {
            Debug.Log("There was an error atempting to log in: " + loginCheckPost.error);
        }
        else
        {
            string response = loginCheckPost.text;
            if (response.StartsWith("E_"))
            {
                if (ErrorEvent != null)
                    ErrorEvent(response.Remove(0, 2));
            }
            else
            {
                //Cache some values
                m_CurrentUsername = WWW.EscapeURL(name);
                m_CurrentPasswordMD5 = passwordHash;
                m_CurrentHash = hash;

                if (LoginEvent != null)
                    LoginEvent(name);
            }
        }

        m_ActiveCoroutine = null;
    }

    private IEnumerator RegisterRoutine(string name, string password)
    {
        string passwordHash = Md5Encrypt(password);
        string hash = Md5Encrypt(name + m_SecretKey + passwordHash);
        string postUrl = m_RegisterURL + "?n=" + WWW.EscapeURL(name) + "&p=" + passwordHash + "&h=" + hash;

        WWW registerPost = new WWW(postUrl);
        yield return registerPost;

        if (registerPost.error != null)
        {
            Debug.Log("There was an error atempting to register: " + registerPost.error);
        }
        else
        {
            string response = registerPost.text;
            if (response.StartsWith("E_"))
            {
                if (ErrorEvent != null)
                    ErrorEvent(response.Remove(0, 2));
            }
            else
            {
                if (RegisterEvent != null)
                    RegisterEvent(name);
            }
        }

        m_ActiveCoroutine = null;
    }


    public string GetBasePostParameters()
    {
        if (m_CurrentPasswordMD5 == "" || m_CurrentHash == "")
            return "";

        return "?n=" + m_CurrentUsername + "&p=" + m_CurrentPasswordMD5 + "&h=" + m_CurrentHash;
    }


    private string BlowfishEncrypt(string stringToEncrypt)
    {
        //return Crypter.Blowfish.Crypt(stringToEncrypt, new CrypterOptions
        //                                                {
        //                                                    { CrypterOption.Variant, BlowfishCrypterVariant.Corrected },
        //                                                    { CrypterOption.Rounds, 10 }
        //                                                });
        return "Depricated";
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
}
