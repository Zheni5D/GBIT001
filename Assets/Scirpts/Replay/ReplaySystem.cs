using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TurboJpegWrapper;
using System.IO.Compression;

public class ReplaySystem : MonoBehaviour
{
    public bool turnOn = false;
    private int stopThresold = 300 * 1024 * 1024;
    public Camera recordCamera;
    public RawImage replayScreenContainer;
    public ComputeShader encodeHelper, decodeHelper;
    public static bool isReplayFin;

    private int screenWidth, screenHeight;
    private byte[] currentFrameData;
    private string[] currentFrameAudios;

    private TJCompressor compressor = new TJCompressor();
    private TJDecompressor decompressor = new TJDecompressor();
    private ComputeBuffer computeBuffer;

    private MemoryStream stageReplayStream;
    private MemoryStream levelReplayStream;
    private BinaryWriter stageReplayWriter;
    private BinaryWriter levelReplayWriter;

    private bool isRecording;
    private bool isPausing;
    private Coroutine replayCorotine;

    private void OnEnable()
    {
        if (LevelManager.isInitialize)
        {
            turnOn = LevelManager.Instance.enableReplay;
        }

        if (turnOn)
        {
            MessageCenter.AddListener(OnLevelClear);
            MessageCenter.AddListener(OnStageClear);
            MessageCenter.AddListener(OnTimeStopOff);
            MessageCenter.AddListener(OnTimeStopOn);
            MessageCenter.AddListener(OnGameRestart);
            MessageCenter.AddListener(OnMoveStage);
            MessageCenter.AddListener(OnGamePauseOff);
            MessageCenter.AddListener(OnGamePauseOn);
        }
    }

    public bool IsReplaying()
    {
        return replayCorotine != null;
    }

    private void OnDestroy()
    {
        if (turnOn)
        {
            MessageCenter.RemoveListner(OnStageClear);
            MessageCenter.RemoveListner(OnLevelClear);
            MessageCenter.RemoveListner(OnTimeStopOff);
            MessageCenter.RemoveListner(OnTimeStopOn);
            MessageCenter.RemoveListner(OnGameRestart);
            MessageCenter.RemoveListner(OnMoveStage); 
            MessageCenter.RemoveListner(OnGamePauseOff);
            MessageCenter.RemoveListner(OnGamePauseOn);
        }
    }

    void Start()
    {
        if (turnOn)
        {
            Application.targetFrameRate = 60;
            screenWidth = recordCamera.targetTexture.width;
            screenHeight = recordCamera.targetTexture.height;

            stageReplayStream = new MemoryStream();
            levelReplayStream = new MemoryStream();
            stageReplayWriter = new BinaryWriter(stageReplayStream);
            levelReplayWriter = new BinaryWriter(levelReplayStream);
            currentFrameData = new byte[recordCamera.targetTexture.width * recordCamera.targetTexture.height * 4];
            
            replayScreenContainer.gameObject.SetActive(false);
            StartRecord();
        }
    }

    void LateUpdate()
    {
        if (turnOn && isRecording)
        {
            if (levelReplayStream.Position + stageReplayStream.Position > stopThresold)
            {
                EndRecord();
            }
        }

        if (isRecording && !isPausing)
        {
           RecordFrame();
        }
    }

    private void StartRecord()
    {
        if (!isRecording)
        {
            isRecording = true;
            isReplayFin = false;
            SoundManager.currentClips.Clear();
        }
        else
        {
            Debug.LogWarning("Recording is in progress already");
        }
    }

    private void PauseRecord()
    {
        isPausing = true;
    }

    private void ResumeRecord()
    {
        isPausing = false;
    }

    private void ClearRecord()
    {
        stageReplayStream.Position = 0;
        stageReplayStream.SetLength(0);
    }

    private void MergeRecordFile()
    {
        levelReplayWriter.Write(stageReplayStream.ToArray());
        ClearRecord();
    }

    private void EndRecord()
    {
        isRecording= false;
        MergeRecordFile();
    }

    private void RecordFrame()
    {
        CaptureFrame();
        SaveFrame();
    }

    private void CaptureFrame()
    {
        currentFrameAudios = new string[SoundManager.currentClips.Count];
        if (SoundManager.currentClips.Count > 0)
        {
            for (int i = 0; SoundManager.currentClips.Count > 0; i++)
            {
                currentFrameAudios[i] = SoundManager.currentClips.Dequeue();
            }
        }

        int kernel = encodeHelper.FindKernel("Kernel");

        if (computeBuffer == null)
        {
            computeBuffer = new ComputeBuffer(screenWidth * screenHeight, sizeof(uint));
        }

        encodeHelper.SetTexture(kernel, "InputTexture", recordCamera.targetTexture);
        encodeHelper.SetBuffer(kernel, "OutputBuffer", computeBuffer);
        encodeHelper.Dispatch(kernel, screenWidth / 8, screenHeight / 8, 1);

        computeBuffer.GetData(currentFrameData);
        recordCamera.targetTexture.Release();
    }

    private void SaveFrame()
    {
        stageReplayWriter.Write(BitConverter.GetBytes(currentFrameAudios.Length));

        foreach (var audio in currentFrameAudios)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(audio);
            stageReplayWriter.Write(stringBytes.Length);
            stageReplayWriter.Write(stringBytes);
        }

        byte[] compressedData = Compress(compressor.Compress(currentFrameData, screenWidth * 4, screenWidth, screenHeight, TJPixelFormat.RGBA, TJSubsamplingOption.Chrominance420, 40, TJFlags.None));

        stageReplayWriter.Write(compressedData.Length);
        stageReplayWriter.Write(compressedData);
    }

    public void StartReplay()
    {
        if (replayCorotine == null) 
        {
            replayCorotine = StartCoroutine(Replay());
        }
    }

    public void EndReplay()
    {
        replayCorotine = null;
        replayScreenContainer.gameObject.SetActive(false);

        stageReplayStream.Close();
        levelReplayStream.Close();
        stageReplayWriter.Close();
        levelReplayWriter.Close();

        compressor = null;
        decompressor = null;


        isReplayFin = true;
        if (UIManager.isInitialize)
        {
            UIManager.Instance.LevelCmpAnim();
        }
    }

    private void DisplayFrame(Texture frame)
    {
        replayScreenContainer.texture = frame;
    }

    IEnumerator Replay()
    {
        replayScreenContainer.gameObject.SetActive(true);

        byte[] decompressedData;

        RenderTexture frameTexture = new RenderTexture(screenWidth, screenHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
        {
            enableRandomWrite = true
        };


        levelReplayStream.Position = 0;
        using BinaryReader reader = new BinaryReader(levelReplayStream);

        while (levelReplayStream.Position < levelReplayStream.Length)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }

            frameTexture.Release();

            int audioNum = reader.ReadInt32();

            for (int i = 0; i < audioNum; i++)
            {
                int nameDataLength = reader.ReadInt32();
                string clipName = new string(reader.ReadChars(nameDataLength));
                SoundManager.PlayAudio(clipName);
            }

            int textureDataLength = reader.ReadInt32();
            decompressedData = decompressor.Decompress(Decompress(reader.ReadBytes(textureDataLength)), TJPixelFormat.RGBA, TJFlags.None, out int width, out int height, out int stride);

            int kernel = decodeHelper.FindKernel("Kernel");

            computeBuffer.SetData(decompressedData);
            decodeHelper.SetTexture(kernel, "OutputTexture", frameTexture);
            decodeHelper.SetBuffer(kernel, "InputBuffer", computeBuffer);

            decodeHelper.Dispatch(kernel, screenWidth / 8, screenHeight / 8, 1);
            DisplayFrame(frameTexture);
            yield return null;
        }

        computeBuffer.Release();
        frameTexture.Release();
        frameTexture = null;
        EndReplay();
    }

    public byte[] Compress(byte[] inputBytes)
    {
        using (MemoryStream outputStream = new MemoryStream())
        {
            using (DeflateStream deflateStream = new DeflateStream(outputStream, CompressionMode.Compress))
            {
                deflateStream.Write(inputBytes, 0, inputBytes.Length);
            }
            return outputStream.ToArray();
        }
    }

    public byte[] Decompress(byte[] inputBytes)
    {
        using (MemoryStream inputStream = new MemoryStream(inputBytes))
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    deflateStream.CopyTo(outputStream);
                }
                return outputStream.ToArray();
            }
        }
    }

    public void OnStageClear(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.STAGE_CLEAR)
            return;
        MergeRecordFile();
    }

    public void OnLevelClear(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.LEVEL_CLEAR)
            return;

        if (isRecording)
        {
            StartCoroutine(StartReplayWithUI());
        }
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART)
            return;
        ClearRecord();
    }

    public void OnTimeStopOn(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.TIME_STOP_ON)
            return;
        Invoke(nameof(PauseRecord), .5f); // 0.5s：特效时间
    }

    public void OnTimeStopOff(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.TIME_STOP_OFF)
            return;
        ResumeRecord(); // 0.5s：特效时间
    }

    public void OnMoveStage(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE)
            return;

        MergeRecordFile();
    }

    public void OnGamePauseOn(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_ON) return;
        PauseRecord();   
    }

    public void OnGamePauseOff(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_OFF) return;
        ResumeRecord();   
    }

    IEnumerator StartReplayWithUI(){
        if(UIManager.isInitialize){
            UIManager.Instance.FadeIn(1f);
        }
        yield return new WaitForSeconds(1f);
        EndRecord();
        yield return new WaitForSeconds(1f);
        StartReplay();
        if(UIManager.isInitialize){
            UIManager.Instance.FadeOut(1f);
        }
    }
}