using System.Collections;
using System.Collections.Generic; // List<> 사용하기 위해
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class WebCam1 : MonoBehaviour
{
    public RawImage rawImage;
    public TextMeshProUGUI recordingStatusText;
    public TextMeshProUGUI accuracyText;
    //public RawImage gifDisplay; // GIF를 표시할 RawImage
    private WebCamTexture webCamTexture;
    private bool isRecording = false;
    private int frameCount = 0;
    private string frameDirectory;
    private float nextCaptureTime = 0.0f;
    private float captureInterval = 1.0f / 60.0f;
    private string ffmpegPath;
    private string pythonPath;
    private string processScriptPath;
    private bool triggerPressed = false;
    private int videoCount = 0;

    private List<float> accuracyValues = new List<float>(); // 정확도 값을 저장할 리스트
    private float accuracy = 0f; // 정확도 값

    // GIF 플레이
    public RawImage gifDisplay; // GIF를 표시할 RawImage
    private string gifPath; // GIF 파일 경로
    private string framesFolderPath; // 프레임 이미지 폴더 경로
    public float frameRate = 10.0f; // 프레임 레이트 설정 (초당 프레임 수)

    private string AccuracyScore;

    private List<Texture2D> gifFrames;
    private int currentFrame;
    private float frameDelay;
    private float nextFrameTime;

    private int gifcount = 0;

    private string initiatepath = @"C:\Users\428-3090\AppData\LocalLow\DefaultCompany\VRBowling";

    void Start()
    {
        ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe");
        pythonPath = @"C:\Users\428-3090\anaconda3\envs\py3_10\python.exe"; // Python 경로 설정
        gifPath = @"C:\Users\428-3090\Desktop\VRBowling_v2\model\output.gif";
        framesFolderPath = @"C:\Users\428-3090\Desktop\CNN_LSTM\model\gif";
        processScriptPath = @"C:\Users\428-3090\Desktop\CNN_LSTM\asdf\process.py"; // Python 스크립트 경로 설정
        WebCamDevice[] devices = WebCamTexture.devices;
        
        gifFrames = new List<Texture2D>();
        currentFrame = 0;

        if (devices.Length > 0)
        {
            webCamTexture = new WebCamTexture(devices[0].name);
            webCamTexture.requestedWidth = 1280;
            webCamTexture.requestedHeight = 720;

            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;
            webCamTexture.Play();

            RectTransform rt = rawImage.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(500, 40);
            rt.localEulerAngles = new Vector3(0, 270, -90);
            rawImage.rectTransform.localScale = new Vector3(1, -1, 1);

            frameDirectory = Path.Combine(Application.persistentDataPath, "Frames");
            if (!Directory.Exists(frameDirectory))
            {
                Directory.CreateDirectory(frameDirectory);
            }
        }
        else
        {
            Debug.LogError("No webcam found");
        }

        if (Directory.Exists(initiatepath))
        {
            // 디렉토리 내 모든 파일 삭제
            foreach (string file in Directory.GetFiles(initiatepath))
            {
                File.Delete(file);
            }

            Debug.Log($"Directory cleared: {initiatepath}");
        }

        if (recordingStatusText != null)
        {
            recordingStatusText.text = "■STOP";
            recordingStatusText.color = Color.blue;
        }

        if (accuracyText != null)
        {
            accuracyText.text = "Accuracy: N/A";
        }

        // 프레임 이미지를 저장할 폴더가 없다면 생성
        if (!Directory.Exists(framesFolderPath))
        {
            Directory.CreateDirectory(framesFolderPath);
        }

        
        LoadGifFrames(framesFolderPath);
        // GIF 파일을 프레임 이미지로 분할
        
    }

    void frame_clear()
    {
        if (Directory.Exists(@"C:\Users\428-3090\AppData\LocalLow\DefaultCompany\VRBowling\Frames"))
        {
            // 디렉토리 내 모든 파일 삭제
            foreach (string file in Directory.GetFiles(@"C:\Users\428-3090\AppData\LocalLow\DefaultCompany\VRBowling\Frames"))
            {
                File.Delete(file);
            }

            Debug.Log($"Frames cleared");
        }
    }

    void Update()
    {
        float rightTrigger = Input.GetAxis("RightTrigger");

        
        if (gifFrames.Count > 0 && Time.time >= nextFrameTime)
            {
                currentFrame = (currentFrame + 1) % gifFrames.Count;
                gifDisplay.texture = gifFrames[currentFrame];
                nextFrameTime = Time.time + frameDelay;
            }
                // 정확도 출력

        if (rightTrigger > 0.1f && !triggerPressed)
        {
            triggerPressed = true;

            if (!isRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
                //frame_clear();
                Debug.Log("컨버팅 시작");
                StartCoroutine(ConvertFramesToGIF());
                LoadGifFrames(framesFolderPath);
            }
        }

        if (rightTrigger <= 0.1f)
        {
            triggerPressed = false;
        }

        if (isRecording && Time.time >= nextCaptureTime)
        {
            CaptureFrame();
            nextCaptureTime = Time.time + captureInterval;
        }
    }

    void StartRecording()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            isRecording = true;
            frameCount = 0;
            nextCaptureTime = Time.time;
            Debug.Log("Recording started");

            if (recordingStatusText != null)
            {
                recordingStatusText.text = "●REC";
                recordingStatusText.color = Color.red;
            }
        }
    }

    void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            Debug.Log("Recording stopped");

            if (recordingStatusText != null)
            {
                recordingStatusText.text = "■STOP";
                recordingStatusText.color = Color.blue;
            }
        }
    }

    void CaptureFrame()
    {
        if (webCamTexture != null)
        {
            Texture2D frame = new Texture2D(webCamTexture.width, webCamTexture.height);
            frame.SetPixels(webCamTexture.GetPixels());
            frame.Apply();

            Texture2D rotatedFrame = RotateTexture(frame, true);

            byte[] bytes = rotatedFrame.EncodeToPNG();
            string filePath = Path.Combine(frameDirectory, $"frame_{frameCount:D04}.png");
            File.WriteAllBytes(filePath, bytes);
            frameCount++;

            Destroy(frame);
            Destroy(rotatedFrame);
        }
    }

    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
{
    int width = originalTexture.width;
    int height = originalTexture.height;
    Texture2D rotatedTexture = new Texture2D(height, width);

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if (clockwise)
            {
                rotatedTexture.SetPixel(y, width - x - 1, originalTexture.GetPixel(x, y));
            }
            else
            {
                rotatedTexture.SetPixel(height - y - 1, x, originalTexture.GetPixel(width - x - 1, y));
            }
        }
    }
    rotatedTexture.Apply();
    return rotatedTexture;
}

    IEnumerator ConvertFramesToGIF()
    {
        Debug.Log("컨버팅 함수 진입");
        videoCount++;
        string videoPath = @"C:\Users\428-3090\AppData\LocalLow\DefaultCompany\VRBowling" + $@"\output{videoCount}.mp4";
        string arguments = $"-framerate 10 -i {Path.Combine(frameDirectory, "frame_%04d.png")} -r 10 -c:v libx264 -pix_fmt yuv420p {videoPath}";

        ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe");
        if (!File.Exists(ffmpegPath))
        {
            Debug.LogError("FFmpeg 실행 파일을 찾을 수 없습니다.");
        }

        Debug.Log("RunFFmpeg 호출 전");
        // RunFFmpeg 호출 및 완료 대기
        yield return StartCoroutine(RunFFmpeg(arguments));
        Debug.Log("RunFFmpeg 호출 후");

        Debug.Log($"Video saved to: {videoPath}");

        DirectoryInfo di = new DirectoryInfo(frameDirectory);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        Debug.Log("Runposedetection 진입 전");
        yield return RunPoseDetectionProcess(videoPath);
        Debug.Log("Runposedetection 탈출");
    }
    
    private IEnumerator RunFFmpeg(string arguments)
    {
        Debug.Log("RunFFmpeg 함수 진입");

        ProcessStartInfo startInfo = new ProcessStartInfo(ffmpegPath, arguments)
        {
            CreateNoWindow = true,
            UseShellExecute = false, // false로 설정하여 리디렉션 사용
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            Debug.Log("FFmpeg 프로세스 시작됨");

            // 프로세스가 종료될 때까지 대기
            while (!process.HasExited)
            {
                yield return null; // 한 프레임을 기다립니다.
            }

            Debug.Log("FFmpeg 프로세스 완료됨");
        }
    }

    private IEnumerator RunPoseDetectionProcess(string videoPath)
    {
        Debug.Log("RunPoseDetection 진입");
        string arguments = $"{processScriptPath} {videoPath}";
        Debug.Log(arguments);

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonPath; // 또는 python.exe의 전체 경로
        start.Arguments = arguments;
        start.CreateNoWindow = true;
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;

        using (Process process = Process.Start(start))
        {
            // 프로세스가 비동기적으로 종료될 때까지 기다립니다.
            while (!process.HasExited)
            {
                yield return null;
            }

            // 표준 출력 읽기
            using (System.IO.StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                AccuracyScore = result;
                UnityEngine.Debug.Log(result);
            }

            // 표준 에러 읽기
            using (System.IO.StreamReader reader = process.StandardError)
            {
                string error = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    UnityEngine.Debug.LogError(error);
                }
            }
        }

        // 프로세스가 완료된 후 GIF를 프레임으로 변환하는 코루틴 시작
        StartCoroutine(ConvertGifToFrames(gifPath, framesFolderPath));
    }

    private IEnumerator ConvertGifToFrames(string gifPath, string outputFolderPath)
    {
        string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe"); // FFmpeg 실행 파일 경로
        string arguments = $"-i \"{gifPath}\" \"{outputFolderPath}/frame_%04d.png\"";

        if (Directory.Exists(outputFolderPath))
        {
            // 디렉토리 내 모든 파일 삭제
            foreach (string file in Directory.GetFiles(outputFolderPath))
            {
                File.Delete(file);
            }

            Debug.Log($"Directory cleared: {outputFolderPath}");
        }
        
        ProcessStartInfo startInfo = new ProcessStartInfo(ffmpegPath, arguments)
        {
            CreateNoWindow = true,
            UseShellExecute = false, // 리디렉션 사용
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // 프로세스가 종료될 때까지 대기
            while (!process.HasExited)
            {
                yield return null; // 한 프레임을 기다립니다.
            }
            
            LoadGifFrames(framesFolderPath);
            UnityEngine.Debug.Log("FFmpeg 프로세스 완료됨");
            accuracyText.text = "Accuracy: " + AccuracyScore;

            // 프레임 이미지를 로드
        }
    }

    private void LoadGifFrames(string folderPath)
    {
        gifFrames.Clear();
        string[] files = Directory.GetFiles(folderPath, "*.png");
        int count = 0;
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
    
    void OnDestroy()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }


}
