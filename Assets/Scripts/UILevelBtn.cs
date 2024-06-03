using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.UI;

public class UILevelBtn : MonoBehaviour
{
    [SerializeField] private AssetReference _levelRef;
    [SerializeField] private GameObject _downloadIcon;
    [SerializeField] private Button _deleteBtn;
    
    public event Action<AssetReference,long> OnDownloadNeeded = delegate {  };
    public event Action<AssetReference,long> OnDeleteNeeded = delegate {  };

    private bool isLocal = false;

    private void Start()
    {
        RefreshBtn();
    }
    public async void RefreshBtn()
    {
        long size = await GetDownloadSizeAsync(_levelRef);
        _downloadIcon.SetActive(size > 0);
        _deleteBtn.gameObject.SetActive(size <= 0 && name!="LevelBtn1");
    }

    public async void OnLevelBtnClick()
    {
        long size = await GetDownloadSizeAsync(_levelRef);
        if (size<=0)
            GameManager.Instance.LoadScene(_levelRef);
        else
            OnDownloadNeeded?.Invoke(_levelRef, size);
    }
    
    /// <summary>
    /// Return the size of an asset reference that is still remote. If the asset is cached, it returns 0. Note: It does not work in test mode: Simulate Groups
    /// </summary>
    /// <param name="levelRef"></param>
    /// <returns></returns>
    private async Task<long> GetDownloadSizeAsync(AssetReference levelRef)
    {
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(levelRef);
        await getDownloadSize.Task;
        return getDownloadSize.Result;
    }
    
    /// <summary>
    /// Return a cached asset reference's size.
    /// </summary>
    /// <param name="levelRef"></param>
    /// <returns></returns>
    private async Task<long> GetCachedSizeAsync(AssetReference levelRef)
    {
        var locs = await Addressables.LoadResourceLocationsAsync(levelRef).Task;
        
        foreach (IResourceLocation loc in locs)
        {
            foreach (IResourceLocation resourceLocation in loc.Dependencies)
            {
                if (resourceLocation.Data is not AssetBundleRequestOptions data) continue;
                if (!Caching.IsVersionCached(new CachedAssetBundle(data.BundleName, Hash128.Parse(data.Hash))))
                    continue;
                    
                var id = Addressables.ResourceManager.TransformInternalId(resourceLocation);
                if (ResourceManagerConfig.IsPathRemote(id))
                    return data.BundleSize;
            }
        }
        return 0;
    }

    public async void OnDeleteBtnClicked()
    {
        long size = await GetCachedSizeAsync(_levelRef);
        OnDeleteNeeded?.Invoke(_levelRef, size);
    }
    
    
}
