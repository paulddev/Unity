# 최적화, 게임, 그래픽스, 파이프라인, 프로파일링, 메모리, 코드 아키텍처, 물리, UI, 오디오

핵심은 `병목현상`을 파악하는 것이다.

- DP Call
- 복잡한 연산
- 3D의 수많은 버텍스, 연산
- 픽셀, 오버 드로우
- 셰이더의 많은 연산
- 압축되지 않고 큰 텍스쳐들
- 고 해상도 프레임 버퍼
- 
## GPU
그래픽 처리를 위한 고성능의 처리장치로 그래픽카드의 핵심이다. 주로 영상정보를 처리하고 화면에 출력시키는 일을 한다.

## CPU
컴퓨터의 정중앙에서 모든 데이터를 처리하는 장치

## 유니티의 메모리
Unity에서 작성하는 스크립트는 자동 메모리 관리를 사용한다. <br>
C,C++와 같은 low level은 수동 메모리 할당을 사용하고, 프로그래머가 직접 메모리 주소에 접근해 읽고 쓰는 것을 허용하므로 모든 오브젝트 관리는 프로그래머의 책임이다.

## 가비지 콜렉터
Unity가 레퍼런스를 참조하지 못하는 오브젝트(아무것도 상속받지 않은 클래스의 인스턴스를 레퍼하는 변수를 null로 설정하면 Unity가 레퍼런스를 참조하지 못한다.) 이 오브젝트에 접근하거나 다시볼 수 없지만
메모리에는 남아있기에 C# 처럼 `managed`언어는 시간이 지나면 가비지 컬렉터가 메모리의 각 블럭에 대한 레퍼런스의 수를 내부적으로 계속 추적하여 제거해준다.

## 클래스와 구조체 차이
힙에 할당된 원본 Class를 가리키는 `레퍼런스(참조)`를 받게 되고 class에 변경이 있을 경우 class가 레퍼런스가 되는 모든 곳에서 이 변경된 내용을 볼 수 있다.

구조체는 `복사본`을 넘겨받는다. 구조체는 힙에 할당되지 않고 가비지 컬렉션의 대상이 되지도 않는다. 구조체의 복사본이 수정되어도 다른 구조체에는 영향을 주지 않는다. 장시간 유지되어야 하는
오브젝트는 클래스여야 하고, 단시간만 사용할 오브젝트는 구조체여야 한다. Vector3는 구조체다.

구조체와 클래스는 동일한 데이터여도 클래스는 참조를 위해 `8~24`바이트의 추가적인 메모리를 필요로 한다.

## 스크립트 최적화
### Instantiate, Destroy는 비용이 상당히 크다.
프리팹 수가 많다면 시작 전 미리 생성 후 `오브젝트 풀`을 활용하자.

### 나눗셈보다 곱셈을 사용하자.
나눗셈은 곱셈보다 연산속도가 월등히 느리다. `1/10` 대신 `x0.1f` 를 사용하자.

### 때로는 Update 함수보다는 Coroutine을 사용하는게 좋다.
Update는 어떠한 경우에서도 매 프레임 확인하지만 Coroutine의 경우는 원하는 순간에 잠들어 있게 할 수 있으므로 매 프레임마다 필수적으로 해야하는 작업이 아니라면 코루틴을 고려하도록 하자.
짧은 주기로 코루틴을 자주 실행한다면 `StartCoroutine()`은 Coroutine타입의 객체를 리턴하기 때문에 GC의 먹이가 되므로 아주 가끔 반복적으로 실행되는게 아니라면 대부분은 Update에서 처리한다.

### 복잡한 수학적 연산은 자제하자.
`초월함수(pow, exp, log, cos, sin, tan)`는 리소스가 상당히 많이 소요되므로 가능한 경우 사용하지 않아야 한다.

### 오브젝트의 자식이 많다면 transform 변경시 많은 비용이 발생한다. 콜백 함수가 비어있다면 지우자!
Start, Update 등 콜백함수는 비어있어도 호출이 되기 때문에 콜백함수 안에 코드가 없다면 지우자.

### GetComponent(), Find() 사용 줄이기
GetComponent, Find 류 메소드는 자주 호출하면 성능에 악영향을 끼치기에 객체 참조가 필요할 때마다 Update에서 호출하는 방식은 지양하고, 최대한 Awake, Start 메소드에서 Get, Find
메소드를 이용해서 객체들을 필드에 캐싱하여 사용해야 한다.
```C#
private Transform cachedTransform
{
  get 
  {
    if (cachedTransform == null)
    {
      cachedTransform = GetComponent<Transform>();
    }
    return cachedTransform;
  }
}
```

### 코루틴 yield 캐싱하기
코루틴은 WaitForSeconds() 등의 객체를 yield return으로 사용하는데 wait할 때마다 `new`로 동적생성할 경우, 전부 GC의 대상이 되므로 아래처럼 캐싱하여 사용하자.
```C#
private IEnumerator Coroutine()
{
  var wfs = new WaitForSeconds(1f);
  while(true)
  {
    yield return wfs;
  }
}
```
### 참조 캐싱하기
```C#
void Update()
{
  _ = Camera.main.gameObject;
  _ = Camera.main.transform.forward;
}
```
주로 프로퍼티 호출에 해당한다. 프로퍼티는 필드가 아니고 메소드처럼 사용할 수 있는 참조다. 그만큼 `메소드 호출만큼의 오버헤드가 발생`한다. 프로퍼티가 연계되어 이어진다면
각각 전부 오버헤드로 이어진다. 따라서 이를 자주 호출할 경우 미리 참조로 필드에 캐싱하여 사용하는 것이 좋다. 심지어 `Time.deltaTime`마저도 하나의 업데이트에서 여러 군데에서 사용
한다면 Update() 최상단에 캐싱하여 사용하는 것이 좋다.

### Transform 변경은 한번에 하자!
position, rotatation, scale 을 한 메소드 내에서 여러 번 변경하면 그 때마다 transform 의 변경이 이루어진다. 그런데 transform이 여러 자식 transform 가지고 있다면 자식 transform
역시 함께 변경된다. 벡터로 미리 담아두고 최종 계산 이후, 트랜스폼에 단 한번 변경을 지정하는 것이 좋다. 또한 position과 rotation을 모두 변경해야 한다면 `SetPositionAndRotation()`
메소드를 사용하는 것이 좋다.

### ScriptableObject
스크립트 내에서 항상 공통적인 변수를 사용하는 경우, 각 객체의 필드로 사용하면 동일한 데이터가 객체의 수의 배수만큼 메모리를 차지한다. 반면에 ScriptableObject로 만들고 이를 필드로
공유하면 객체의 수에 관계없이 동일 데이터는 하나만 존재하므로 메모리를 절약할 수 있다. 추가적으로 `Flyweight` 방법도 있다.

### Debug.Log 는 랩핑해서 사용하자.
디버깅할 때 도움이 되지만 빌드 이후에도 호출되면 성능을 많이 소모하므로 빌드 직전 코드를 다 지워주자.

### 박싱, 언박싱 피하자.
`박싱` 값 형식을 참조 형식으로 바꾸는 것, `언박싱` 참조 형식에서 값 형식으로 바뀌는 것을 말한다. 박싱을 하면 단순 참조보다 20배까지 시간이 소요되며, 언박싱은 할당에 4배 정도다.
이를 피할 수 있는 대표적인 방법이 바로 `제너릭`이다. 제너릭은 객체 생성, 메소드 호출 시 제너릭 타입(하나의 타입)으로 고정되기에 박싱과 언박싱을 피할 수 있다.

### Magnitude 보다 sqrMagnitude 를 사용하여 비교해서 쓴다. (제곱근 계산 x)

### 삼각함수의 값은 상수로 저장하고 사용하는게 좋다.

### 문자열은 readonly 혹은 const 키워드를 사용하여, 가비지 컬렉션으로부터 벗어나도록 한다.

## 가비지 컬렉터
MonoBehaviour는 메모리 관리에 GC가 자동호출되도록 설계되어 있는데, `GC`가 실행되는 동안 유저들은 게임에서 갑자기 렉이 걸리게 된다. 무엇이든 간에 동적 생성 및 해제는 부하가 굉장히
큰 작업이다.

### 문자열 병합이 자주 일어난다면? StringBuilder의 Append를 사용하자.
`string + string`은 임시 문자열을 뱉기 때문에 가비지 컬렉션이 일어나는 환경을 제공한다. (불변 객체 = string)

### 임시 객체를 만들어내는 API 를 조심하자.
`GetComponent<T>`, `Mesh`, `Vertices`, `Camera.allCameras`
  
### 텍스쳐 아틀라스를 사용
텍스쳐 아틀라스를 최대한 묶어 사용한다. UI만 아니라, 같은 재질의 오브젝트를 묶어 사용하게 된다.

### 태그 비교는 CompareTag() 사용하기
객체의 tag 프로퍼티를 호출하는 것은 복사를 하여 메모리를 추가 할당하게 되므로 지양하자.

### 모든 비교문에서는 .equals()를 사용하자
`==`구문은 사용하면 암시적인 메모리가 나오게 되어 가비지 컬렉션의 먹이를 주므로 지양하자.

### foreach 대신에 for문을 사용하자.
foreach는 한 번 반복시 `24byte`의 가비지 메모리를 생성하여 수없이 돌면 더 많은 메모리를 생성시키므로 for문을 이용하자.

## 물리 성능 최적화
### Fixed Timestep 조절
Fixed Tiemstep 설정을 조정하여 물리 업데이트에 드는 시간을 줄일 수 있다. 줄이게 되면 CPU 오버헤드가 늘어나고 물리 정확도가 상승하지만, 늘리면 물리 정확도는 떨어지는 대신에 성능은
향상된다.

### Maximum Allowed Timestep
시스템에 부하가 걸려 지정된 시간보다 오래 걸릴 경우 물리 계산을 건너 뛰는 설정이다. `8~10`fps 범위로 설정하여 최악의 시나리오를 막도록 한다.

### Mesh Collider 지양
mesh Collider는 기본 콜라이더에 비해 퍼포먼스 오버헤드가 크므로 꼭 필요한 경우에만 사용해야 된다.

### Raycast, SpareCheck와 같이 충돌 감지 요소를 최소화하자.

### 움직이지 않는 배경 오브젝트는 StaticObject로 처리한다.
다만 StaticObject로 처리하였는데 중간에 움직인다면 매우 많은 비용이 들게 된다.

## 렌더링
### Frustum Culling(프러스텀 컬링)
Layer 별로 컬링 거리 설정이 가능 (NGUI 경우 Panel 에서 Smooth Culling 도 먹일 수 있다.) 멀리 보이는 중요한 오브젝트는 거리를 멀게 설정하고 중요도가 낮은 풀이나 나무 등은
컬링 거리를 짧게 설정하여 컬링한다.
  
### Occlusion Culling(오클루젼 컬링)
`Window > Occlusion Culling` 에서 설정가능하며 `카메라에서 보이는 각도의 오브젝트 들만 렌더링 하는 기법`을 뜻한다.

### Combine (오브젝트 통합)
드로우 콜은 오브젝트에 설정된 재질의 셰이더 패스당 하나씩 일어나게 된다. 렌더러에서 사용된 재질의 수만큼 드로우 콜이 발생한다.
성질이 동일한 오브젝트들은 하나의 메쉬와 재질을 사용하도록 통합한다. Script패키지-CombineChildren 컴포넌트를 제공한다.(하위 오브젝트를 모두 하나로 통합)

## Batch
### Static Batch
- `Edit > Project Setting > Player` 에서 설정한다.
- 움직이지 않는 오브젝트들은 static으로 설정해서 배칭이 되게 한다.
- Static으로 설정된 게임 오브젝트에서 동일한 재질을 사용할 경우, 자동으로 통합된다.
- 통합되는 오브젝트를 모두 하나의 커다란 메쉬로 만들어서 따로 저장한다. (메모리 사용량 증가)

### Dynamic Batch
- 움직이는 물체를 대상으로 동일한 재질을 사용하는 경우, 자동으로 통합한다.
- 동적 배칭은 계산량이 많으므로, 정점이 900개 미만인 오브젝트만 대상이 된다.

## 라이팅
### 라이트 맵 사용
- 고정된 라이트와 오브젝트의 경우(배경) 라이트 맵을 최대한 활용하자.
- 아주 빠르게 실행된다. (per-pixel light보다 2~3배)
- 더 좋은 결과를 얻을 수 있는 GI와 Light Mapper를 사용할 수 있다.

### 라이트 렌더 모드
- 라이팅 별로 Render Mode : Important / Not Important 설정이 가능하다.
- 게임에서 중요한 동적 라이팅만 Important 로 설정
- 그렇지 않은 라이트들은 Not Important로 설정한다.

## Overdraw(오버드로우)
### 화면의 한 픽셀에 두 번 이상 그리게 되는 경우 Fill Rate
- DP call의 문제만큼 overdraw로 인한 프레임 저하도 중요한 문제
- 특히 2D 게임에서는 DP Call보다 더욱 큰 문제가 된다.

### 기본적으로 앞에서 뒤로 그린다.
- Depth testing으로 인해서 오버드로우를 방지한다.
- 하지만 알파 블렌딩이 있는 오브젝트의 경우에는 알파 소팅 문제가 발생한다.

### 반투명 오브젝트의 개수의 제한을 건다.
- 반투명 오브젝트는 뒤에서부터 앞으로 그려야 한다. (Overdraw 증가)
- 반투명 오브젝트의 지나친 사용에는 주의해야 한다.

### 유니티 Render Mode 를 통해서 overdraw 확인이 가능하다.

## 유니티 프로파일러
### CPU Usage
- 프로파일러에서 주로 확인하게 되는 부분으로, 해당 프레임에서 각각의 코드 수행에 걸린 시간과 비율 등을 확인할 수 있다.
- Hierarchy mode : 대부분 callstack을 표시하되 몇몇 유사한 데이터들 또는 global unity function call를 그룹화한다.
- Timeline mode : 블럭 길이로 CPU 사용량을 볼 수 있어 편하다. 수직축은 callstack을 의미하므로 아래쪽 블럭으로 깊이 쌓일수록 callstack에 더 많은 call이 쌓였다는 걸 의미한다.

- `Total` : 해당 메소드와 하위 메소드들의 호출에 소요된 시간 비율의 총합
- `Self`  : 해당 메소드의 호출에서만 소요된 시간 비율
- `Calls` : 한 프레임 내에서 해당 메소드가 실행된 횟수
- `GC Alloc` : 해당 메소드 및 하위 메소드에서 생성된 메모리의 크기
- `Time ms` : 해당 메소드 및 하위 메소드를 실행하는 데 소요된 시간
- `Self ms` : 해당 메소드의 호출에만 소요된 시간
- `Warning` : 한 프레임 내에서 경고가 호출된 횟수
![image](https://user-images.githubusercontent.com/31722512/167442378-4b35b688-bfac-4a3f-9b36-f9949d17fb8b.png)

### Show Related Objects
연관된 오브젝트의 이름과 구체적인 정보를 확인할 수 있다. 연관된 오브젝트가 없는 경우 `N/A`로 표시된다.
![image](https://user-images.githubusercontent.com/31722512/167442321-bbc8d457-ed1c-4a0d-86b8-c4b9e3d25c42.png)

### Show Calls
하나의 항목을 선택할 경우 해당 메소드를 호출한 부분들을 모두 확인할 수 있다.
![image](https://user-images.githubusercontent.com/31722512/167442482-01b8d3bd-03bb-4de8-af1a-94adfcdf60d6.png)

### Deep Profile
- 더 구체적인 메소드 호출 스택을 확인할 수 있다. 하지만 프로파일리에 성능을 더 많이 소모하므로 유의해야 한다.
- 일반 프로파일링은 유니티 콜백함수(Awake, Start, Update)에 의한 소요시간과 메모리 할당을 체크한다.
- 딥 프로파일링은 프로젝트의 모든 스크립트를 다시 컴파일해서 전체 callstack에 올라오는 모든 함수를 추적한다. 따라서 런타임에 수행하기에는 비용과 메모리 소모가 크므로 사용하지 못할 수 있다.
- 토글될 때마다 다시 컴파일이 이뤄지므로 자주 누르면 안된다.

### GPU Usage
- CPU와 유사하게 메소드 호출 스택에 따른 성능을 파악할 수 있으며, 드로우콜 횟수를 확인할 수 있다.
- method calls 와 processing time 을 표시한다.
![image](https://user-images.githubusercontent.com/31722512/167442709-a8294eb7-85e4-4042-b8c6-3533d9831c56.png)

### Rendering
- 드로우 콜, 패스 콜 등 렌더링 관련 정보를 확인할 수 있다.
- GPU에게 rendering을 지시하는 CPU의 활동을 나타낸다. (SetPass calls, Batches등)
![image](https://user-images.githubusercontent.com/31722512/167442811-ea4dc7e6-d708-4639-9046-87e6ee8671a3.png)

### Memory
항목 별 메모리 사용량을 확인할 수 있다.
![image](https://user-images.githubusercontent.com/31722512/167442885-442b10c4-0c96-475d-b573-341b4eda80b5.png)

## Verifying the correct order of events
- 유니티는 Native code가 managed code로 호출하는 일련의 콜백들에 의해 작동한다.
- 여러 Awake() 함수 간에 어떤 게 먼저 실행될지 알 수 없으므로 Awake() 함수 간에 상호의존적인 코드를 작성하지 말아야 한다.
- Awake 보다는 분명하게 이후에 호출되는 Start 함수를 적절히 이용해야 한다.
- Update 함수와 LateUpdate 함수를 구분해서 사용한다.
- 유니티 엔진은 동일한 디바이스에서도 작동 양상이 그때그때 조금씩 다르다. 

## Minimizing internal distractions
- 유니티 에디터에서 프로파일링 할 때 하나의 프레임을 처리하는데 오래걸리는 경우(첫 실행시 초기화)등 해당 시점에 대한 프로파일 정보를 기록하지 못할 수 있다.
- 프로파일링할 때에는 항상 게임 뷰가 활성화된 상태(클릭된 상태)여야 한다.
- Vsync 옵션은 CPU Usage에 스파크를 발생시켜서 프로파일링에 방해되므로 체크박스에서 비활성화 하자.
- Debug.Log(), LogError(), LogWarning() 같은 함수는 CPU와 메모리를 낭비하므로 과도하게 호출된다면 프로파일링에 방해가 될 수 있다.

## 코드 단에서 이뤄지는 프로파일링
```C#
UnityEngine.Profiling.Profiler 클래스의 delimiter 함수 이용
Development Build에서만 컴파일된다.

void DoSomethingCompletelyStupid()
{
  Profiler.BeginSample("My Profiler Sample");
  List<int> listOfInts = new List<int>();
  for (int i = 0; i < 1000000; i++)
  {
    listOfInts.Add(i);
  }
  Profiler.EndSample();
}
```

## System.Diagnostics.Stopwatch
- 빠르고 저렴하게 코드실행 시간을 측정할 수 있다.
- 호출되는 함수가 반복해서 동일한 메모리에 접근하게 되면 일반적인 경우보다 메모리 접근 속도가 빨라지므로 실제 사용되는 상황에서보다 처리속도가 빠른 것처럼 측정될 수 있다.
- using 블록은 unmanaged resource가 정해진 블록을 벗어날 때 적절하게 처리되도록 보장하기 위해 블록이 끝나는 시점에 오브젝트의 Dispose() 함수를 호출한다. 이를 위해 해당 오브젝트는 IDisposable 인터페이스를 상속받고 Dispose() 함수를 구현해야 한다.

- 최적화가 필요한 부분을 특정하기 위한 방법 : FPS, 메모리 사용량, CPU 사용량,  CPU/GPU 온도체크
- 에디터가 아니라 실제 디바이스에서 Unity Profiler 를 연결하여 벤치마크한다.

1. Build Setting에서 Development Build, Autoconnect Profiler flags 체크
2. Mobile 기기와 Mac 을 Wi-Fi로 연결함과 동시에 USB 또는 라이트링 케이블로도 연결
3. Build & Run 옵션으로 빌드를 진행
4. Profiler 뷰의 Connected Player 옵션에서 Editor 가 아닌 디바이스를 선택한다.

- 아무런 목적 없이 맹목적으로 수집된 데이터는 분석에 방해가 되므로 현재 필요한 데이터의 범위를 좁히는 것이 좋다.
- 유저가 퍼포먼스 저하를 인지할 수 있는가라는 기준으로 최적화가 필요한지 아닌지를 판단한다.
- 퍼포먼스 저하가 반복적으로 재현될 수 있는지, 어떤 환경에서 생기는지, 정확히 어떤 코드 블럭에서 생기는지를 알아야 한다.
- 관습적으로 변수 이름 앞에 언더스코어`_`를 붙이면 클래스의 멤버 함수(=field)라는 의미이며 지역 변수 및 인수와 구분하기 쉬워진다.
- string.Format() 함수는 string 객체를 생성한 후 + 연산자를 통해 문자열을 만드는 것보다 메모리를 적게 할당한다.

## Z order 이어서..




