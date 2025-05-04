using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public static class DOTweenHelper
{
    #region 3D Transform Animations
    
    /// <summary>
    /// Smoothly moves a Transform in 3D space
    /// </summary>
    public static Tween DoMove3D(this Transform transform, Vector3 targetPosition, float duration, Ease ease = Ease.OutQuad)
    {
        return transform.DOMove(targetPosition, duration).SetEase(ease);
    }
    
    /// <summary>
    /// Moves a Transform on a bezier path in 3D
    /// </summary>
    public static Tween DoPath3D(this Transform transform, Vector3[] waypoints, float duration, PathType pathType = PathType.CatmullRom)
    {
        return transform.DOPath(waypoints, duration, pathType);
    }
    
    /// <summary>
    /// Smoothly rotates a Transform in 3D
    /// </summary>
    public static Tween DoRotate3D(this Transform transform, Vector3 targetRotation, float duration, RotateMode mode = RotateMode.Fast)
    {
        return transform.DORotate(targetRotation, duration, mode);
    }
    
    /// <summary>
    /// Look at target transform with smooth rotation
    /// </summary>
    public static Tween DoLookAt(this Transform transform, Vector3 lookAtPosition, float duration)
    {
        Vector3 targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position).eulerAngles;
        return transform.DORotate(targetRotation, duration);
    }
    
    /// <summary>
    /// Jump animation in 3D space
    /// </summary>
    public static Tween DoJump3D(this Transform transform, Vector3 endPosition, float jumpPower, int numJumps, float duration)
    {
        return transform.DOJump(endPosition, jumpPower, numJumps, duration);
    }

    #endregion

    #region 2D Transform Animations
    
    /// <summary>
    /// Moves a Transform in 2D space
    /// </summary>
    public static Tween DoMove2D(this Transform transform, Vector2 targetPosition, float duration, Ease ease = Ease.OutQuad)
    {
        return transform.DOMove(new Vector3(targetPosition.x, targetPosition.y, transform.position.z), duration).SetEase(ease);
    }
    
    /// <summary>
    /// 2D jump animation
    /// </summary>
    public static Tween DoJump2D(this Transform transform, Vector2 endPosition, float jumpPower, int numJumps, float duration)
    {
        Vector3 end = new Vector3(endPosition.x, endPosition.y, transform.position.z);
        return transform.DOJump(end, jumpPower, numJumps, duration);
    }
    
    /// <summary>
    /// Rotates a sprite around the Z axis
    /// </summary>
    public static Tween DoRotate2D(this Transform transform, float targetAngle, float duration, Ease ease = Ease.OutQuad)
    {
        return transform.DORotate(new Vector3(0, 0, targetAngle), duration).SetEase(ease);
    }
    
    /// <summary>
    /// Moves on a 2D bezier path
    /// </summary>
    public static Tween DoPath2D(this Transform transform, Vector2[] waypoints, float duration, PathType pathType = PathType.CatmullRom)
    {
        Vector3[] path = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            path[i] = new Vector3(waypoints[i].x, waypoints[i].y, transform.position.z);
        }
        return transform.DOPath(path, duration, pathType);
    }

    #endregion

    #region Sprite Animations
    
    /// <summary>
    /// Fades SpriteRenderer alpha
    /// </summary>
    public static Tween DoFadeSprite(this SpriteRenderer spriteRenderer, float endValue, float duration)
    {
        return spriteRenderer.DOFade(endValue, duration);
    }
    
    /// <summary>
    /// Changes SpriteRenderer color
    /// </summary>
    public static Tween DoColorSprite(this SpriteRenderer spriteRenderer, Color endValue, float duration)
    {
        return spriteRenderer.DOColor(endValue, duration);
    }
    
    /// <summary>
    /// Flips sprite horizontally with animation
    /// </summary>
    public static Sequence DoFlipX(this SpriteRenderer spriteRenderer, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.transform.DOScaleX(-spriteRenderer.transform.localScale.x, duration));
        return sequence;
    }

    #endregion

    #region Camera Animations
    
    /// <summary>
    /// Shakes camera in 3D space
    /// </summary>
    public static Tween DoCameraShake3D(this Camera camera, float duration, float strength = 1f, int vibrato = 10)
    {
        return camera.DOShakePosition(duration, strength, vibrato);
    }
    
    /// <summary>
    /// Shakes camera in 2D space (only X and Y)
    /// </summary>
    public static Tween DoCameraShake2D(this Camera camera, float duration, float strength = 1f, int vibrato = 10)
    {
        // Use relative shake with fadeOut=true to prevent permanent camera displacement
        return camera.DOShakePosition(duration, new Vector3(strength, strength, 0), vibrato, 90, true, DG.Tweening.ShakeRandomnessMode.Harmonic);
    }
    
    /// <summary>
    /// Smoothly changes camera field of view
    /// </summary>
    public static Tween DoFieldOfView(this Camera camera, float endValue, float duration)
    {
        return camera.DOFieldOfView(endValue, duration);
    }
    
    
    //<summary>
    // Shakes the transform
    //</summary>
    public static void Shake(Transform transform)
    {
        transform.DOShakePosition(0.5f, 0.5f, 10, 90, false, true);
    }
    
    /// <summary>
    /// Smoothly changes orthographic size (for 2D cameras)
    /// </summary>
    public static Tween DoOrthoSize(this Camera camera, float endValue, float duration)
    {
        return camera.DOOrthoSize(endValue, duration);
    }

    #endregion

    #region UI Animations
    
    /// <summary>
    /// Creates a bouncy button effect
    /// </summary>
    public static Sequence DoButtonPop(this Button button, float duration = 0.5f, float scaleFactor = 1.2f)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(button.transform.DOScale(Vector3.one * scaleFactor, duration * 0.5f));
        sequence.Append(button.transform.DOScale(Vector3.one, duration * 0.5f));
        return sequence;
    }
    
    /// <summary>
    /// Slides UI element from position
    /// </summary>
    public static Tween DoSlideFrom(this RectTransform rectTransform, Vector2 startPosition, float duration, Ease ease = Ease.OutQuad)
    {
        rectTransform.anchoredPosition = startPosition;
        return rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(ease);
    }
    
    /// <summary>
    /// Types text character by character
    /// </summary>
    public static Tween DoTypewriter(this TMP_Text text, string message, float duration)
    {
        text.text = "";
        return DOTween.To(() => text.text, x => text.text = x, message, duration);
    }

    #endregion

    #region Effect Sequences
    
    /// <summary>
    /// Creates a floating effect for both 2D and 3D objects
    /// </summary>
    public static Sequence DoFloat(this Transform transform, float amplitude = 0.5f, float duration = 1f)
    {
        Sequence sequence = DOTween.Sequence();
        Vector3 startPos = transform.position;
        sequence.Append(transform.DOMoveY(startPos.y + amplitude, duration).SetEase(Ease.InOutSine));
        sequence.Append(transform.DOMoveY(startPos.y, duration).SetEase(Ease.InOutSine));
        sequence.SetLoops(-1);
        return sequence;
    }
    
    /// <summary>
    /// Creates a pulsing effect for both 2D and 3D objects
    /// </summary>
    public static Sequence DoPulse(this Transform transform, float scaleFactor = 1.1f, float duration = 0.5f)
    {
        Sequence sequence = DOTween.Sequence();
        Vector3 originalScale = transform.localScale;
        sequence.Append(transform.DOScale(originalScale * scaleFactor, duration * 0.5f));
        sequence.Append(transform.DOScale(originalScale, duration * 0.5f));
        return sequence;
    }
    
    /// <summary>
    /// Creates a shockwave effect expanding outward
    /// </summary>
    public static Sequence DoShockwave(this Transform transform, float radius = 5f, float duration = 1f)
    {
        Sequence sequence = DOTween.Sequence();
        transform.localScale = Vector3.zero;
        sequence.Append(transform.DOScale(Vector3.one * radius, duration).SetEase(Ease.OutQuad));
        sequence.Join(transform.GetComponent<Renderer>()?.material.DOFade(0, duration));
        return sequence;
    }

    #endregion

    /// <summary>
    /// Sets common tween parameters
    /// </summary>
    public static Tween SetTweenParams(this Tween tween, bool useUnscaledTime = false, bool autoKill = true, int loops = 0)
    {
        return tween.SetUpdate(useUnscaledTime)
                   .SetAutoKill(autoKill)
                   .SetLoops(loops);
    }
    
    /// <summary>
    /// Pauses all tweens affecting a specific target
    /// </summary>
    public static void PauseAllTweens(object target)
    {
        DOTween.Pause(target);
    }
    
    /// <summary>
    /// Kills all tweens affecting a specific target
    /// </summary>
    public static void KillAllTweens(object target, bool complete = false)
    {
        DOTween.Kill(target, complete);
    }
    
    /// <summary>
    /// Rewinds all tweens affecting a specific target
    /// </summary>
    public static void RewindAllTweens(object target)
    {
        DOTween.Rewind(target);
    }
}