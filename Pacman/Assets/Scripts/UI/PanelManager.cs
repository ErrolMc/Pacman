using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager instance;

    [SerializeField] PanelID initialPanel;

    // caches
    Panel currentPanel;
    Panel[] panelList;

    public PanelID CurrentPanel { get { return currentPanel.panelID; } }

    void Awake()
    {
        instance = this;

        // get the references to the panels and show the initial panel
        panelList = GetComponentsInChildren<Panel>(true);
        ShowPanel(initialPanel, true);
    }

    /// <summary>
    /// Method to show a new panel, hiding any panels that are currently active
    /// </summary>
    /// <param name="panel">The panelID of the panel to show</param>
    /// <param name="forceCloseOthers">If we want to force close other panels</param>
    public void ShowPanel(PanelID panel, bool forceCloseOthers = false)
    {
        for (int i = 0; i < panelList.Length; i++)
        {
            // if we found the panel we want to go to
            if (panelList[i].panelID == panel)
            {
                panelList[i].gameObject.SetActive(true);    // enable the object
                panelList[i].isActive = true;               // set the active state
                panelList[i].OnShow();                      // call the panels OnShow method

                currentPanel = panelList[i];                // set a reference to the currentPanel
            }
            else if (panelList[i].isActive || forceCloseOthers)
            {
                panelList[i].gameObject.SetActive(false);   // disable the object

                // the force bool is just to disable panels and not call the OnHide()
                if (panelList[i].isActive)                  
                {
                    panelList[i].isActive = false;          // set the active state
                    panelList[i].OnHide();                  // call the panels onHide method
                }
            }
        }
    }
}



