# 트랜스폼(Transform)
생각보다는 간단하지 않은 얘기, UnityKorea 유튜브 영상 정리

```c#
transform.position = transform.position + new Vector3(horizontalInput * movementSpeed);
```
![image](https://user-images.githubusercontent.com/31722512/167891662-2cd32605-230d-4501-85fa-6c6515248daa.png)
User Script 작성하는 공간은 `C# API`, 엔진 내부는 C++ Engine으로 구현되어 있다. 물리충돌을 위한 Physics Box2D/Physx 공간이 따로 존재한다. 그런데 트랜스폼을 하나 움직이면 위의 3가지
를 모두 고려하게 된다.

![image](https://user-images.githubusercontent.com/31722512/167890837-f0c901e1-91e7-4298-b8d0-dce8fd6136a2.png)
Translate 도 다양하게 오버로딩 되어있다. -> C++ Engine 내부로 들어가 처리하게 된다.

![image](https://user-images.githubusercontent.com/31722512/167891226-d0a92768-f76d-49ec-a5dd-1fdc96868562.png)

## 행렬과 트랜스폼 (matrix & transform)
![image](https://user-images.githubusercontent.com/31722512/167892800-5f118337-f3cf-4641-8b9f-06030345d91a.png)
수포자를 위한 게임 수학 #17로 이어서 공부할 수 있다.

내부로는 결국 `행렬 형태`로 존재하게 된다.
![image](https://user-images.githubusercontent.com/31722512/167893114-79fa207c-2f00-49d4-a3ea-07dd7472f794.png)
연산 최적화 방법이다.

많은 시스템에서 Transform을 업데이트를 하고 있다면? 모든 변경 사항들을 모아서 한 군데에서 처리하자.<br>
`Jump.cs`, `Run.cs`, `Dash.cs` etc..

## 간과하는 점
![image](https://user-images.githubusercontent.com/31722512/167894049-e053e1fe-8590-4c7d-ae58-a6e3fa828288.png)

피직스 공간과 트랜스폼 공간을 따로 존재하여 비효율적인 연산을 하게 된다. => `연산 낭비`

![image](https://user-images.githubusercontent.com/31722512/167894366-1c8eb768-e65d-4bd0-84e7-5a7413ccc9e5.png)
트랜스폼 변경했을 때 물리 공간을 항상 싱크 맞출거냐 아니면 물리 업데이트 주기에 맞춰서 할거냐를 옵션을 주는 형식이다.

## 결론
Transform -> Hierarchy
![image](https://user-images.githubusercontent.com/31722512/167894851-d20c4e98-9e3e-416b-8546-0c31064cbd84.png)
간단하게 `Example Assets`의 트랜스폼을 변경한다고 했지만, 내부적으로는 자식들의 Transform 까지도 업데이트가 되게 된다. 그래서 보통 계층 구조가 복잡하게 이뤄어져있다면
부모의 transform 의 변경을 할 때 많은 연산이 일어나게 된다. (자식들 내부에서 콜라이더나, 물리, 트랜스폼등을) 사용한다고 가정했을 때

다만 피할 수 없는 순간이 있는데 그것은 바로 캐릭터의 구조다. 팔, 다리 등등 관절이 계층 구조로 되어 있다.
![image](https://user-images.githubusercontent.com/31722512/167895308-715841fe-dc02-4f21-8e93-48e6cb14ec8b.png)

![image](https://user-images.githubusercontent.com/31722512/167895410-1c2b2914-1fdc-4b9a-9998-7a22ef3f1354.png)
엄청 많아지게 되면 많은 연산이 필요하게 된다.

![image](https://user-images.githubusercontent.com/31722512/167895466-cb70bf61-0f69-4778-afa0-7f19a7839989.png)
옵션을 체크하게 되면 계층 구조는 단순해지면서도 애니메이션이 잘 동작하게 된다. (성능을 선택 But 세세하게 접근할 수 없게 된다.)

![image](https://user-images.githubusercontent.com/31722512/167895570-8bb49b75-59ca-4eda-a004-e290cb2add56.png)
![image](https://user-images.githubusercontent.com/31722512/167895705-4a370b43-4036-446c-9bc5-17b49a2cd4ef.png)


