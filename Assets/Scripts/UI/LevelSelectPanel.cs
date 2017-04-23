using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectPanel : MonoBehaviour
{
    [SerializeField]
    private Session m_Session;

    [SerializeField]
    private RectTransform m_Content;

    [SerializeField]
    private LevelSelectButton m_ButtonPrefab;

    [Header("URL")]
    [SerializeField]
    private string m_GetLevelListURL;

    [SerializeField]
    private string m_GetLevelURL;

    private Coroutine m_ActiveCoroutine;

    private void OnEnable()
    {
        if (m_ActiveCoroutine != null)
            StopCoroutine(m_ActiveCoroutine);

        m_ActiveCoroutine = StartCoroutine(GetLevelListRoutine());
    }

    public void SelectLevel(int levelID)
    {
        if (m_ActiveCoroutine != null)
            StopCoroutine(m_ActiveCoroutine);

        StartCoroutine(GetLevelRoutine(levelID));
    }


    private IEnumerator GetLevelListRoutine()
    {
        string postUrl = m_GetLevelListURL + m_Session.GetBasePostParameters();

        WWW getLevelListPost = new WWW(postUrl);
        yield return getLevelListPost;

        if (getLevelListPost.error != null)
        {
            Debug.Log("There was an error atempting to get the level list: " + getLevelListPost.error);
        }
        else
        {
            string response = getLevelListPost.text;
            if (response.StartsWith("E_"))
            {
                Debug.LogError(response.Remove(0, 2));
            }
            else
            {
                string[,] parsedResponse = ParseCSV(response);

                for (int y = 0; y < parsedResponse.GetLength(1); ++y)
                {
                    LevelSelectButton button = GameObject.Instantiate<LevelSelectButton>(m_ButtonPrefab, m_Content);

                    int levelID = -1;
                    bool success = int.TryParse(parsedResponse[0, y], out levelID);

                    if (success)
                        button.Initialize(this, levelID, parsedResponse[1, y]);
                }

                Debug.Log(response);
            }
        }

        m_ActiveCoroutine = null;
    }

    private IEnumerator GetLevelRoutine(int levelId)
    {
        string postUrl = m_GetLevelURL + m_Session.GetBasePostParameters() + "&l=" + levelId;

        WWW getLevelPost = new WWW(postUrl);
        yield return getLevelPost;

        if (getLevelPost.error != null)
        {
            Debug.Log("There was an error atempting to get the level: " + getLevelPost.error);
        }
        else
        {
            string response = getLevelPost.text;
            if (response.StartsWith("E_"))
            {
                Debug.LogError(response.Remove(0, 2));
            }
            else
            {
                Debug.Log(response);
            }
        }

        m_ActiveCoroutine = null;
    }

    //Utility
    private string[,] ParseCSV(string contents)
    {
        string[,] result = new string[0, 0];

        //Split the text in rows
        string[] srcRows = contents.Split(new char[] { '\r', '\n' });
        List<string> rows = new List<string>(srcRows);
        rows.RemoveAll(rowName => rowName == "");

        //Split the rows in colmuns
        for (int y = 0; y < rows.Count; ++y)
        {
            string[] srcColumns = rows[y].Split(new char[] { ';' });

            //Create new 2 dimensional array if required (we only now know the size)
            if (result.Length == 0)
                result = new string[srcColumns.Length, rows.Count];

            for (int x = 0; x < srcColumns.Length; ++x)
            {
                //Pretty much impossible, just an extra safety net
                if (x >= result.GetLength(0))
                {
                    Debug.LogWarning("Row consists of more columns than the first row of the table! Source: " + rows[y]);
                    return null;
                }

                result[x, y] = srcColumns[x];
            }
        }

        return result;
    }
}
