/*
 * UIStarFXManager.cs (Simplified Version)
 * ------------------------------------------------------------
 * 역할
 *  - UI 상에서 클릭 지점(또는 특정 UI 요소 중심)에서
 *    별 아이콘을 위로 솟구치게 한 뒤 자연스럽게 낙하시키는 연출을 수행합니다.
 *  - DOTween을 활용하여 상승, 하강, 회전, 페이드 아웃을 함께 처리합니다.
 *
 * 특징
 *  - Canvas 좌표계 기반으로 작동하며, Overlay / ScreenSpace-Camera 모두 지원됩니다.
 *  - 오브젝트 풀(starPool)을 사용하여 성능 부담 없이 반복 방출이 가능합니다.
 *  - 코드를 단순화하여 사용자가 별도의 옵션 없이 쉽게 호출할 수 있습니다.
 *
 * 사용 예시
 *  - UIStarFXManager.Instance.PlayAtTarget(myButtonRect);
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIStarFXManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RectTransform canvasRect;      // 상위 Canvas의 RectTransform
    [SerializeField] Camera uiCamera;               // ScreenSpace-Camera 모드라면 할당, Overlay면 null
    [SerializeField] List<RectTransform> starPool;  // 미리 배치된 비활성 별 오브젝트들 (Image 포함 권장)

    [Header("Emission Settings")]
    [SerializeField] int emitCount = 7;             // 한 번에 방출할 별 개수
    [SerializeField] Vector2 xRange = new Vector2(-260f, 260f); // 좌우 흩뿌림 범위
    [SerializeField] Vector2 upRange = new Vector2(380f, 620f); // 상승 높이 범위
    [SerializeField] Vector2 fallRange = new Vector2(720f, 980f);// 낙하 거리 범위

    [Header("Timing & Motion")]
    [SerializeField] float totalDuration = 1.25f;   // 전체 연출 시간
    [SerializeField] float apexRatio = 0.42f;       // 상승 비율
    [SerializeField] Vector2 startScaleRange = new Vector2(0.45f, 0.9f);
    [SerializeField] Vector2 spinRange = new Vector2(-720f, 720f); // 회전 각도 범위
    [SerializeField] AnimationCurve easeUp = null;  // 상승 구간 이징
    [SerializeField] AnimationCurve easeDown = null;// 하강 구간 이징

    /// <summary>
    /// 지정한 RectTransform 중심에서 별 이펙트를 방출합니다.
    /// </summary>
    public void PlayAtTarget(RectTransform target)
    {
        // 타겟 중심점을 월드 → 스크린 → 로컬로 변환
        Vector3 worldPos = target.TransformPoint(target.rect.center);
        var screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPos);

        int spawned = 0;
        for (int i = 0; i < starPool.Count && spawned < emitCount; i++)
        {
            var star = starPool[i];
            if (star.gameObject.activeSelf) continue;

            var parent = (RectTransform)star.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent, screenPos, uiCamera, out var localForParent);

            star.anchoredPosition = localForParent;
            LaunchOne(star, localForParent);
            spawned++;
        }
    }

    /// <summary>
    /// 단일 별을 발사하고 상승/낙하/페이드/회전을 DOTween 시퀀스로 처리합니다.
    /// </summary>
    void LaunchOne(RectTransform star, Vector2 origin)
    {
        star.gameObject.SetActive(true);
        star.anchoredPosition = origin;

        // 시작 크기와 색상 초기화
        float s = Random.Range(startScaleRange.x, startScaleRange.y);
        star.localScale = Vector3.one * s;

        var img = star.GetComponent<Image>();
        if (img != null)
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);

        // 궤적 계산
        float dx = Random.Range(xRange.x, xRange.y);
        float up = Random.Range(upRange.x, upRange.y);
        float fall = Random.Range(fallRange.x, fallRange.y);

        Vector2 apex = origin + new Vector2(dx * 0.5f, up);
        Vector2 target = origin + new Vector2(dx, -fall);

        float t1 = Mathf.Clamp01(apexRatio) * totalDuration;
        float t2 = totalDuration - t1;

        // 상승/하강 트윈 구성
        var seq = DOTween.Sequence();

        var upTween = star.DOAnchorPos(apex, t1)
            .SetEase(easeUp != null ? Ease.INTERNAL_Custom : Ease.OutQuad);
        if (easeUp != null) upTween.SetEase(easeUp);

        var downTween = star.DOAnchorPos(target, t2)
            .SetEase(easeDown != null ? Ease.INTERNAL_Custom : Ease.InQuad);
        if (easeDown != null) downTween.SetEase(easeDown);

        seq.Append(upTween).Append(downTween);

        // 회전 및 페이드 병행
        float spin = Random.Range(spinRange.x, spinRange.y);
        seq.Join(star.DORotate(new Vector3(0, 0, spin), totalDuration, RotateMode.FastBeyond360));

        if (img != null)
            seq.Join(img.DOFade(0f, totalDuration));

        // 완료 시 복귀
        seq.OnComplete(() =>
        {
            star.gameObject.SetActive(false);
            star.rotation = Quaternion.identity;
        });
    }
}
