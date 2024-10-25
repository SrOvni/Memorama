using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class RetriveImages : MonoBehaviour
{
    Coroutine requestCoroutine;
    public class ImageList
    {
        public List<string> uri { get; set; }
    }
    
    
    public ImageList ImagesList {get;set;}
    private string URL = "http://localhost:4001/images/";
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch(webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                Debug.Log("Succesfull conection");
                List<string> uris = JsonConvert.DeserializeObject<List<string>>(webRequest.downloadHandler.text);
                if(uris is not null)
                {
                    Game.Uries = uris;
                }else{
                    Game.AttempedToGetUris = true;
                    Debug.LogWarning("Uris weren't obtained");
                }
                break;
                case UnityWebRequest.Result.InProgress:
                Debug.Log("In Progress");
                break;
                case UnityWebRequest.Result.ConnectionError:
                Debug.LogError(webRequest.error);
                Debug.Log("Conection Error");
                break;
                default:
                Debug.Log("Something went wrong");
                break;
            }
        }

    }
    private void OnEnable() {
        requestCoroutine = StartCoroutine(GetRequest(URL));
    }
    private void OnDisable() {
        StopCoroutine(requestCoroutine);
    }
}
