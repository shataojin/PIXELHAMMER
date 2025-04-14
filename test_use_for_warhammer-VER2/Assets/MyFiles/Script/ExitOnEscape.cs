using UnityEngine;

public class ExitOnEscape : MonoBehaviour
{
    void Update()
    {
        // 检查是否按下 ESC 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 打印一条调试信息
            Debug.Log("ESC pressed. Exiting the game...");

            // 如果在编辑器中运行，退出 Play 模式
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // 如果在构建的游戏中运行，退出游戏
            Application.Quit();
#endif
        }
    }
}
