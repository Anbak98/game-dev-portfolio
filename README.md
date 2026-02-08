![alt text](./Assets/profile.png)

# 🧑 유승민 ( 1998.05.23 )

### 게임 클라이언트 개발자  

"AI를 명세 기반 개발에 활용해 게임 아이디어를 빠르게 검증하고, 탄탄한 기본기와 최적화를 통해 게임의 완성도를 끌어올리는 개발자를 지향합니다" <br></br>

# 📁 PROJECT #1 


### 게임 "INFEST" - 친구들과 함께 시원하게 총을 쏘며 스트레스를 푸는 게임
### 기술 스택 : PC / Unity / Photon Fusion 2 / 멀티 플레이 네트워크
### 작업 개요
- Fusion2의 NetworkBool 단일 bit 직렬화 특성을 활용하여, 애니메이션 트리거, 피격 여부 등 ON/OFF 성격의 상태 플래그를 효율적으로 동기화했습니다. [> 상세 1](./Docs/INFEST/network.md#1-애니메이션-트리거-피격-여부-등-onoff-성격의-상태-플래그-동기화에는-networkbool과-onchangedrender-콜백을-사용했습니다)
- 렌더 기반 Change Detection를 처리하는 OnChangeRender 콜백을 NetworkBool에 연결하여 상태 변화 시 즉시 대응함으로써, RPC 없이 단일 bit 기반의 최적화된 동기화를 구현했습니다. [> 상세 1](./Docs/INFEST/network.md#1-애니메이션-트리거-피격-여부-등-onoff-성격의-상태-플래그-동기화에는-networkbool과-onchangedrender-콜백을-사용했습니다)
- 게임 시작 투표, 레그돌, 무기 구매 등 단발성 고성능 상호작용은, 패킷 손실 가능성이 있는 입력 구조체나 다양한 파라미터를 전달할 수 없는 Change Detection(OnChangeRender) 대신, RPC를 통해 안정적으로 처리했습니다. [> 상세 2](./Docs/INFEST/network.md#2-게임-시작-투표-상점-열람-무기-구매-등의-단발성-상호작용-함수는-rpc로-실행했습니다)
- Host와 Shared 토폴로지의 특성(일관성 vs 권한 분산)을 분석하여, 기획 명세에 맞게 게임 플레이와 매치메이킹에 서로 다른 토폴로지를 적용했습니다. [> 상세 3](./Docs/INFEST/network.md#-네트워크-토폴로지---매칭과-플레이에-shared와-host의-각기-다른-토폴로지를-적용하다)
- 비전공자 팀원과 협업하기 위해 R&D를 선행하고 기반 시스템을 구축한 뒤, 세부 작업을 팀원에게 안정적으로 인계했습니다.
### 작업 상세
- [네트워크 작업 상세 보기](./Docs/INFEST/network.md)
- [포스트모템 보고서](./Docs/INFEST/INFEST_Postmortem_Report.pdf)

<br></br>

# 📁 PROJECT #2


### 게임 "수레기 머학생" - 다양한 게임으로 순위를 경쟁하는 아케이드 게임
### 기술 스택 : 모바일 / Unity / 객체 지향 프로그래밍 / 리팩토링 / 프레임워크 구현 / 반응형 UI
### 작업 개요
- [작업 개요 보기](./Docs/TrashStudent/수레기머학생_포폴_4페이지.pdf)

<br></br>

# 📁 PROJECT #3


### 연구 "Artificial Hunter Vision" - 인공지능에게 눈을 주다
### 기술 스택 : Unity ML-Agent / Pytorch / Flask / 컴퓨터 비전 & 이미지 세그멘테이션 
### 작업 개요
- [작업 개요 보기](./Docs/ArtificialHunterVision/AHV_포폴.pdf)

<br></br>

# ➕ 더보기
![alt text](./Assets/image.png)
![alt text](./Assets/image-2.png)
### [유승민의 게임 아카이브 바로가기](https://anbak98.github.io/timeline)

<br></br>

--- 

![alt text](./Assets/image-3.png)
### [유승민의 개발 아카이브 바로가기](https://www.notion.so/20942be5b78c80c49a3bf0aa55328543?source=copy_link)

<br></br>
---
![alt text](./Docs/Anting/anting.png)

<br></br>
---

### [유승민의 블로그 바로가기](https://anbak.tistory.com/)
### [유승민의 레딧 바로가기](https://www.reddit.com/user/Anbak98/)