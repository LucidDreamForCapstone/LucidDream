using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GoToFirstScene : MonoBehaviour
{
    // 3�� �ڿ� 0�� ������ �̵��ϴ� �Լ�
    private async UniTask GoToSceneAfterDelay() {
        await UniTask.Delay(3000); // 3�� ��� (�и��� ����)
        SceneManager.LoadScene(0); // 0�� �� �ε�
    }

    public void SignalGotoFirstScene() {
        GoToSceneAfterDelay().Forget();
    }
}
