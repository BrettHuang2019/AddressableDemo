using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

public class CatalogLoader : MonoBehaviour
{
    public string catalogURL;
    private string catalogName;

    private void Start()
    {
        LoadLatestExtraCatalog();
    }

    public async Task LoadLatestExtraCatalog()
    {
        string html = await GetHtmlContent(catalogURL);
        List<string> fileNames = ParseHtmlForFileNames(html);
        if (fileNames.Count<=0)
        {
            Debug.LogWarning("No Extra Catalog Found.");
            return;
        }
        
        string latestFile = GetLatestFile(fileNames);

        if (!string.IsNullOrEmpty(latestFile))
        {
            string url = catalogURL + latestFile;
            Debug.Log(url);
            var handle = Addressables.LoadContentCatalogAsync(url, true);
            await handle.Task;
        }
    }
    
    private async Task<string> GetHtmlContent(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                return www.downloadHandler.text;
            }
            else
            {
                Debug.LogError(www.error);
                return null;
            }
        }
    }
    
    private List<string> ParseHtmlForFileNames(string html)
    {
        var matches = Regex.Matches(html, @"href=""(catalog_\d{4}\.\d{2}\.\d{2}\.\d{2}\.\d{2}\.\d{2}\.json)");
        return matches.Cast<Match>().Select(m => m.Groups[1].Value).ToList();
    }
    
    private string GetLatestFile(List<string> fileNames)
    {
        DateTime latestDate = DateTime.MinValue;
        string latestFile = null;

        foreach (var fileName in fileNames)
        {
            var match = Regex.Match(fileName, @"catalog_(\d{4}\.\d{2}\.\d{2}\.\d{2}\.\d{2}\.\d{2})");
            if (match.Success)
            {
                DateTime fileDate = DateTime.ParseExact(match.Groups[1].Value, "yyyy.MM.dd.HH.mm.ss", null);
                if (fileDate > latestDate)
                {
                    latestDate = fileDate;
                    latestFile = fileName;
                }
            }
        }

        return latestFile;
    }
    
}