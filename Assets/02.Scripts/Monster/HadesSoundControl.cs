using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HadesSoundControl : MonoBehaviour
{
    [SerializeField] private AudioClip triggerSound; // ����� ����
    private AudioSource audioSource;

    private void Start()
    {
        // AudioSource ������Ʈ �������� (���ٸ� �߰�)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Ʈ���ſ� ���� �� ȣ��
    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾� �Ǵ� ���ϴ� ������Ʈ�� Ʈ���ſ� ���Դ��� Ȯ��
        if (other.CompareTag("Player")) // Player �±׸� ���� ������Ʈ�� ���
        {
            PlaySound();
        }
    }

    // ���� ��� �Լ�
    private void PlaySound()
    {
        if (triggerSound != null && audioSource != null)
        {
            Debug.Log("Player entered the trigger area");  // �� �αװ� ��µǴ��� Ȯ��
            audioSource.clip = triggerSound;
            audioSource.Play();
        }
    }
}
