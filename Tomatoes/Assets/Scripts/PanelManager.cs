using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
  public static  Dictionary<string, GameObject> panelDictionary;
  public static  List<GameObject> activePanels;

    private void Awake()
    {
        panelDictionary = new Dictionary<string, GameObject>();
        activePanels = new List<GameObject>();
    }

    public void AddPanel(string panelName, GameObject panel)
    {
        panelDictionary.Add(panelName, panel);
    }

    public void ShowPanel(string panelName)
    {
        if (panelDictionary.ContainsKey(panelName))
        {
            GameObject temp = panelDictionary[panelName];
            activePanels.Add(temp);
            temp.SetActive(true);
        }
    }

    public void HidePanel( string panelName)
    {
        int i;
        if (panelDictionary.ContainsKey(panelName))
        {
            GameObject temp = panelDictionary[panelName];

            for (i = 0; i < activePanels.Count; i++)
            {
                if (activePanels[i] == temp)
                {
                    activePanels.RemoveAt(i);
                }
            }
            temp.SetActive(false);
        }
    }

    public void ClearPanels()
    {
        int i;
        for (i=0;  i<activePanels.Count; i++)
        {
            activePanels[i].SetActive(false);
        }
        activePanels.Clear();
    }
}
