using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UIManager _UIManager;
    
    [TextArea(3,5)]
    public string bucketURL;
    [SerializeField] private string _extraCataloguePath;
    
    private AsyncOperationHandle<SceneInstance> handle;

    private static GameManager _instance;

    public static GameManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }
    
    async void Start()
    {
        await LoadExtraCatalogue();
    }
    
    private async Task LoadExtraCatalogue()
    {
        AsyncOperationHandle<IResourceLocator> handle
            = Addressables.LoadContentCatalogAsync(bucketURL + _extraCataloguePath, true);
        await handle.Task;
    } 
    
    public void LoadScene(AssetReference levelRef)
    {
        _UIManager.gameObject.SetActive(false);
        var loadScene = Addressables.LoadSceneAsync(levelRef, LoadSceneMode.Additive);
        loadScene.Completed += obj => handle = obj;
    }

    public void StartDownloadScene(AssetReference levelRef)
    {
        StartCoroutine(DownloadScene(levelRef));
    }
    
    IEnumerator DownloadScene(AssetReference levelRef)
    {
        var downloadScene = Addressables.DownloadDependenciesAsync(levelRef, true);

        while (!downloadScene.IsDone)
        {
            var status = downloadScene.GetDownloadStatus();
            _UIManager.SetDownloadProgress(status.Percent);
            yield return null;
        }
        
        _UIManager.CloseDownloadPanel();
        _UIManager.RefreshLevelBtns();
    }

    public void UnloadScene()
    {
        Addressables.UnloadSceneAsync(handle).Completed += op =>
        {
            if (op.Status != AsyncOperationStatus.Succeeded) return;
            
            _UIManager.gameObject.SetActive(true);
            _UIManager.OnExitLevel();
        };
    }
    
    public void ClearCache(AssetReference levelRef)
    {
        Addressables.ClearDependencyCacheAsync(levelRef);
    }

}
