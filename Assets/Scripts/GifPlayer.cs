using System.Collections;
using System.Diagnostics;
using System.Collections.Generic; // List<>를 사용하기 위해 추가
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour
{
    public RawImage gifDisplay; // GIF를 표시할 RawImage
    public string gifPath; // GIF 파일 경로
    public string framesFolderPath; // 프레임 이미지 폴더 경로
    public float frameRate = 10.0f; // 프레임 레이트 설정 (초당 프레임 수)

    private List<Texture2D> gifFrames;
    private int currentFrame;
    private float frameDelay;
    private float nextFrameTime;

    void Start()
    {
        gifFrames = new List<Texture2D>();
        currentFrame = 0;

        // 프레임 이미지를 저장할 폴더가 없다면 생성
        if (!Directory.Exists(framesFolderPath))
        {
            Directory.CreateDirectory(framesFolderPath);
        }

        // GIF 파일을 프레임 이미지로 분할
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
        string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe"); // FFmpeg 실행 파일 경로
        string arguments = $"-i \"{gifPath}\" \"{outputFolderPath}/frame_%04d.png\"";

        ProcessStartInfo startInfo = new ProcessStartInfo(ffmpegPath, arguments)
        {
            CreateNoWindow = true,
            UseShellExecute = false, // 리디렉션 사용
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

            // 프로세스가 종료될 때까지 대기
            while (!process.HasExited)
            {
                yield return null; // 한 프레임을 기다립니다.
            }

            UnityEngine.Debug.Log("FFmpeg 프로세스 완료됨");

            // 프레임 이미지를 로드
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
            frameDelay = 1.0f / frameRate; // 프레임 딜레이 시간 계산
            nextFrameTime = Time.time;
        }
        else
        {
            UnityEngine.Debug.LogError("프레임 이미지를 찾을 수 없습니다.");
        }
    }
}
