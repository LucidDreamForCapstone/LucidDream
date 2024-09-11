using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HadesSoundControl : MonoBehaviour
{
    [SerializeField] private AudioClip triggerSound; // 재생할 사운드
    private AudioSource audioSource;

    private void Start()
    {
        // AudioSource 컴포넌트 가져오기 (없다면 추가)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 트리거에 들어갔을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 또는 원하는 오브젝트가 트리거에 들어왔는지 확인
        if (other.CompareTag("Player")) // Player 태그를 가진 오브젝트일 경우
        {
            PlaySound();
        }
    }

    // 사운드 재생 함수
    private void PlaySound()
    {
        if (triggerSound != null && audioSource != null)
        {
            Debug.Log("Player entered the trigger area");  // 이 로그가 출력되는지 확인
            audioSource.clip = triggerSound;
            audioSource.Play();
        }
    }
}
