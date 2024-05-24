using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UILevelBtn> _uiLevelBtns;
    [SerializeField] private UIDownloadPanel _uiDownloadPanel;
    [SerializeField] private UIDeletePanel _uiDeletePanel;
    
    [SerializeField] private GameObject levelBtnsPanel;
    private AssetReference _currentLevel;

    private void Start()
    {
        _uiDownloadPanel.gameObject.SetActive(false);
        _uiDeletePanel.gameObject.SetActive(false);

        _uiDownloadPanel.OnDownloadStart += OnDownloadConfirmed;
        _uiDeletePanel.OnDeleteConfirm += OnDeleteConfirmed;
        
        foreach (UILevelBtn uiLevelBtn in _uiLevelBtns)
        {
            uiLevelBtn.OnDownloadNeeded += OpenDownloadPanel;
            uiLevelBtn.OnDeleteNeeded += OpenDeletePanel;
        }
    }

    public void RefreshLevelBtns()
    {
        foreach (UILevelBtn uiLevelBtn in _uiLevelBtns)
            uiLevelBtn.RefreshBtn();
    }
    
    public void OnExitLevel()
    {
        _uiDownloadPanel.gameObject.SetActive(false);
        _uiDeletePanel.gameObject.SetActive(false);
        levelBtnsPanel.SetActive(true);
    }
    
    public void OpenDownloadPanel(AssetReference levelRef, long size)
    {
        _currentLevel = levelRef;
        _uiDownloadPanel.gameObject.SetActive(true);
        _uiDownloadPanel.InitPanel(size);
    }
    
    public void CloseDownloadPanel()
    {
        _uiDownloadPanel.gameObject.SetActive(false);
    }

    public void SetDownloadProgress(float percent)
    {
        _uiDownloadPanel.Progress = percent;
    }
    
    public void OnDownloadConfirmed()
    {
        GameManager.Instance.StartDownloadScene(_currentLevel);
    }

    public void OpenDeletePanel(AssetReference levelRef, long size)
    {
        _currentLevel = levelRef;
        _uiDeletePanel.gameObject.SetActive(true);
        _uiDeletePanel.InitPanel(size);
    }

    public void OnDeleteConfirmed()
    {
        GameManager.Instance.ClearCache(_currentLevel);
        RefreshLevelBtns();
    }
}
