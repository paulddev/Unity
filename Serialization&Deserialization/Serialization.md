# Serialization (직렬화)
- Scriptable Objects
- YAML
- JSON
- XML
- FileID
- GUID
- Meta file
- Prefab

```C#
[System.Serializable] // 인스펙터에 노출할 수 있다.
struct TestStruct
{
    public float value1;
    public int value2;
}

public class SerializationTest : MonoBehaviour
{
    public float _float = 0;
    public Vector3 _vec3;

    [SerializeField] private TestStruct _struct;     // private 멤버도 인스펙터뷰에 노출할 수 있다.
    [SerializeField] private UnityEvent<int> _event; // 이벤트도 노출 가능
}
```

# Serialization(C#) MS 정의
`Serialization`은 개체를 저장하거나 메모리, 데이터베이스 또는 파일로 전송하기 위해 개체를 바이트 스트림(연속적인)으로 변환하는 프로세스다. <br>
주 목적은 필요할 때 다시 개체로 만들 수 있도록 개체의 상태를 저장하는 것입니다. <br>
역 프로세스를 `deserialization`이라고 한다.

## File로 저장하게 된다. (사람이 읽은 수 있는 형태로)
- XML(엑스엠엘) : Unity project
- YAML(야물) : Unity scene, prefab, scriptable object 
- JSON(제이슨, javascript object notation) : package file, network 통신

## 유니티에서의 직렬화
직렬화는 데이터 구조나 오브젝트 상태를 Unity 에디터가 저장하고 나중에 재구성할 수 있는 포맷으로 자동으로 변환하는 프로세스를 말한다. <br>
Unity 에디터에서는 저장 및 로딩, 인스펙터 창, 인스턴스화, 프리팹과 같은 일부 내장 기능에 직렬화가 사용된다. <br>

![image](https://user-images.githubusercontent.com/31722512/164980634-0b9a6254-faa9-4a9a-9d69-668db9a109ac.png)

![image](https://user-images.githubusercontent.com/31722512/164980711-953ea474-dcf6-46d5-939f-240045685d6b.png)
Asset 과 meta 파일이 쌍으로 존재한다.

![image](https://user-images.githubusercontent.com/31722512/164980762-2ae6161a-2244-4f5f-99b7-9786bdda668d.png)
Version Control 에서 `Mode` 에서 메타 파일을 보여줄지를 선택할 수 있다. <br>
Asset Serialization 에서 `Mode` 에서 기본적으로 Force Text 로 되어 있다. 사람이 읽을 수 있는 형태로 보이게 된다. <br>

- `guid` : global unique id (Asset 고유 아이디)
- `fileID` : 같은 파일 내에서 어떤 위치에 해당하는 데이터를 보면 되는지 가리키는 아이디
- `meta` : 파일에 머가 저장되어 있는지 확인하고 싶을 때, 메모장을 열거나 Sublime Text 에 드래그 해서 살펴볼 수 있다.

## 빌트인 직렬화
Unity에 빌트인된 일부 기능은 자동으로 직렬화를 사용한다.
- 저장(Saving) 및 로드
- 인스펙터 창
- Unity 에디터에서 스크립트 재로드
- Prefab
- 인스턴스화

![image](https://user-images.githubusercontent.com/31722512/164981270-335b73cc-c9a6-484c-9277-af5016d2f84e.png)
게임 오브젝트를 직렬화한 것이 `프리팹`이다.

## 인스턴스화
프리팹 또는 게임 오브젝트 같이 씬에 존재하는 것에 `Instantiate`를 호출하면 Unity가 이 항목을 직렬화한다. <br>
이는 런타임 시 그리고 에디터에서 모두 발생한다. UnityEngine.Object에서 파생된 모든 항목이 직렬화될 수 있다.

이후 Unity는 새 게임 오브젝트를 생성해 데이터를 새 게임 오브젝트에 역직렬화한다. <br>
다음으로 Unity는 다른 배리언트에서 동일한 직렬화 코드를 실행해 어떤 UnityEngine.Object가 레퍼런스된 것인지 보고한다. <br>
모든 레퍼런스된 UnityEngine.Objects를 체크해 인스턴스화되는 데이터의 일부인지를 확인한다. 레퍼런스가 텍스터 같은 `외부`요소를 가리키면 Unity는 해당 레퍼런스를 그대로 유지한다. 레퍼런스가 자식 게임오브젝트 같은 `내부`요소를 가리키면 Unity는 해당 복사본 레퍼런스로 패치한다.

## Scriptable Object
클래스 인스턴스와는 별도로 대량의 데이터를 저장하는 데 사용할 수 있는 데이터 컨테이너.<br>
`ScriptableObject`의 주요 사용 사례 중 하나는 값의 사본이 생성되는 것을 방지하여 프로젝트의 메모리 사용을 줄이는 것이다. <br>
이는 연결된 `MonoBehaviour` 스크립트에 변경되지 않는 데이터를 저장하는 프리팹이 있는 프로젝트의 경우 유용하다.

![image](https://user-images.githubusercontent.com/31722512/164981615-0bd8868c-4e6f-47fc-90b9-e0d6a25c5796.png)
몬스터의 상태 정보를 사용할 때(같은 정보를 초기화하는 상태) 메모리 사용량을 줄일 수 있다. 성능적인 면에서도 좋다고 한다.

![image](https://user-images.githubusercontent.com/31722512/164981688-4c8d24d8-3b84-415f-9502-aa26486cb00e.png)
여러가지 스크립트에 정보가 중복되어 있다면 유지보수에 불편하게 되는데, 이런 것들을 `Scriptable Object`를 사용하면 스펙이 변경되거나 데이터가 변경될 때 간단하게 변경할 수 있게 된다.

![image](https://user-images.githubusercontent.com/31722512/164981737-e4fe88f5-beb5-44a0-ab4c-b24a11bd077b.png)
코드의 간결성, 리펙토리, 유지보수에 편리하다.

![image](https://user-images.githubusercontent.com/31722512/164981771-7f17e838-2b0d-4390-b176-3d568dc5a9a5.png)
설정값 저장용으로도 많이 사용한다. (렌더러 설정, 렌더러 에셋) 프로필 용으로 많이 사용한다.

스크립터블 오브젝트도 역시 데이터를 직렬화한 구조이고, YAML 구조다.

![image](https://user-images.githubusercontent.com/31722512/164981858-cae2bced-8c55-4652-b891-f1e15e6f23d5.png)
- XML : 시간이 지남에 따라 다양한 개발환경이 생기게 되면서 점차 사용처가 줄어든다.
- JSON : 모든 어플리케이션에서 공용으로 사용하는 데이터 포맷
- YAML : Unity 내부 정보 사용중, version control 사용할 때 좋다.

## JsonUtility
내부적으로는 cpp로 구현되어있고 이것을 C#으로 랩핑해서 사용한다. 많은 기능을 지원하지 않지만 직렬화만 사용하게 된다면 이것의 사용을 고려해도 좋다.
![image](https://user-images.githubusercontent.com/31722512/164981909-6d333cf4-2141-43ae-9dfe-a8c61a1e0105.png)
```C#
[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int lives;
    public float health;

    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonString); // Read 할 때
    }

    public string SaveToString()
    {
        return JsonUtility.ToJson(this); // Write 할 때
    }

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f
}
```

## Unity Learn
- Unity Documentation
- Unity Learn (한글화 몇 개 지원)
- retr0
- 고라니
- 베르
- 고박사
- 골드메탈






