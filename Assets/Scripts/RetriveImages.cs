using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class RetriveImages : MonoBehaviour
{
    Coroutine requestCoroutine;
    public class Character
    {
        public string Id { get; set; }
        public string CharacterName { get; set; }
        public string ImageURL { get; set; }
    }

    
    
    public Character CharacterList {get;set;}
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
                List<Character> charaterList = JsonConvert.DeserializeObject<List<Character>>(webRequest.downloadHandler.text);
                if(charaterList is not null)
                {
                    Game.CharacterList = charaterList;
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
                Game.Instance.LoadingErrorLog.text = "No esta activado el servidor o el puerto ingresado es el incorreto.";
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
