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
















