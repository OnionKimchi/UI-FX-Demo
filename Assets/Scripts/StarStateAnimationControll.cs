/*
 * StarStateAnimationControll.cs
 * ------------------------------------------------------------
 * 역할
 *  - 별 아이콘의 상태(켜짐 / 꺼짐 / 반짝임)를 애니메이터 트리거를 통해 전환합니다.
 *  - 외부 UI나 버튼 입력으로 호출되어, 해당 상태에 맞는 애니메이션을 재생하도록 설계되었습니다.
 *
 * 의도
 *  - UIStarFXManager가 별을 시각적으로 “방출”하는 연출을 담당한다면,
 *    본 스크립트는 개별 별 오브젝트의 “상태 변화”를 표현하는 보조 컨트롤러입니다.
 *  - Animator 트리거 기반으로 상태를 관리하여 코드에서 직접 애니메이션 재생을 제어하지 않고,
 *    Unity의 Animation 전이(State Machine) 시스템을 적극 활용합니다.
 *
 * 사용 예시
 *  - StarStateAnimationControll controll;
 *    controll.SetStarState(StarState.Twinkle);  // 반짝임 애니메이션 재생
 */

using UnityEngine;

/// <summary>
/// 별의 상태를 정의한 열거형입니다.
/// On(켜짐), Off(꺼짐), Twinkle(반짝임)의 세 가지 상태를 가집니다.
/// </summary>
public enum StarState
{
    On = 1,        // 켜짐 상태
    Off = -1,      // 꺼짐 상태
    Twinkle = 0    // 반짝임 상태
}

/// <summary>
/// 별 오브젝트의 Animator 트리거를 제어하는 클래스입니다.
/// 외부 입력(버튼 클릭 등)에 따라 상태를 변경하고,
/// Animator의 트리거를 통해 해당 상태의 애니메이션을 재생합니다.
/// </summary>
public class StarStateAnimationControll : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator starAnimator;
    // 별 오브젝트에 연결된 Animator 컴포넌트 참조.
    // 각 상태 전환은 Animator Controller 내의 트리거를 통해 수행됩니다.

    /// <summary>
    /// 별의 상태를 설정하고 Animator 트리거를 발동합니다.
    /// </summary>
    /// <param name="state">설정할 별 상태 (On / Off / Twinkle)</param>
    public void SetStarState(StarState state)
    {
        if (starAnimator == null) return;

        switch (state)
        {
            case StarState.On:
                starAnimator.SetTrigger("SetOn");        // “켜짐” 상태 트리거 발동
                break;
            case StarState.Off:
                starAnimator.SetTrigger("SetOff");       // “꺼짐” 상태 트리거 발동
                break;
            case StarState.Twinkle:
                starAnimator.SetTrigger("SetTwinkle");   // “반짝임” 상태 트리거 발동
                break;
        }
    }

    /// <summary>
    /// 버튼 입력을 통해 상태를 간접적으로 전환하는 헬퍼 메서드입니다.
    /// 인자로 전달된 정수 값의 부호를 기준으로 상태를 구분합니다.
    /// </summary>
    /// <param name="button">
    /// -1 → Off / 0 → Twinkle / +1 → On  
    /// UI 버튼의 인스펙터 파라미터로 직접 지정할 수 있습니다.
    /// </param>
    public void OnClickStarStateButton(int button)
    {
        if (button < 0)
            SetStarState(StarState.Off);
        else if (button > 0)
            SetStarState(StarState.On);
        else
            SetStarState(StarState.Twinkle);
    }
}
