/*
 * StarSpawnOnClick.cs (Simplified Version)
 * ------------------------------------------------------------
 * 역할
 *  - 버튼 클릭 시, 해당 UI 오브젝트의 중심에서 UIStarFXManager를 통해 별 이펙트를 방출합니다.
 *  - 인스펙터에서 UIStarFXManager를 지정해 두면 자동으로 연동됩니다.
 *  - Button 컴포넌트가 존재할 경우, Awake()에서 자동으로 클릭 리스너를 등록합니다.
 *
 * 특징
 *  - UIStarFXManager와 짝을 이루는 단순한 트리거 스크립트입니다.
 *  - OriginMode를 제거하여 코드 가독성과 사용 편의성을 높였습니다.
 *  - UI Button뿐 아니라 임의의 UI 요소에서도 OnClick 이벤트로 연결해 호출할 수 있습니다.
 */

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class StarSpawnOnClick : MonoBehaviour
{
    [Header("FX Manager Reference")]
    [Tooltip("별 이펙트를 재생할 UIStarFXManager를 지정합니다.")]
    public UIStarFXManager fx;

    void Awake()
    {
        // 버튼이 있을 경우 자동으로 클릭 이벤트에 연결합니다.
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(Spawn);
    }

    /// <summary>
    /// 현재 UI 오브젝트의 중심 지점에서 별 이펙트를 방출합니다.
    /// </summary>
    public void Spawn()
    {
        if (fx == null) return;

        fx.PlayAtTarget((RectTransform)transform);
    }
}
