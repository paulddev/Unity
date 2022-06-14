/*
https://docs.microsoft.com/ko-kr/windows/mixed-reality/develop/unity/performance-recommendations-for-unity

# Unity에 대한 성능 추천 사항

1. 캐시 참조
GetComponet<T> 및 Camera.main 과 같은 반복적인 함수 호출이 포인터를 저장하는 메모리 비용에 비해 더 비싸기 때문에
초기화 시 모든 관련 구성 요소 및 GameObjects에 대한 참조를 캐싱하는 것이 좋다.

특히 Camera.main 은 FindGameObjectsWithTag() 만 사용한다.
MainCamera 태그를 사용하여 장면 그래프에서 카메라 개체를 검색하지만 많은 비용이 든다고 한다.

참고!

(좋음) GetComponent<Type 형식> 구성 요소
(좋음) GetComponent<T>()
(나쁨) GetComponent<문자열>()

항상 형식 기반 구현을 사용하고, 문자열 기반 검색 오버로드를 사용하지 않는 것이 좋다.
*/

namespace Program
{
    public class Test : MonoBehaviour
    {
        private Camera cam;
        private CustomComponent comp;

        private void Start()
        {
            cam = Camera.main;
            comp = GetComponent<CustomComponent>();
        }

        private void Update()
        {
            // Good
            this.transform.position = cam.transform.position + cam.transform.forward * 10.0f;

            // Bad
            this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 10.0f;

            // Good
            comp.DoSomethingAwesome();

            // Bad
            GetComponent<CustomComponent>().DoSomethingAwesome();
        }
    }
}

/*
비용이 많이 드는 작업 방지

# Linq 사용 방지
Linq는 깨끗하고 읽기 및 쓰기가 쉬울 수 있지만 알고리즘을 수동으로 작성한 경우보다 일반적으로 더 많은 계산과 메모리가 필요하다.
*/

using System.Linq;

List<int> data = new List<int>();
data.Any(x => x > 10);

var result = from x in data
             where x > 10
             select x;

/*
# 일반 Unity API
일부 Uniy API는 유용하지만 실행 비용이 많이 들 수 있다.
이러한 API 중 대부분은 전체 장면 그래프에서 일치하는 몇 가지 GameObjects 목록을 찾는다.
일반적으로 런타임에 참조를 추적하기 위해 참조를 캐싱하거나 GameObjects에 대한 관리자 구성 요소를 구현하여 이러한 작업을 방지할 수 있다.

참고!
SendMessage() 와 BroadcastMessage() 는 반드시 제거해야 한다.
이러한 함수는 직접 함수 호출보다 1,000배 더 느릴 수 있다.
 */

GameObject.SendMessage();
GameObject.BroadcastMessage();
UnityEngine.Object.Find();
UnityEngine.Object.FindWithTag();
UnityEngine.Object.FIndObjectOfType();
UnityEngine.Object.FindObjectsOfType();
UnityEngine.Object.FindGameObjectsWithTag();

/*
# Boxing 주의
박싱은 C# 언어 및 런타임의 핵심 개념이다.
char, int, bool 등과 같은 값 형식 변수를 참조 형식 변수로 래핑하는 프로세스인데,
값 형식 변수가 boxed 가 되면 관리형 힙에 저장된 System.Object 에 래핑된다.

메모리가 할당되며, 결국에는 삭제 시 가비지 수집기에서 처리해야 한다.
이러한 할당 및 할당 취소는 성능 비용을 발생시키며, 많은 시나리오에서 필요하지 않거나 비용이 더 저렴한 대안으로 쉽게 대체할 수 있다.

boxing을 방지하려면 숫자 유형과 구조체(Nullable<T> 포함)를 저장하는 변수, 필드 및 속성의 형식이 개체를 사용하는 대신
int, float? 또는 MyStruct 와 같은 특정 형식의 강력한 형식이어야 한다.
이러한 개체를 목록에 넣는 경우는 List<object> 또는 ArrayList 가 아닌 List<int> 와 같은 강력한 형식의 목록을 사용해야 한다.
*/

bool myVar = true;
object boxedMyVar = myVar;

/*
# 반복 코드 경로
초당 여러 번 실행되는 반복 Unity 콜백 함수(즉, 업데이트)는 신중하게 작성해야 한다.
비용이 많이 드는 작업은 성능에 크고 일관된 영향을 미칠 수 있다.

1. 빈 콜백 함수
모든 Unity 스크립트가 Update 메서드로 자동 초기화 되기 때문에 이러한 빈 콜백은 비용이 많이 들 수 있다.
Unity는 UnityEngine 코드와 애플리케이션 코드 간에 비관리/관리 코드 경계를 넘나들며 작동한다.
이 브리지를 통해 컨텍스트를 전환하는 것은 실행하는 작업이 없더라도 비용이 매우 많이 발생한다.
반복되는 빈 Unity 콜백이 있는 구성 요소가 포함된 100개의 GameObjects가 앱에 있는 경우 특히 문제가 된다.

참고!

Update()는 이 성능 문제의 가장 일반적인 증상이지만 다른 Unity 콜백은 나쁘지 않은 경우에도 마찬가지로 나쁠 수 있다.
ex) FixedUpdate(), LateUpdate(), OnPostRender(), OnPreRender(), OnRenderImage()
*/

void Update()
{
    // 빈 콜백
}

/*
2. 프레임당 한 번 실행되도록 하는 작업

다음 Unity API는 많은 홀로그램 앱의 일반적인 작업이다.
이러한 함수의 결과는 항상 가능한 것은 아니지만 일반적으로 한 번 계산될 수 있으며, 지정된 프레임에 대해 애플리케이션 전체에서 다시 사용할 수 있다.

a) 각 구성 요소에서 반복적이고 동일한 Raycast 작업을 수행하는 대신, 장면에서 Raycast를 응시하도록 처리한 다음,
이 결과를 다른 모든 장면 구성 요소에서 다시 사용하는 전용 Singleton클래스 또는 서비스를 사용하는 것이 좋다.
일부 애플리케이션에는 다른 원본 또는 다른 LayerMask 에 대한 Raycast가 필요할 수 있다.
 */

UnityEngine.Physics.Raycast();
UnityEngine.Physics.RaycastAll();

// b) Start() 또는 Awake()에서 캐싱 참조를 통해 Update()와 같은 반복적인 Unity 콜백에서 GetComponent() 작업을 사용하도록 방지한다.

UnityEngine.Object.GetComponent();

// c) 가능한 경우 초기화 시 모든 개체를 인스턴스화하고 개체 풀링을 사용하여 애플리케이션의 런타임 전체에서 GameObjects를 재활용하고 다시 사용하는 것이 좋다.

UnityEngine.Object.Instantiate();

/*
3. 인터페이스 및 가상 구문 사용 방지

직접 객체보다 인터페이스를 통해 함수 호출을 호출하거나 가상 함수를 호출하는 경우 종종 직접 구문 또는 직접 함수 호출을 활용하는 것보다 훨씬 비용이 많이 들 수 있다.
가상 함수 또는 인터페이스가 필요하지 않은 경우 이를 제거해야 한다.
그러나 이러한 방법의 성능 결과는 이를 사용하여 개발 협업, 코드 가독성 및 코드 유지 관리가 간소화되는 경우 절충할만한 가치가 있다.

일반적으로 이 멤버를 덮어쓸 필요가 있는 경우에만 필드 및 함수를 가상으로 표시하는 것이 좋다.
UpdateUI() 메서드와 같이 프레임당 여러 번 또는 한 번 호출되는 빈도가 높은 코드 경로에서는 특히 주의하자.

4. 값으로 구조체 전달 방지

클래스와 달리 구조체는 값 형식이며, 함수에 직접 전달되면 해당 내용이 새로 만든 인스턴스에 복사된다.
이 복사본으로 인해 CPU 비용과 스택의 추가 메모리가 추가된다.
작은 구조체의 경우 효과가 최소화되므로 허용되지만, 모든 프레임을 반복적으로 호출하는 함수와 큰 구조체를 사용하는 함수의 경우에서는
가능하면 함수 정의를 참조로 전달하도록 수정하는 것이 좋다.
https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/classes-and-structs/how-to-know-the-difference-passing-a-struct-and-passing-a-class-to-a-method

기타

1. Physics

a) 일반적으로 물리학을 향상시키는 가장 쉬운 방법은 Physics에 소요되는 시간 또는 초당 반복 횟수를 제한하는 것이다.
이렇게 하면 근데 시뮬레이션의 정확도가 떨어지게 된다. Unity의 TimeManager 참조

b) Unity의 Collider 형식에는 매우 다양한 성능 특징이 존재한다.
아래 순서는 왼쪽에서 성능이 가장 높은 collider, 오른쪽에는 성능이 가장 낮은 collider의 순서로 나열되어 있다.
기본 collider보다 실질적으로 더 비싼 메시 collider를 사용하지 않도록 방지하는 것이 중요하다. 

구 < 캡슐 < 상자 <<< 메시(볼록) < 메시(비볼록)
https://learn.unity.com/tutorial/physics-best-practices

2. 애니메이션

Animator 구성 요소를 사용하지 않도록 설정하여 유휴 애니메이션을 사용하지 않도록 설정한다. (게임 개체를 사용하지 않도록 설정하면 동일한 효과가 없다.)
애니메이터가 동일한 값으로 반복에 있는 디자인 패턴을 사용하지 않도록 방지한다.
이 기술에는 애플리케이션에 영향을 주지 않지만 상당한 오버헤드가 존재한다.

3. 복잡한 알고리즘

애플리케이션에서 역운동학, 경로 찾기 등과 같은 복잡한 알고리즘을 사용하는 경우 더 간단한 방법을 찾거나 해당 성능과 관련된 설정을 조정하자.
*/

/*
# CPU - GPU 성능 추천 사항
CPU-GPU 성능은 결국에는 그래픽 카드에 제출되는 그리기 호출이 된다.
성능을 향상시키려면 최적의 결과를 얻을 수 있도록 그리기 호출을 전략적으로 줄이거나, 재구성해야 한다.
그리기 호출 자체는 리소스를 많이 사용하므로 이러한 호출을 줄이려면 필요한 전체 작업을 줄일 수 있게 된다.
또한 그리기 호출 간의 상태 변경을 그래픽 드라이버에서 비용이 많이 드는 유효성 검사 및 변환 단계가 필요하므로 상태 변경을 제한하는 애플리케이션의 그리기를 호출을
재구성 한다면 성능을 개선할 수 있다.

https://docs.unity3d.com/Manual/DrawCallBatching.html
*/

