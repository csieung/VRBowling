# VRBowling

졸업 작품으로 진행한 VR 볼링 게임

# 목적

사용자의 볼링 자세를 분석하는 모델을 통해 영상 분석을 VR 게임 환경 속에서 제공하는 모습이다.
초보자의 경우 볼링공의 무게로 인한 부상을 방지하기 위해 VR로 재밌게 볼링 게임을 즐길 수 있다.
  
# 구현 모습

1. 볼링장 제작
   
  ![image](https://github.com/csieung/VRBowling/assets/72512163/636e7f2a-51eb-4e38-ae3a-eaca069e39d4)


2. 볼링공 집기 - Grab Interactable

  ![image](https://github.com/csieung/VRBowling/assets/72512163/34e5932e-9718-4b6b-b580-7e6fdf34c0a7)


3. 웹캡 촬영 시작 - 오른쪽 트리거 당기기

  ![image](https://github.com/csieung/VRBowling/assets/72512163/a87233d2-9902-424a-8f38-74ba7cdf9225)

  오른쪽 트리거를 당기면 촬영이 시작되고, 한번 더 당기면 촬영이 종료되며 프레임 단위로 저장된다.
  저장된 프레임들로 자세를 분석하고 다시 VR 게임 내에서 GIF 형식으로 출력되어 확인할 수 있다.


4. 사용자의 후방에서 촬영 - 웹캠 출력

  ![image](https://github.com/csieung/VRBowling/assets/72512163/89373b6f-0b5f-4094-a583-887c5c0859a6)


5. 볼링 자세 분석 후 점수와 정확도 출력

  ![image](https://github.com/csieung/VRBowling/assets/72512163/815792c3-11d5-4067-8c60-ea57764951ff)


6. 점수 확인

  ![image](https://github.com/csieung/VRBowling/assets/72512163/b1128ed4-ac67-4009-a93d-1202de95610a)

  볼링 게임이 완료 된 후, 점수를 확인할 수 있다.


7. 게임 재시작

  ![image](https://github.com/csieung/VRBowling/assets/72512163/e7ff044c-f15e-46c6-9b02-518fede75b85)
  ![image](https://github.com/csieung/VRBowling/assets/72512163/aee90cfb-7f39-446e-bf6d-5823b9ff3633)

  왼쪽 트리거를 당기면 볼링공과 볼링핀의 위치가 초기화되어 다시 게임을 진행할 수 있다.
  
