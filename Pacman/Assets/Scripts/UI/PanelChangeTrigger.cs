using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that allows a UI button to navigate to another panel without having to write code for it
/// </summary>
public class PanelChangeTrigger : MonoBehaviour
{
    [SerializeField] PanelID panelID;

    /// <summary>
    /// Calls the PanelManager to change the panel to the selected panel id
    /// </summary>
    public void TriggerChange()
    {
        PanelManager.instance.ShowPanel(panelID);
    }
}
