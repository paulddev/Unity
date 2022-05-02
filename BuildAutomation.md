# 유니티 모바일 빌드 자동화
- 우리는 다양한 빌드를 하고 있다.
- Android
- IOS
- IAP
- QA
- DIST
- DEV
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

## 사람은 언제나 실수를 한다.
- 잘못된 빌드 설정
  * 테스트 코드나 디버깅용 설정이 그대로 배포
  * 단 한번도 실수를 하지 않고 매번 동일하게 빌드할 자신이 있는가?

- 빌드 환경의 차이
  * Unity 버전 차이
  * IOS / Android SDK 버전 차이
  * Xcode, JDK, NDK, Ant, Gradle 버전 차이
  * 사소한 버전 차이도 빌드에 큰 영향을 준다.

# Jenkins(젠킨스) vs Unity Cloud Build
![image](https://user-images.githubusercontent.com/31722512/166258929-f514730a-a152-4e00-819c-4c6c29bcd76a.png)
## Jenkins
- 무료
- 다양한 버전 관리 시스템과 빌드툴 지원
- 사용자가 많아서 Reference가 풍부하다.
- 플러그인이 굉장히 많다. (100여개)
- 오픈소스임에도 불구하고 유지보수가 잘되고 있다.

## Unity Cloud Build
- 유니티 전용 빌드 자동화 서비스
- 유료 서비스 ($9/월)
- 빌드 전용 맥 또는 PC 필요 없다
- 매우 간단하고 쉽게 빌드 자동화 셋팅 가능
- Jenkins 만큼 세세하고 다양한 설정은 불가능하다.

## Jenkins 를 이용한 유니티 모바일 빌드 자동화
![image](https://user-images.githubusercontent.com/31722512/166259172-b55d1f05-0575-4db8-ab39-cd70e6e85475.png)
![image](https://user-images.githubusercontent.com/31722512/166259282-51fe89ed-ebbf-4e03-9fdf-3a83327efddb.png)
![image](https://user-images.githubusercontent.com/31722512/166259680-f9a12682-6936-4590-b80a-a0451f9ae72c.png)
![image](https://user-images.githubusercontent.com/31722512/166259723-177a05e0-cad4-4608-ac02-5fda80316643.png)
![image](https://user-images.githubusercontent.com/31722512/166259789-f50a7d2f-97f3-48ea-8c2e-6f4ef042c050.png)
![image](https://user-images.githubusercontent.com/31722512/166259861-3ec7ce4e-2555-440d-aa3a-628d36a2f7d7.png)

## 빌드 프로세스 구성
- `빌드 전 단계`
  * 빌드를 하기 위한 준비 단계
  * 프로젝트 설정, 소스 코드 관리, 빌드 유발, 빌드 환경

- `빌드 단계`
  * 실제로 빌드가 이루어지는 단계
  * Unity3D, Xcode, Ant, Gradle, Shell, Maven, MSBuild

- `빌드 후 조치`
  * 빌드가 끝난 후 테스트, 배포, 알림 등의 작업을 하는 단계
  * ftp, 아마존 S3, Test Flight 등의 배포 작업
  * Email, Slack 등의 알림 작업

![image](https://user-images.githubusercontent.com/31722512/166260406-8b1012e4-89e2-4aa2-af45-bcfa3b97e076.png)
![image](https://user-images.githubusercontent.com/31722512/166260511-fa55a671-68d3-49cb-a15f-28bfe4c15dca.png)
![image](https://user-images.githubusercontent.com/31722512/166260650-a45b31c6-7161-4072-bf20-5c8a1cec2adb.png)

![image](https://user-images.githubusercontent.com/31722512/166260722-4c056b57-b567-45b0-ae9c-65793c99418d.png)
![image](https://user-images.githubusercontent.com/31722512/166260966-1c2a55cd-ad3a-4217-aa2b-81d1276a32ab.png)
![image](https://user-images.githubusercontent.com/31722512/166261061-c5bf5b01-d129-4f95-8e66-e96e3d74f4ee.png)
![image](https://user-images.githubusercontent.com/31722512/166261143-e919571f-a7ab-4d12-87da-23f6ac899666.png)
![image](https://user-images.githubusercontent.com/31722512/166261174-fb73cf56-c3a7-4d7d-b12c-a3c583515aad.png)
![image](https://user-images.githubusercontent.com/31722512/166261332-46561211-7d95-497a-b82e-9a236ddbde7c.png)
![image](https://user-images.githubusercontent.com/31722512/166261368-8a6b66da-b51b-48e5-b24b-52ef3b7150d5.png)

## Unity Cloud Build를 이용한 유니티 모바일 빌드 자동화
![image](https://user-images.githubusercontent.com/31722512/166261458-401a6be7-d117-422c-8f11-5b502c6a80b8.png)
![image](https://user-images.githubusercontent.com/31722512/166261544-b1bca002-8d74-4b3f-acab-4c4a0aca7820.png)
![image](https://user-images.githubusercontent.com/31722512/166261586-3f547e58-322f-412c-962c-3bfd1da55016.png)
![image](https://user-images.githubusercontent.com/31722512/166261608-b8eacaaa-4415-4630-ab2b-b990ba907589.png)
![image](https://user-images.githubusercontent.com/31722512/166261823-d8793b37-8212-4401-9070-1c3aef4466c0.png)
![image](https://user-images.githubusercontent.com/31722512/166261909-e2120acf-aef1-4293-bf55-ffcb64c37d68.png)
![image](https://user-images.githubusercontent.com/31722512/166261976-d10ebd76-d545-4fe2-9b03-2d184eaf145d.png)
![image](https://user-images.githubusercontent.com/31722512/166262148-996616b2-d163-47ac-a3bf-20813480655b.png)




















