using UnityEngine;

public class GroundLooper : MonoBehaviour
{
    [Header("Ground Settings")]
    [SerializeField] private GameObject[] grounds; // 多个地面对象
    [SerializeField] private GameObject[] closeBackgrounds; // 近景背景对象
    [SerializeField] private GameObject[] midBackgrounds; // 中景背景对象
    [SerializeField] private GameObject[] farBackgrounds; // 远景背景对象
    [SerializeField] private BoxCollider checkBox; // 检查盒

    private float groundWidth;
    private float closeBackgroundWidth;
    private float midBackgroundWidth;
    private float farBackgroundWidth;

    private void Start()
    {
        if (grounds.Length > 0)
        {
            // 计算地面的宽度
            groundWidth = grounds[0].GetComponent<Renderer>().bounds.size.x;
        }

        if (closeBackgrounds.Length > 0)
        {
            // 计算近景背景的宽度
            closeBackgroundWidth = closeBackgrounds[0].GetComponent<Renderer>().bounds.size.x;
        }

        if (midBackgrounds.Length > 0)
        {
            // 计算中景背景的宽度
            midBackgroundWidth = midBackgrounds[0].GetComponent<Renderer>().bounds.size.x;
        }

        if (farBackgrounds.Length > 0)
        {
            // 计算远景背景的宽度
            farBackgroundWidth = farBackgrounds[0].GetComponent<Renderer>().bounds.size.x;
        }
    }

    private void Update()
    {
        foreach (GameObject ground in grounds)
        {
            CheckAndRepositionGround(ground);
        }

        foreach (GameObject closeBackground in closeBackgrounds)
        {
            CheckAndRepositionBackground(closeBackground, closeBackgroundWidth, closeBackgrounds.Length);
        }

        foreach (GameObject midBackground in midBackgrounds)
        {
            CheckAndRepositionBackground(midBackground, midBackgroundWidth, midBackgrounds.Length);
        }

        foreach (GameObject farBackground in farBackgrounds)
        {
            CheckAndRepositionBackground(farBackground, farBackgroundWidth, farBackgrounds.Length);
        }
    }

    private void CheckAndRepositionGround(GameObject ground)
    {
        // 获取地面的世界位置
        Vector3 groundPosition = ground.transform.position;
        // 获取检查盒的世界位置和尺寸
        Vector3 checkBoxPosition = checkBox.transform.position;
        Vector3 checkBoxSize = checkBox.bounds.size;

        // 检查地面是否移出检查盒的一半
        if (groundPosition.x + groundWidth  < checkBoxPosition.x - checkBoxSize.x / 2- checkBoxSize.x)
        {
            // 将地面移动到右边
            ground.transform.position = new Vector3(groundPosition.x + grounds.Length * groundWidth, groundPosition.y, groundPosition.z);
        }
        else if (groundPosition.x - groundWidth  > checkBoxPosition.x + checkBoxSize.x / 2+ checkBoxSize.x)
        {
            // 将地面移动到左边
            ground.transform.position = new Vector3(groundPosition.x - grounds.Length * groundWidth, groundPosition.y, groundPosition.z);
        }
    }

    private void CheckAndRepositionBackground(GameObject background, float backgroundWidth, int numberOfBackgrounds)
    {
        // 获取背景的世界位置
        Vector3 backgroundPosition = background.transform.position;
        // 获取检查盒的世界位置和尺寸
        Vector3 checkBoxPosition = checkBox.transform.position;
        Vector3 checkBoxSize = checkBox.bounds.size;

        // 检查背景是否移出检查盒的一半
        if (backgroundPosition.x + backgroundWidth  < checkBoxPosition.x - checkBoxSize.x / 2 - checkBoxSize.x)
        {
            // 将背景移动到右边
            background.transform.position = new Vector3(backgroundPosition.x + numberOfBackgrounds * backgroundWidth, backgroundPosition.y, backgroundPosition.z);
        }
        else if (backgroundPosition.x - backgroundWidth  > checkBoxPosition.x + checkBoxSize.x / 2 + checkBoxSize.x)
        {
            // 将背景移动到左边
            background.transform.position = new Vector3(backgroundPosition.x - numberOfBackgrounds * backgroundWidth, backgroundPosition.y, backgroundPosition.z);
        }
    }
    //ok, it is the same shit codes and works same
}
