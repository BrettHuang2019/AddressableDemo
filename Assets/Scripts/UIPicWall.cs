using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIPicWall : MonoBehaviour
{
    [SerializeField] private string _label = "Pic";
    [SerializeField] private Image _picPrefab;
    
    private Transform _content;
    async void Start()
    {
        _content = transform.Find("Viewport/Content");

        await LoadPicsAsync();
    }

    private async Task LoadPicsAsync()
    {
        var handle = Addressables.LoadAssetsAsync<Sprite>(_label, pic =>
        {
            Image newPic = Instantiate(_picPrefab, _content);
            newPic.sprite = pic;
        });
        await handle.Task;
    }
}
