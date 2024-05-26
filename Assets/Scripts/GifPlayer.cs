using System.Collections;
using System.Diagnostics;
using System.Collections.Generic; // List<>�� ����ϱ� ���� �߰�
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour
{
    public RawImage gifDisplay; // GIF�� ǥ���� RawImage
    public string gifPath; // GIF ���� ���
    public string framesFolderPath; // ������ �̹��� ���� ���
    public float frameRate = 10.0f; // ������ ����Ʈ ���� (�ʴ� ������ ��)

    private List<Texture2D> gifFrames;
    private int currentFrame;
    private float frameDelay;
    private float nextFrameTime;

    void Start()
    {
        gifFrames = new List<Texture2D>();
        currentFrame = 0;

        // ������ �̹����� ������ ������ ���ٸ� ����
        if (!Directory.Exists(framesFolderPath))
        {
            Directory.CreateDirectory(framesFolderPath);
        }

        // GIF ������ ������ �̹����� ����
        StartCoroutine(ConvertGifToFrames(gifPath, framesFolderPath));
    }

    void Update()
    {
        if (gifFrames.Count > 0 && Time.time >= nextFrameTime)
        {
            gifDisplay.texture = gifFrames[currentFrame];
            currentFrame = (currentFrame + 1) % gifFrames.Count;
            nextFrameTime = Time.time + frameDelay;
        }
    }

    private IEnumerator ConvertGifToFrames(string gifPath, string outputFolderPath)
    {
        string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe"); // FFmpeg ���� ���� ���
        string arguments = $"-i \"{gifPath}\" \"{outputFolderPath}/frame_%04d.png\"";

        ProcessStartInfo startInfo = new ProcessStartInfo(ffmpegPath, arguments)
        {
            CreateNoWindow = true,
            UseShellExecute = false, // ���𷺼� ���
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    UnityEngine.Debug.Log("FFmpeg Output: " + e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    UnityEngine.Debug.LogError("FFmpeg Error: " + e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // ���μ����� ����� ������ ���
            while (!process.HasExited)
            {
                yield return null; // �� �������� ��ٸ��ϴ�.
            }

            UnityEngine.Debug.Log("FFmpeg ���μ��� �Ϸ��");

            // ������ �̹����� �ε�
            LoadGifFrames(outputFolderPath);
        }
    }

    private void LoadGifFrames(string folderPath)
    {
        string[] files = Directory.GetFiles(folderPath, "*.png");
        foreach (string file in files)
        {
            byte[] fileData = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            gifFrames.Add(texture);
        }

        if (gifFrames.Count > 0)
        {
            frameDelay = 1.0f / frameRate; // ������ ������ �ð� ���
            nextFrameTime = Time.time;
        }
        else
        {
            UnityEngine.Debug.LogError("������ �̹����� ã�� �� �����ϴ�.");
        }
    }
}
