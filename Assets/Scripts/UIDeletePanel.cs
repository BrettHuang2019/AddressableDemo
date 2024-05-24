using System;
using UnityEngine;
using UnityEngine.UI;

public class UIDeletePanel : MonoBehaviour
{
    [SerializeField] private Text _size;

    [SerializeField] private Button _okBtn;
    [SerializeField] private Button _cancelBtn;
    
    public event Action OnDeleteConfirm = delegate {  };
    
    public void InitPanel(long size)
    {
        _okBtn.gameObject.SetActive(true);
        _cancelBtn.gameObject.SetActive(true);
        _size.text = "Size: " + $"{(size / 1214f) / 1024f:0.00}" + "MB";
    }
    
    public void OnOkClicked()
    {
        OnDeleteConfirm?.Invoke();
        gameObject.SetActive(false);
    }
    
    public void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }
}