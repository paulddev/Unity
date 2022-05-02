# 유니티 모바일 빌드 자동화

## 유니티 모바일 빌드 파이프라인
1. `유니티 빌드`
2. `xcode` or `Eclipse` or `Android Studio`
3. `IPA`(xcode) or `APK`(Eclipse, Android Studio)

![image](https://user-images.githubusercontent.com/31722512/166255797-0530f077-c3e5-40b6-a190-51aba52d7468.png)

## ios 빌드
- xcode 프로젝트
  * 유니티 빌드시 xcode 프로젝트가 생성됨
  * Mac에서 xcode를 이용하여 Build->Archive를 해야한다.

- Replace / Append 모드
  * 기존의 xcode 프로젝트 변동 사항 유지를 위한 Append 모드 제공

- ipa 빌드
  * 유니티에서 바로 ipa 빌드가 불가능하다.

## ios 빌드 주의사항
![image](https://user-images.githubusercontent.com/31722512/166256257-0027d515-c33f-426b-8d79-2d82deffacc8.png)

- IL2CPP 필수
  * 64비트 단말기 대응을 위해서

- Target SDK 설정
  * Device SDK - 단말기용 빌드
  * Simulation SDK - Xcode Simulator 용 빌드

- Enable Bitcode
  * 향후 업데이트와 빠른 다운로드를 제공하기 위한 기술
  * 앱 용량이 커지는 현상, 써드파티 라이브러리 충돌
  * 가급적 `No`로 설정! (Xcode 프로젝트에서 설정 가능하다.)

## Android 빌드
- ADT / Gradle 프로젝트
  * ADT는 eclipse 또는 ant로 빌드
  * Gradle은 Android Studio 또는 gradle로 빌드

- APK 빌드
  * 유니티에서 바로 apk 빌드 가능
  * Android SDK, Android NDK, JDK 설정이 필요하다.

- Append 모드가 없다.
  * 그래서 항상 빌드할 때마다 새로운 프로젝트가 만들어지게 된다.

## Android 빌드 주의사항
- 가급적 Gradle 사용
  * ADT는 구글에서 더 이상 지원을 하지 않음
  * 64K 참조제한, MultiDex 같은 문제는 ADT로 해결 불가능

- APK 빌드 사용 권장
  * ADT / Gradle 프로젝트 생성이 없어 빌드가 빠르다.
  * KeyStore 생성 / Signing 이 유니티에서 가능하여 편하다.

## PlayerSetting에 대해서
- 플랫폼별 빌드 세부설정
- ProjectSettings.asset 파일에 저장
- 여러 종류의 빌드를 위해 C# 스크립트 빌드 셋팅 권장

![image](https://user-images.githubusercontent.com/31722512/166257300-19b9d62b-a550-44d0-bd7c-afefa29c7b7d.png)
이렇게 쓰면 관리하기 편하다.

## PostProcessBuild 활용 (유니티 빌드 후 제일 마지막에 PostProcessBuild 가 실행된다.)
- ios 빌드시 Append 모드의 문제점
  * 모듈 수정/삭제 시 -> Xcode 프로젝트에 이전 내용이 그대로 남아 있게 된다.
  * 유니티 업데이트 시 -> Xcode 프로젝트 생성방식이 변경된 경우 변경 사항 제대로 적용이 되지 않는 문제가 있다.

- Replace 모드 사용
- Xcode 프로젝트 설정은 PostProcessBuild 활용

![image](https://user-images.githubusercontent.com/31722512/166257691-1ee82166-2010-483d-8bd5-deb9563fa1a8.png)

## 빌드 자동화를 해야 하는 이유
- 유니티 모바일 빌드 과정은 꽤 번거롭다.
  * Switch Platform -> Unity Build -> ios / Android Build
  * 최소 20분 이상 소요

- 빌드 중에 아무 작업도 할 수 가 없다.
  * 컴파일 / 빌드 작업은 시스템 점유율이 제일 높다.

- 귀찮은 단순 반복 작업
  * 항상 똑같은 패턴의 귀찮은 작업
  * 자동화를 하지 않는 이유가 무엇인가?






