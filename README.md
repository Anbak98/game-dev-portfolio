![alt text](./Assets/profile.png)

# 🧑 유승민 ( 1998.05.23 )

### 게임 클라이언트 개발자  

"하드웨어 관점의 최적화 기술과 기본기로 게임의 완성도를 보장하고, 이를 바탕으로 AI를 활용한 명세 기반의 빠른 개발이 가능한 개발자를 지향합니다" <br></br>

# 📁 PROJECT #1 

### 게임 "Anting" - 덱빌딩 개미 시뮬레이션 ([시연 영상](https://drive.google.com/file/d/11Cro0v9kvqwha7-t4IcAmy6IIKVdFj42/view?usp=sharing))
### 2026.02 - 개발 중 / 1인 개발 / PC / 모바일 / Unity
### 작업 개요
1. 10K 에이전트 규모의 대규모 연산에서 프레임 방어를 위해 캐시 지역성을 최적화. [상세](Docs/Anting/anting.md#캐시-지역성-최적화)
    1. 다차원일 때 물리적으로 분산되는 List<T> 및 가변 배열 대신 메모리에 물리적으로 연속되며 인덱싱 계산 최적화까지 가능한 평탄화된 1차원 배열 float[]의 사용
    2. 참조 타입 변수의 캐싱 및 클래스 개수 축소 및 interface 제거로 인한 virtual dispatch 방지로 핫 루프 내 pointer chain 최소화
    3. 향후 DOTS 기반 ECS를 활용해 에이전트를 물리적으로 연속 배치하거나 아니면 분기 없는 데이터 중심 구조로 GPU 기반 병렬 처리까지 확장헤 60FPS 프레임을 확보할 계획.
2. 파라미터 튜닝 기반의 통제 가능한 시스템을 구축하기 위해 확률 기반 메타 휴리스틱 알고리즘인 ACO를 반복적으로 시뮬레이션하며 패턴을 도출. [상세](Docs/Anting/anting.md#개미-군집-알고리즘의-컨텐츠화)
3. 20K 드로우 콜을 줄이기 위해 GPU Instancing 옵션을 활성화해 동일한 스프라이트들은 한 번의 드로우 콜로 렌더링하고, Sprite Atals로 여러 개의 스프라이트들을 하나의 텍스처에 몰아넣어 최종적으로 50 Batch까지 감소.[상세](Docs/Anting/anting.md#드로우-콜-최적화)
4. 친선 전투 같은 친구 기능을 구현하기 위한 사전 단계로 스팀웍스 API 적용.


### 작업 상세
- [상세보기](Docs/Anting/anting.md)

<br></br>

# 📁 PROJECT #2  

### 게임 "INFEST" - 멀티 FPS 좀비 슈터 ([시연 영상](https://youtu.be/ZbKr9C13bc4))
### 2025.05 - 2025.06 / 5인 개발 (팀장) / PC / Unity / Photon Fusion 2
### 작업 개요
1. ON/OFF 성격의 상태 플래그(e.g. 애니메이션 트리거)를 최소한의 자원으로 네트워크에 동기화하기 위해 단일 bit 직렬화 특성을 가진 Fusion2 NetworkBool 변수를 활용. [상세](./Docs/INFEST/network.md#1-애니메이션-트리거-피격-여부-등-onoff-성격의-상태-플래그-동기화에는-networkbool과-onchangedrender-콜백을-사용했습니다)
2. 렌더링을 최소한의 자원으로 동기화하기 위해 렌더 기반 Change Detection를 처리하는 OnChangeRender 콜백을 NetworkBool에 연결하여 RPC 없는 로컬 동기화를 구현. [상세](./Docs/INFEST/network.md#1-애니메이션-트리거-피격-여부-등-onoff-성격의-상태-플래그-동기화에는-networkbool과-onchangedrender-콜백을-사용했습니다)
3. 단발성 고성능 상호작용(e.g. 게임 시작 투표, 상점 이용)을 안정적으로 동기화하기 위해 패킷 손실 가능성이 있는 입력 구조체나 다양한 파라미터를 전달할 수 없는 Change Detection(OnChangeRender) 대신, 도착이 보장되는 신뢰성 있는 RPC를 통해 안정적으로 처리. [상세](./Docs/INFEST/network.md#2-게임-시작-투표-상점-열람-무기-구매-등의-단발성-상호작용-함수는-rpc로-실행했습니다)
4. 게임 플레이와 매치메이킹에서 기획 명세에 맞는 토폴로지를 적용하기 위해 Host와 Shared 토폴로지의 특성을 일관성과 권한 분산으로 분석하고 구현. [상세](./Docs/INFEST/network.md#-네트워크-토폴로지---매칭과-플레이에-shared와-host의-각기-다른-토폴로지를-적용하다)
5. 비경험자 팀원과 협업하기 위해 R&D를 선행하고 기반 시스템을 구축한 뒤, 세부 작업을 팀원에게 안정적으로 인계.
### 작업 상세
- [네트워크 작업 상세 보기](./Docs/INFEST/network.md)
- [트러블슈팅 상세 보기](./Docs/INFEST/trouble.md)
- [포스트모템 보고서](./Docs/INFEST/INFEST_Postmortem_Report.pdf)

<br></br>

# 📁 PROJECT #3

### 게임 "수레기 머학생" - 종합 퍼즐 순위 경쟁
### 2025.07 - 2025.09 / 5인 개발 (클라이언트) / 모바일 / Unity / 객체 지향 프로그래밍 / 리팩토링 / 프레임워크 구현 / 반응형 UI
### 작업 개요
1. 협업을 위해 2천 줄의 메인 게임 스크립트를 10개의 스크립트로 분리하는 단일 책임 원칙 기반 리팩토링 진행. 개발자 두 명이 작업 중인 스크립트를 서로 건드려 충돌나거나 사전에 합의하면서 생기는 비용을 최소화. [상세](Docs/TrashStudent/framework.md#2천-줄에-달하던-메인-게임-스크립트를-10개의-스크립트로-분리하는-단일-책임-리팩토링을-진행)
2. 출시를 대비한 유지보수성 증진를 위해 기존의 5개 미니 게임 각각에서 따로 처리하던 승리, 보상, 기록의 핵심 기능을 하나의 프레임워크로 통합해 신뢰성과 확장성 확보. 보상 획득 기능을 모든 미니 게임 스크립트에 추가하다 로직이 달라지거나 하는 등의 상황을 하나의 프레임워크로 해결. [상세](Docs/TrashStudent/framework.md#한-번의-작업을-수-십-개의-미니게임에서-반복하던-상황을-해결하기-위해-통합-프레임워크-구현)
3. 다양한 모바일 해상도에도 불구하고 1920*1020으로 고정되던 해상도가 화면에 빈 부븐을 만들어 옛날 게임의 느낌을 준다는 점을 개선하기 위해 pivot·anchor을 다시 설정하고, 기기별 해상도에 대응하는 match 값 조정과 기기별로 다른 노치 영역에 대한 대응 및 UI와 오브젝트 스케일 조정을 스크립트로 제어해 다양한 화면 비율에서도 일관된 UX를 확보
### 작업 상세
- [작업 상세 보기](Docs/TrashStudent/framework.md)
<!-- - [작업 개요 보기](./Docs/TrashStudent/수레기머학생_포폴_4페이지.pdf) -->

<br></br>

<!-- # 📁 PROJECT #4

### 연구 "Artificial Hunter Vision" - 시야 기반 강화학습 에이전트
### 기술 스택 : Unity ML-Agent / Pytorch / Flask / 컴퓨터 비전 & 이미지 세그멘테이션 
### 작업 개요
- 시야 기반 강화학습 에이전트를 구현하기 위해 Unity ML-Agent 플러그인 활용.
- 복합 환경에서의 에이전트 성능을 향상시키기 위해 Pytorch를 활용해 Image Segmentation 모델 학습.
- 모델 학습을 위한 데이터 확보를 위해 유니티 시뮬레이션을 반복하며 Render Texture로 이미지를 생성하고 Occlusion Culling과 Shader를 이용해 라벨 데이터 확보.
- Unity와 Pytorch를 연결하기 위해 Flask로 통신 채널 구축.
- [작업 개요 보기](./Docs/ArtificialHunterVision/AHV_포폴.pdf) -->

<br></br>

# ➕ 더보기
### [유승민의 게임 아카이브 바로가기](https://anbak98.github.io/timeline)
### [유승민의 개발 아카이브 바로가기](https://www.notion.so/20942be5b78c80c49a3bf0aa55328543?source=copy_link)
### [유승민의 블로그 바로가기](https://anbak.tistory.com/)
### [유승민의 레딧 바로가기](https://www.reddit.com/user/Anbak98/)