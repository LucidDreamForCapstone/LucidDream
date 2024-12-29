using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GoToFirstScene : MonoBehaviour
{
    // 3초 뒤에 0번 씬으로 이동하는 함수
    private async UniTask GoToSceneAfterDelay() {
        await UniTask.Delay(3000); // 3초 대기 (밀리초 단위)
        SceneManager.LoadScene(0); // 0번 씬 로드
    }

    public void SignalGotoFirstScene() {
        GoToSceneAfterDelay().Forget();
    }
}
