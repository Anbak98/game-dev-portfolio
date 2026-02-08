

# 🌐 네트워크 토폴로지 - 매칭과 플레이에 Shared와 Host의 각기 다른 토폴로지를 적용하다

### Shared 토폴로지를 사용해 매치 메이킹 시스템을 구현했습니다.

기획 명세에 따르면 매칭 페이즈에선 모든 플레이어가 동등한 권한을 가지며, 한 명의 유저만 게임 시작과 강퇴의 권한을 가진 방장이 될 수 있었습니다. 처음에는 Host

### Host 토폴로지를 사용해 게임 플레이 시스템을 구현했습니다.

Host–Client 토폴로지는 호스트가 서버 역할을 수행하기 때문에, 호스트가 세션에서 퇴장할 경우 세션 자체가 종료되는 구조적 특성이 있습니다. 그러나 기획상 “누가 나가더라도 방이 파괴되지 않는다”는 요구사항이 있었고, 소규모 매칭에 적합하다고 판단된 Shared 토폴로지를 채택했습니다. 두 토폴로지의 특성과 차이에 기반해서 설( MatchManager.cs ) 매칭과 실제 게임 플레이의 네트워크 요구사항이 다르다고 판단하여, 매칭 단계에서는 Shared 토폴로지를 사용하고 게임 진입 시 Host–Client 토폴로지로 전환하는 구조를 구현했습니다. 기존 Shared 세션을 종료한 뒤, 방장 권한을 가졌던 클라이언트가 Host로 세션을 생성하고, 다른 클라이언트들은 해당 세션 생성 완료 시점에 맞춰 재연결하도록 설계했습니다.( GameStarter.cs )

INetworkRunnerCallbacks는 NetworkRunner가 시작 시 등록하는 콜백 인터페이스로, 플레이어 입·퇴장, 연결 상태 변화 등의 네트워크 이벤트가 발생하면 NetworkRunner가 해당 메서드를 호출합니다. 저는 이 콜백들을 구현하여 클라이언트 관점의 플레이어 입·퇴장 및 상태 관리 로직을 Room.cs에서 처리했습니다.   


### 📄 참조 문서  
[> 네트워크 토폴로지 - '공유 모드에서는 네트워크 객체에 대한 권한이 모든 클라이언트 간에 분배됩니다. 각 클라이언트는 자신이 생성한 객체에 대해 초기 상태 권한을 가지며, 해당 권한을 다른 클라이언트에게 양도할 수 있습니다. '](https://doc.photonengine.com/ko-kr/fusion/current/manual/network-topologies)  

<br></br>

# 🌐 네트워크 동기화 - 최소 자원으로 동기화를 꾀하다
Fusion 2 공식 문서에 따르면, 네트워크 속성은 NetworkBehaviour를 상속한 클래스의 자동 속성(auto-property)에 [Networked] 어트리뷰트를 추가하여 정의합니다. 이 어트리뷰트는 Fusion이 해당 속성의 getter와 setter를 네트워크 객체의 상태 메모리 버퍼와 자동으로 연결하도록 IL 코드를 생성하도록 지시합니다. 이에 따라 플레이어 HP와 같이 모든 클라이언트가 공유해서 확인해야 하는 데이터는 [Networked] 속성으로 정의하여 네트워크 상태로 관리했습니다.


### 애니메이션 트리거, 피격 여부 등 ON/OFF 성격의 상태 플래그 동기화에는 NetworkBool과 OnChangedRender 콜백을 사용했습니다.

1. 공식 문서에 따르면 Photon Fusion 2의 NetworkBool은 C#의 bool이 플랫폼마다 크기가 일관되지 않을 수 있다는 문제를 보완하며, 단일 bit로 올바르게 직렬화됩니다. 또한 아키텍처 설명에 따르면 Fusion 2는 NetworkObject에서 변경된 데이터만 전송하는 궁극적 일관성 모델을 사용하여 전송해야 하는 데이터를 최소화합니다. 이에 저는 NetworkBool이 그 상태가 변화할 때만 1bit 크기(혹은 그에 준하는 수준)의 네트워크 전송량을 차지할 것이라 기대했으며(공식적으로 명시된 수치는 없어 확신할 수는 없었습니다), 만약 그렇다면 이는 애니메이션 트리거나 피격 여부 등 ON/OFF 성격의 상태 플래그에 대한 읽기·쓰기와 논리적으로 1:1로 대응하기에 NetworkBool이 가장 높은 수준의 최적화 선택지라고 결론지었습니다. 대안으로 RPC가 있었는데 이는 함수 호출 단위의 헤더 및 메타데이터가 포함되어 큰 사이즈를 가집니다. 때문에 NetworkBool이 정말로 1bit의 전송량을 차지하지 않는다고 하더라도 RPC에 비하면 효율은 매우 뛰어납니다.

2. 상태 변경 시에는 Fusion 2에서 Change Detection in Render로 분류되는 OnChangedRender 콜백을 사용하여, 네트워크 속성의 변화가 감지될 경우 로컬에서 즉시 애니메이션을 갱신하는 함수를 실행했습니다. 이를 통해 RPC 호출 없이도 상태 변화 함수를 호출 해 최소한의 전송 규모로 시각적 동기화를 구현할 수 있었습니다. 다만 Change Detection 방식의 특성상, 값이 매우 짧은 간격으로 두 번 토글될 경우 첫 번째 변경이 누락될 수 있는 구조적 한계가 존재합니다. 이 문제는 int 부호 값을 사용해 불리언 상태를 인코딩하고, 절댓값을 통해 변경 횟수나 시점과 같은 추가 정보를 저장하는 방식으로 해결할 수 있습니다. 그러나 본 프로젝트에서는 애니메이션 시작 시 true, 종료 시 false로 상태를 변경하며, 두 토글 사이에 충분한 시간 간격이 확보되므로 실제 동기화 누락 위험은 없다고 판단해 고급 불리언 처리 방식에 추가 자원을 사용하지 않았습니다. 마지막으로 OnChangedRender는 리모트 객체가 최초 스폰될 때 자동으로 호출되지 않는 특성이 있으므로, Spawned() 함수에서 초기 상태를 명시적으로 반영하는 초기화 로직을 추가했습니다.

3. 비슷한 이유로 점수판 UI에서 유저 데이터를 동기화하기 위해 NetworkDictionary를 사용했습니다. NetworkBool과 마찬가지로 네트워크 속성으로 분류되는 네트워크 컬렉션인 NetworkDictionary는 유저(key)–점수(value) 형태의 엔트리 데이터를 동기화하는 데 논리적으로 1:1로 대응됩니다. 가능한 최적의 선택지라고 판단했으며, OnChangedRender 콜백을 활용해 데이터가 변경될 때마다 UI 점수판을 갱신하도록 구현했습니다. 

### 📄 참조 문서  
[> 네트워크 입력 - 'C#은 플랫폼 간에 bool 크기를 일관되게 강제하지 않으므로, NetworkBool을 사용하여 이를 단일 비트로 올바르게 직렬화합니다.'](https://doc.photonengine.com/ko-kr/fusion/current/manual/data-transfer/player-input)  
[> 네트워크 속성 - '네트워크 속성은 NetworkBehaviour를 상속받은 클래스의 속성으로, 관련된 네트워크 객체의 네트워크 상태를 정의합니다.'](https://doc.photonengine.com/ko-kr/fusion/current/manual/data-transfer/networked-properties )  
[> 네트워크 버퍼 - 'NetworkBehaviour의 [Networked] 속성은 이러한 스냅샷(기록에 있는 스냅샷이든 로컬 시뮬레이션 상태이든 상관없이) 중 가장 최신의 값에 직접 접근할 수 있게 해줍니다.'](https://doc.photonengine.com/ko-kr/fusion/current/manual/advanced/network-buffers)  
[> 궁극적 일관성 - 'NetworkObject에서 변경된 데이터만 전송하는 궁극적 일관성 모델을 사용하여 전송해야 하는 데이터를 최소화합니다.'](https://doc.photonengine.com/ko-kr/fusion/current/manual/data-transfer/eventual-consistency)  

<br></br>

# 🌐 네트워크 물리 작용 - Collider 충돌 이벤트를 처리하다

상점 시스템은 Collider 이벤트 기반으로, 플레이어와 충돌 시 UI가 열리는 구조였습니다. 다만 Photon Fusion2에 의해 클라이언트 단에서 물리 이벤트를 처리할 수 없기 때문에, 호스트 측에서 RPC를 통해 클라이언트를 제어하는 방식으로 구현했습니다. ( Store.cs )

<br></br>

# 🌐 기타

1인칭 카메라의 부드러운 움직임을 위해 KCC(Kinematic Character Controller)를 사용했습니다.
