using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    private void Awake()
    {
        AdManager[] adManager = FindObjectsOfType<AdManager>();
        if (adManager.Length == 1)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        string gameID = "3415447";
        Advertisement.Initialize(gameID, false);        
    }

    public void ActiveAD()
    {
        Debug.Log("ready : " + Advertisement.IsReady());
        if (Advertisement.IsReady())
        {
            ShowOptions options = new ShowOptions { resultCallback = HandleShowResult };
            this.transform.GetChild(1).gameObject.SetActive(false);
            this.transform.GetChild(0).gameObject.SetActive(true);
            Advertisement.Show();
        }
        else
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                break;

            case ShowResult.Skipped:
                break;

            case ShowResult.Failed:
                break;
        }
    }

}
