using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    private LevelSelectPanel m_LevelSelectPanel;

    [Space(5)]
    [Header("Required references")]
    [SerializeField]
    private Text m_Text;

    private int m_LevelID;

    public void Initialize(LevelSelectPanel panel, int levelId, string levelName)
    {
        m_LevelSelectPanel = panel;
        m_LevelID = levelId;

        m_Text.text = "Level: " + levelName;
    }

    public void SelectLevel()
    {
        m_LevelSelectPanel.SelectLevel(m_LevelID);
    }
}
