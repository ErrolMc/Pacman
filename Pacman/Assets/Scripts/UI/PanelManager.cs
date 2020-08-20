using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager instance;

    [SerializeField] PanelID initialPanel;

    PanelID currentPanel;
    Panel[] panelList;

    void Awake()
    {
        instance = this;
        panelList = GetComponentsInChildren<Panel>(true);
        ShowPanel(initialPanel, true);
    }

    public void ShowPanel(PanelID panel, bool force = false)
    {
        for (int i = 0; i < panelList.Length; i++)
        {
            if (panelList[i].panelID == panel)
            {
                panelList[i].gameObject.SetActive(true);
                panelList[i].isActive = true;
                panelList[i].OnShow();
            }
            else if (panelList[i].isActive || force)
            {
                panelList[i].gameObject.SetActive(false);
                panelList[i].isActive = false;
                panelList[i].OnHide();
            }
        }

        currentPanel = panel;
    }
}
