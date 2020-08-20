using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelChangeTrigger : MonoBehaviour
{
    [SerializeField] PanelID panelID;

    public void TriggerChange()
    {
        PanelManager.instance.ShowPanel(panelID);
    }
}
