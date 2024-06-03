using System;
using UnityEngine;
using UnityEngine.UI;

public class UIDownloadPanel : MonoBehaviour
{
    [SerializeField] private Text _size;
    [SerializeField] private Text _percentText;
    [SerializeField] private Slider _progressbar;

    [SerializeField] private Button _okBtn;
    [SerializeField] private Button _cancelBtn;

    public float Progress
    {
        get => _progressbar.value;
        set
        {
            _progressbar.value = value;
            _percentText.text = $"{value * 100:0.0}" + "%";
        }
    }


    public event Action OnDownloadStart = delegate {  };

    public void InitPanel(long size)
    {
        _okBtn.gameObject.SetActive(true);
        _cancelBtn.gameObject.SetActive(true);
        
        _percentText.text = $"{0 * 100:0.0}" + "%";
        _progressbar.value = 0;
        _size.text = "Size: " + $"{(size / 1214f) / 1024f:0.00}" + "MB";
    }

    public void OnOkClicked()
    {
        _okBtn.gameObject.SetActive(false);
        _cancelBtn.gameObject.SetActive(false);
        OnDownloadStart?.Invoke();
    }

    public void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }
}
