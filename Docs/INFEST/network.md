# INFEST 상세
# 🌐 네트워크 동기화 - 최소 자원으로 동기화를 꾀하다
Fusion 2 공식 문서에 따르면, 네트워크 속성은 NetworkBehaviour를 상속한 클래스의 자동 속성(auto-property)에 [Networked] 어트리뷰트를 추가하여 정의합니다. 이 어트리뷰트는 Fusion이 해당 속성의 getter와 setter를 네트워크 객체의 상태 메모리 버퍼와 자동으로 연결하도록 IL 코드를 생성하도록 지시합니다. 이에 따라 플레이어 HP와 같이 모든 클라이언트가 공유해서 확인해야 하는 데이터는 [Networked] 속성으로 정의하여 네트워크 상태로 관리했습니다.


### 애니메이션 트리거, 피격 여부 등 ON/OFF 성격의 상태 플래그 동기화에는 NetworkBool과 OnChangedRender 콜백을 사용했습니다.


(1) 공식 문서에 따르면 Photon Fusion 2의 NetworkBool은 C#의 bool이 플랫폼마다 크기가 일관되지 않을 수 있다는 문제를 보완하며, 단일 bit로 올바르게 직렬화됩니다. 또한 아키텍처 설명에 따르면 Fusion 2는 NetworkObject에서 변경된 데이터만 전송하는 궁극적 일관성 모델을 사용하여 전송해야 하는 데이터를 최소화합니다. 이에 저는 NetworkBool이 그 상태가 변화할 때만 1bit 크기(혹은 그에 준하는 수준)의 네트워크 전송량을 차지할 것이라 기대했으며(공식적으로 명시된 수치는 없어 확신할 수는 없었습니다), 만약 그렇다면 이는 애니메이션 트리거나 피격 여부 등 ON/OFF 성격의 상태 플래그에 대한 읽기·쓰기와 논리적으로 1:1로 대응하기에 NetworkBool이 가장 높은 수준의 최적화 선택지라고 판단했습니다. [Monster_RageFang.cs](Monster_RageFang.cs)

(1-1) 대안으로 RPC가 있었는데 이는 함수 호출 단위의 헤더 및 메타데이터가 포함되어 큰 사이즈를 가집니다. 때문에 NetworkBool이 정말로 1bit의 전송량을 차지하지 않는다고 하더라도 RPC에 비하면 효율은 매우 뛰어납니다.

(2) 상태 변경 시에는 Fusion 2에서 Change Detection in Render로 분류되는 OnChangedRender 콜백을 사용하여, 네트워크 속성의 변화가 감지될 경우 로컬에서 즉시 애니메이션을 갱신하는 함수를 실행했습니다. 이를 통해 RPC 호출 없이도 상태 변화 함수를 호출 해 최소한의 전송 규모로 시각적 동기화를 구현할 수 있었습니다.  [Monster_RageFang.cs](Monster_RageFang.cs)

(2-1) 다만 Change Detection 방식의 특성상, 값이 매우 짧은 간격으로 두 번 토글될 경우 첫 번째 변경이 누락될 수 있는 구조적 한계가 존재합니다. 이 문제는 int 부호 값을 사용해 불리언 상태를 인코딩하고, 절댓값을 통해 변경 횟수나 시점과 같은 추가 정보를 저장하는 방식으로 해결할 수 있습니다. 그러나 본 프로젝트에서는 애니메이션 시작 시 true, 종료 시 false로 상태를 변경하며, 두 토글 사이에 충분한 시간 간격이 확보되므로 실제 동기화 누락 위험은 없다고 판단해 고급 불리언 처리 방식에 추가 자원을 사용하지 않았습니다. 마지막으로 OnChangedRender는 리모트 객체가 최초 스폰될 때 자동으로 호출되지 않는 특성이 있으므로, Spawned() 함수에서 초기 상태를 명시적으로 반영하는 초기화 로직을 추가했습니다.

(3) 비슷한 이유로 점수판 UI에서 유저 데이터를 동기화하기 위해 NetworkDictionary를 사용했습니다. NetworkBool과 마찬가지로 네트워크 속성으로 분류되는 네트워크 컬렉션인 NetworkDictionary는 유저(key)–점수(value) 형태의 엔트리 데이터를 동기화하는 데 논리적으로 1:1로 대응됩니다. 가능한 최적의 선택지라고 판단했으며, OnChangedRender 콜백을 활용해 데이터가 변경될 때마다 UI 점수판을 갱신하도록 구현했습니다. [GamePlayerHandler.cs](GamePlayerHandler.cs)

> #### 📄 참조 문서  
> [> Client-Server Player Input - 'C# does not enforce a consistent size for bools across platforms so NetworkBool is used to properly serialize it as a single bit'](https://doc.photonengine.com/ko-kr/fusion/current/manual/data-transfer/player-input)  
> [> Networked Properties - 'Networked Properties are properties of a NetworkBehaviour derived class that define the networked State of its associated Network Object.'](https://doc.photonengine.com/ko-kr/fusion/current/manual/data-transfer/networked-properties )  
> [> Network Buffers - 'The [Networked] properties on a NetworkBehaviour provides direct access to the most recent of these snapshots (whether this is in the history or the local simulated state).'](https://doc.photonengine.com/ko-kr/fusion/current/manual/advanced/network-buffers)  
> [> Eventual Consistency - 'An Eventual Consistency model is used, which minimises the data which must be transmitted, as it sends only the changes in the data, for NetworkObjects which have changed.'](https://doc.photonengine.com/ko-kr/fusion/current/manual/data-transfer/eventual-consistency)  
> [> Change Detection - 'The OnChangedRender attribute is the easiest way for handling Render based Change Detection.'](https://doc.photonengine.com/ko-kr/fusion/current/manual/data-transfer/change-detection)  

### 게임 시작 투표, 상점 열람, 무기 구매 등의 단발성 상호작용 함수는 RPC로 실행했습니다.

(1) 초기 설계 단계에서는 게임 시작 투표, 상점 열람, 무기 구매와 같은 클라이언트 상호작용 입력과 관련 파라미터를 NetworkInput으로 전달하고, 이를 서버(State Authority)에서 처리하는 구조를 고려했습니다. 클라이언트 입력을 서버에서 일괄 처리한 뒤, 그 결과만을 네트워크 프로퍼티에 반영한다면 RPC 사용을 최소화하면서도 동기화가 가능할 것이라 판단했기 때문입니다. 그러나 Fusion 2의 입력 구조체는 신뢰되지 않는 메시지로 전송되며, 패킷 손실이 발생할 수 있다는 점이 문제였습니다. 또한 단발성 상호작용 처리를 위해 입력 구조체를 사용하는 것이, 호출 빈도가 낮은 RPC에 비해 명확한 성능적 이점을 가진다고 판단하기도 어려웠습니다. 입력 구조체는 매 Tick 전송되는 스트림 데이터에 적합한 반면, 해당 상호작용들은 이벤트성 메시지에 가깝기 때문입니다. 이에 따라 Fusion 2에서 전송과 실행이 공식적으로 보장되는 신뢰성 높은 RPC(Reliable RPC) 를 사용해 단발성 상호작용을 처리하는 방식으로 방향성을 확립했습니다. 실제로 이후 개발 과정에서 네트워크 모니터링을 진행하며 패킷 손실이 발생하는 상황을 빈번히 확인했고, 이로 인해 게임 시작 투표와 같은 단일·일회성 상호작용의 실행을 보장할 수 없다는 한계를 확인했습니다. [Store.cs](Store.cs) [StoreController.cs](StoreController.cs)

> #### 📄 참조 문서  
> [> Remote Procedure Calls - 'RPCs (Remote Procedure Calls) are ideal for sharing punctual game events; in contrast [Networked] properties are the go-to solutions for sharing state between network clients that are undergoing continuous change.'](https://doc.photonengine.com/fusion/current/manual/data-transfer/rpcs?utm_source=chatgpt.com)  
> [> Fusion Namespace - 'RpcHeader: Header for RPC messages'](https://forum.photonengine.com/discussion/20514/fusion-and-collisions-ontriggerenter-and-ontriggerexit-questions)  

<br></br>

### INetworkRunnerCallbacks로 유저 입장/퇴장 이벤트 및 입력을 처리했습니다.

(1) 세션 접속/퇴장과 같은 이벤트 및 Client-Server 입력 처리를 INetworkRunnerCallbacks에서 수행했습니다. [NetworkRunnerCallbacks.cs](NetworkRunnerCallbacks.cs)

# 🌐 네트워크 토폴로지 - 매칭과 플레이에 Shared와 Host의 각기 다른 토폴로지를 적용하다

### Host 토폴로지를 사용해 게임 플레이 시스템을 구현했습니다.

게임 INFEST는 대규모 좀비 웨이브가 동시에 발생하는 복잡한 네트워크 환경을 요구하며, 슈팅 FPS 특성상 오브젝트의 이동이나 공격 판정에 작은 오차만 발생해도 플레이어가 즉각적으로 인지할 수 있습니다. 따라서 높은 입력 반응성과 함께 정확하고 일관된 동기화 환경이 필수적이었습니다. Photon Fusion 2 공식 문서에서는 FPS 장르의 네트워크 토폴로지로 Shared 토폴로지를 추천하고 있습니다. Shared 토폴로지는 클라이언트가 직접 캐릭터 이동과 카메라 회전을 처리할 수 있어 입력 지연이 최소화되며, 정적인 협동(Co-op) 중심의 FPS나 VR FPS와 같이 반응성이 중요한 장르에 적합합니다. 그러나 INFEST는 수십~수백 마리의 좀비 AI가 동시에 행동하는 구조를 가지고 있으며, 이들을 Shared 토폴로지에서 관리할 경우 네트워크 권한이 여러 클라이언트로 분산되어 상태 불일치와 판정 오류가 발생할 가능성이 높습니다. 그 결과, 플레이어가 좀비를 명중시켰음에도 타격이 인정되지 않거나, 시각적으로 존재하지 않는 좀비에게 피격되는 등 치명적인 게임 플레이 오류로 이어질 수 있습니다. 이에 따라, 모든 게임 로직과 동기화를 서버 권한 하에 일관되게 처리할 수 있는 Host 토폴로지를 채택하여 대규모 AI 웨이브 환경에서도 안정적이고 신뢰 가능한 판정 시스템을 구현했습니다. [PlayGameListener.cs](PlayGameListener.cs) 

### Shared 토폴로지를 사용해 매치 메이킹 시스템을 구현했습니다.

(1) 기획 명세에 따르면 매칭 페이즈에서는 모든 플레이어가 동등한 권한을 가지되, 게임 시작과 강퇴 기능만은 한 명의 방장이 담당하는 구조였습니다. 초기에는 매칭부터 게임 플레이까지의 흐름을 끊김 없이 유지하기 위해, 게임 플레이와 동일한 Host 토폴로지를 사용하는 방안을 고려했습니다. 그러나 Host 토폴로지는 방장 권한이 고정되며, Host가 세션을 이탈할 경우 세션 자체가 종료되는 구조적 한계를 가지고 있었습니다. 이는 “모든 플레이어가 동등한 권한을 가진다”는 기획 의도와 논리적으로 1:1로 대응되지 않으며, 추후 예기치 못한 기획–기술 간 충돌을 야기할 수 있다고 판단했습니다. 대안으로 Host Migration을 고려했으나, 제한된 개발 일정 내에서 새로운 네트워크 기술을 연구·적용하는 비용 대비 효율이 낮다고 판단했습니다. 이에 따라 기존에 특성을 충분히 파악한 Shared 토폴로지를 채택했습니다. Shared 토폴로지는 Fusion 2 공식 문서에서 모든 클라이언트가 권한을 분산하여 보유하고, 해당 권한을 자유롭게 양도할 수 있다고 명시되어 있어 기획 명세와 논리적으로 1:1로 대응했습니다. 또한 각 클라이언트가 권한을 가진 오브젝트에 대해 개별 시뮬레이션을 수행함으로써 발생할 수 있는 상태 불일치는, 매치 메이킹이라는 소규모 기능 범위와 정적인 네트워크 환경을 고려했을 때 충분히 감수 가능한 수준이라고 판단했습니다.
[Room.cs](Room.cs) [MatchManager.cs](MatchManager.cs)

### 📄 참조 문서  
[> Network Topologies - 'In Shared Authority, authority over network objects is distributed among all clients. Each client initially has State Authority over objects they spawn, but are free to release that State Authority to other clients.'](https://doc.photonengine.com/ko-kr/fusion/current/manual/network-topologies)  

<br></br>
