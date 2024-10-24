using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Sprite faceSprite;
    public Sprite FaceSprite{get{return faceSprite;}set  {faceSprite = value;}}
    [SerializeField] private Sprite backsprite;
    public Sprite backSprite{get{return backsprite;} set {backsprite = value;}}
    private Image _currentImage;
    public Image CurrentImage{get{return _currentImage;}set {_currentImage = value;}}
    private bool endOfFlipAnimation = false;
    [SerializeField] private UnityEvent OnFlipCard;
    private void Awake() {

        _currentImage = GetComponent<Image>();

        _currentImage.type = Image.Type.Simple;
        _currentImage.preserveAspect = true;
        _currentImage.sprite = backSprite;
    }
    
    private bool facingFront = false;
    
    public void FlipCard()
    {
        OnFlipCard?.Invoke();
        FlipAnimation();
    }
    private void FlipAnimation()
    {
        if(facingFront)
        {
            transform.DOScaleX(0,0.2f).OnComplete(()=> 
            {
                _currentImage.sprite = backSprite;

                transform.DOScaleX(1,0.2f);
            });
            facingFront = false;
        }else{

            transform.DOScaleX(0,0.2f).OnComplete(()=> 
            {
                _currentImage.sprite = faceSprite;

                transform.DOScaleX(-1,0.2f);
            });
            facingFront = true;
        }
    }
    private void ChangeImage(Sprite sprite)
    {
        _currentImage.sprite = sprite;
    }
    public void RestartCard()
    {
        if(facingFront)
        {

        }
    }

    public void DisableButton()
    {
        gameObject.GetComponent<Button>().enabled = false;
    }
}
