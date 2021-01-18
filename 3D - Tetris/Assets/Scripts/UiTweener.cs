using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public enum UIAnimationType
{
    Move,
    Scale,
    Fade
}
public enum UpdateMode
{
    Normal,
    UnscaledTime
}

public class UiTweener : MonoBehaviour
{
    [SerializeField] private UpdateMode updateMode;
    [SerializeField] private UIAnimationType animationType;
    [SerializeField] private Ease easeType = Ease.Linear;
    [SerializeField] private float duration;
    [SerializeField] private float delay;

    [SerializeField] private AnimationCurve customCurve;

    [SerializeField] private bool startPositionOffset;
    [SerializeField] private Vector3 from = Vector3.zero;
    [SerializeField] private Vector3 to = Vector3.one;

    [SerializeField] private bool loop;
    [SerializeField] private bool pingpong;

    [SerializeField] private bool ShowOnEnable;

    [SerializeField] private UnityEvent OnFinishEvent;

    private Tween _tweenObj;

    private void Awake()
    {
        if (!startPositionOffset)
        {
            switch (animationType)
            {
                case UIAnimationType.Move:
                    from = transform.localPosition;
                    break;
                case UIAnimationType.Scale:
                    from = gameObject.GetComponent<RectTransform>().localScale;

                    break;
                case UIAnimationType.Fade:
                    if (GetComponent<CanvasGroup>())
                        from.x = GetComponent<CanvasGroup>().alpha;
                    break;
            }
        }
    }

    public void OnEnable()
    {
        if (ShowOnEnable)
            Show();
    }

    public void Show()
    {
        HandleTween();
    }

    private void HandleTween()
    {
        switch (animationType)
        {
            case UIAnimationType.Move:
                Move();
                break;
            case UIAnimationType.Scale:
                Scale();
                break;
            case UIAnimationType.Fade:
                Fade();
                break;
        }

        if (easeType == Ease.Unset)
            _tweenObj.SetEase(customCurve);
        else
            _tweenObj.SetEase(easeType);

        _tweenObj.SetDelay(delay);

        if (loop)
        {
            LoopType loopType;
            if (pingpong)
                loopType = LoopType.Yoyo;
            else
                loopType = LoopType.Restart;

            _tweenObj.SetLoops(int.MaxValue, loopType);
        }
        else
        {
            if (pingpong)
            {
                _tweenObj.SetLoops(2, LoopType.Yoyo);
                _tweenObj.OnComplete(() => gameObject.SetActive(false));
            }
        }

        _tweenObj.SetUpdate(updateMode == UpdateMode.UnscaledTime);


        if (OnFinishEvent.GetPersistentEventCount() > 0)
            _tweenObj.OnComplete(() => OnFinishEvent.Invoke());
    }

    private void Move()
    {
        GetComponent<RectTransform>().anchoredPosition = from;

        _tweenObj = transform.DOLocalMove(to, duration);
    }
    private void Scale()
    {
        if (startPositionOffset)
            gameObject.GetComponent<RectTransform>().localScale = from;

        _tweenObj = transform.DOScale(to, duration);
    }
    private void Fade()
    {
        CanvasGroup canvasGroup;
        if ((canvasGroup = gameObject.GetComponent<CanvasGroup>()) == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();


        if (startPositionOffset)
            canvasGroup.alpha = from.x;

        _tweenObj = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, to.x, duration);
    }

    private void SwapDirection()
    {
        var temp = from;
        from = to;
        to = temp;
    }

    public void Disable()
    {
        SwapDirection();

        HandleTween();

        _tweenObj.OnComplete(() =>
        {
            SwapDirection();

            gameObject.SetActive(false);
        });
    }
}