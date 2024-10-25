
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    private Coroutine imageCoroutine;
    [SerializeField] private TMP_Text loadingErrorLog;
    public TMP_Text LoadingErrorLog{get{return loadingErrorLog;} set{loadingErrorLog = value;}}
    [SerializeField] private GameObject cardsGroup;
    [SerializeField] private List<Vector2> randomCardsPositions;
    [SerializeField] private List<int> nonRepetingNumbers;
    public static List<string> Uries { get; set; }
    public static bool AttempedToGetUris { get { return attempedToGetUris; } set { attempedToGetUris = value; } }
    private static bool attempedToGetUris = false;
    [SerializeField] private Vector2 spritePivot = new Vector2(0.5f, 0.5f);
    [SerializeField] private float pixelsPerUnit = 100;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    private bool imageAssigmentFinished = false;
    [SerializeField] private GameObject LoadingCanvas;
    [SerializeField] private GameObject LoseCanvas;
    [SerializeField] private GameObject WinCanvas;
    [SerializeField] private Card card1 = null;
    [SerializeField] private Card card2 = null;
    [SerializeField] private int numberOfPairs = 0;

    [SerializeField] private Slider loadingSlider;

    #region returnToFalse

    private bool playerWinGame = false;
    private bool gamestartanimationEnded = false;
    private bool endOfGameAniamtionEnded = false;
    private bool buttonsEnabled = false;
    private bool randomAssigmentFinished = false;
    private bool finishedFliping = false;
    private bool buttonsDisable = false;
    private bool loadingAnimationFinished = false;

    #endregion
    [SerializeField] private float timeToFlipBackCards = 1;
    [SerializeField] private float timeToShowPlayerCards = 3;
    [SerializeField] private float timeToFlipNextCard = .2f;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent OnRestartGameEvent;
    [SerializeField] private UnityEvent OnEndOfTheGameEvent;
    [SerializeField] private UnityEvent OnStartOfTheGameEvent;
    [SerializeField] private UnityEvent OnCorrectPair;
    [SerializeField] private UnityEvent OnWrongPair;
    [SerializeField] private UnityEvent OnWinGame;
    [SerializeField] private UnityEvent OnLoseGame;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        randomCardsPositions = new List<Vector2>(cardsGroup.transform.childCount);
        ListWithRandomNonRepetingNumbers();
        GetRandomPositions();

    }

    public void FindPair(Card card)//Main method to using during game
    {
        if (card1 == card) return;
        if (card2 is not null) return;
        if (card1 is null)
        {
            card1 = card;
            card1.FlipCard();

            return;
        }
        else if (card2 is null)
        {
            card2 = card;
            card2.FlipCard();
        }
        if (card1.FaceSprite.name == card2.FaceSprite.name)
        {
            OnCorrectPair?.Invoke();
            card1.GetComponent<Button>().enabled = false;
            card1 = null;
            card2.GetComponent<Button>().enabled = false;
            card2 = null;
            numberOfPairs++;
        }
        else if (card1 is not null && card2 is not null)
        {
            OnWrongPair?.Invoke();
            StartCoroutine(WrongPairAniamtion());
        }

    }
    IEnumerator WrongPairAniamtion()
    {
        yield return new WaitForSeconds(timeToFlipBackCards);
        card1.FlipCard();
        card1 = null;
        card2.FlipCard();
        card2 = null;
        yield return null;
    }

    private void Start()
    {
        GameManager.Instance.StopMainAudio();
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        Debug.Log("Loading game...");
        yield return new WaitUntil(() => imageAssigmentFinished);
        loadingSlider.DOValue(1,1).OnComplete(() =>
            {
                loadingAnimationFinished = true;
            }
        );
        yield return new WaitUntil(() => loadingAnimationFinished);
        yield return new WaitForSeconds(timeToFlipNextCard);
        LoadingCanvas.SetActive(false);
        StartCoroutine(OnStartGame());

    }
    public void StartGame()//EventUse
    {
        GameManager.Instance.PlayMainAudio();
        StartCoroutine(DisableMemoryGameButton());
        OnStartOfTheGameEvent?.Invoke();
        StartCoroutine(OnStartGame());
    }

    private IEnumerator OnStartGame()
    {
        Debug.Log("Game started");
        StartCoroutine(FlipAllCards());
        yield return new WaitUntil(() => finishedFliping);
        finishedFliping = false;
        StartCoroutine(RandomizeCardPosition());
        yield return new WaitUntil(() => randomAssigmentFinished);
        yield return new WaitForSeconds(timeToShowPlayerCards);
        StartCoroutine(FlipAllCards());
        yield return new WaitUntil(() => finishedFliping);
        StartCoroutine(GameStartAnimation());
        yield return new WaitUntil(() => gamestartanimationEnded);
        GameManager.Instance.StartGame = true;
        StartCoroutine(EnableMemoryGameButtons());
        yield return new WaitUntil(() => buttonsEnabled);
        Timer.Instance.StartTimer();
        yield return null;
    }

    public void EndGame()
    {
        OnEndOfTheGameEvent?.Invoke();
        StartCoroutine(OnEndGame());
    }

    private IEnumerator OnEndGame()
    {
        Debug.Log("Game ended");
        StartCoroutine(DisableMemoryGameButton());
        Timer.Instance.StopTimer();

        StartCoroutine(DisableMemoryGameButton());
        yield return new WaitUntil(() => buttonsDisable);


        StartCoroutine(EndOfGameAnimation());

        GameManager.Instance.StartGame = false;

        yield return new WaitUntil(() => endOfGameAniamtionEnded);

        //Falta deshabilitar los botones de las cartas
        yield return null;
    }

    private IEnumerator OnRestartGame()
    {
        Debug.Log("Restarting Game...");
        GameManager.Instance.StartGame = false;

        StartCoroutine(GameRestartAnimation());
        yield return new WaitUntil(() => endOfGameAniamtionEnded);
        Timer.Instance.RestartTime();
        StartGame();
        //Falta deshabilitar los botones de las cartas
        yield return null;
    }

    private IEnumerator AssignImages()
    {
        yield return new WaitUntil(() => Uries is not null || attempedToGetUris);
        if (attempedToGetUris)
            yield break;
        for (int i = 0; i < GameManager.Instance.ImagesRequired; i++)
        {
            UnityWebRequest uri = UnityWebRequestTexture.GetTexture(Uries[i]);
            yield return uri.SendWebRequest();
            if (uri.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(uri.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uri);
                Rect rect = cardsGroup.transform.GetChild(i).GetComponent<RectTransform>().rect;


                Card image1 = cardsGroup.transform.GetChild(i).GetComponent<Card>();
                image1.FaceSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), spritePivot, pixelsPerUnit);
                image1.FaceSprite.name = $"{i}";
                Card image2 = cardsGroup.transform.GetChild(i + GameManager.Instance.ImagesRequired).GetComponent<Card>();
                image2.FaceSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), spritePivot, pixelsPerUnit);
                image2.FaceSprite.name = $"{i}";

            }

        }
        
        imageAssigmentFinished = true;
    }
    private void GetRandomPositions()
    {
        
        randomCardsPositions.Clear();
        for (int i = 0; i < cardsGroup.transform.childCount; i++)
        {
            RectTransform rectTransform = cardsGroup.transform.GetChild(nonRepetingNumbers[i]).GetComponent<RectTransform>();
            randomCardsPositions.Add(rectTransform.position);
        }
    }
    private void ListWithRandomNonRepetingNumbers()
    {
        nonRepetingNumbers.Clear();
        nonRepetingNumbers = new List<int>();  // Initialize an empty list

        // Continue until we have as many numbers as there are children in cardsGroup
        while (nonRepetingNumbers.Count < cardsGroup.transform.childCount)
        {
            int randomNumber = Random.Range(0, cardsGroup.transform.childCount);  // Generate a random number

            // Check if the number already exists in the list
            if (!nonRepetingNumbers.Contains(randomNumber))
            {
                nonRepetingNumbers.Add(randomNumber);  // Add the random number if it is unique
            }
        }
    }

    IEnumerator RandomizeCardPosition()
    {
        gridLayoutGroup.enabled = false;
        for (int i = 0; i < cardsGroup.transform.childCount; i++)
        {
            cardsGroup.transform.GetChild(i).transform.DOMoveX(randomCardsPositions[i].x, 1);
            cardsGroup.transform.GetChild(i).transform.DOMoveY(randomCardsPositions[i].y, 1);
        }
        yield return new WaitForSeconds(timeToFlipBackCards);
        gridLayoutGroup.enabled = false;
        randomAssigmentFinished = true;
    }

    private void Update()
    {
        if (GameManager.Instance.StartGame)
        {
            if (numberOfPairs == GameManager.Instance.ImagesRequired)
            {
                playerWinGame = true;
                EndGame();
            }
            else if (Timer.Instance.TimeRemaining <= 0)
            {
                playerWinGame = false;
                EndGame();
            }

        }
    }

    IEnumerator EndOfGameAnimation()
    {

        if (playerWinGame)
        {
            
            OnWinGame?.Invoke();
            WinCanvas.SetActive(true);
            Debug.Log("Player Win");
        }
        else
        {
            LoseCanvas.SetActive(true);
            OnLoseGame?.Invoke();
            Debug.Log("Player lose");
        }
        endOfGameAniamtionEnded = true;
        yield return null;
    }

    IEnumerator GameRestartAnimation()
    {
        ListWithRandomNonRepetingNumbers();
        GetRandomPositions();
        for(int i = 0;i < cardsGroup.transform.childCount; i++)
        {
            var card = cardsGroup.transform.GetChild(i).GetComponent<Card>();
            if(card.FacingFront)
            {
                card.FlipCard();
            }
        }
        endOfGameAniamtionEnded = true;
        yield return null;
    }

    IEnumerator GameStartAnimation()
    {
        gamestartanimationEnded = true;
        yield return null;
    }

    IEnumerator EnableMemoryGameButtons()
    {
        for (int i = 0; i < cardsGroup.transform.childCount; i++)
        {
            cardsGroup.transform.GetChild(i).GetComponent<Button>().enabled = true;
        }
        buttonsEnabled = true;
        yield return null;
    }
    IEnumerator DisableMemoryGameButton()
    {
        for (int i = 0; i < cardsGroup.transform.childCount; i++)
        {
            cardsGroup.transform.GetChild(i).GetComponent<Button>().enabled = false;
        }
        buttonsDisable = true;
        yield return null;
    }

    public void RestartGame() //EventUse
    {
        OnRestartGameEvent?.Invoke();
        playerWinGame = false;
        gamestartanimationEnded = false;
        endOfGameAniamtionEnded = false;
        buttonsEnabled = false;
        randomAssigmentFinished = false;
        finishedFliping = false;
        buttonsDisable = false;
        card1 = null;
        card2 = null;
        numberOfPairs = 0;
        StartCoroutine(OnRestartGame());
    }

    IEnumerator FlipAllCards()
    {
        for (int i = 0; i < cardsGroup.transform.childCount; i++)
        {
            cardsGroup.transform.GetChild(i).GetComponent<Card>().FlipCard();
            yield return new WaitForSeconds(timeToFlipNextCard);
        }
        //yield return new WaitForSeconds(timeToFlipBackCards);
        finishedFliping = true;
    }

    private void OnEnable()
    {
        imageCoroutine = StartCoroutine(AssignImages());
    }

    private void OnDisable()
    {
        // Detener la coroutine cuando el objeto se deshabilita
        if (imageCoroutine != null)
        {
            StopCoroutine(imageCoroutine);
            imageCoroutine = null;
        }
    }

}