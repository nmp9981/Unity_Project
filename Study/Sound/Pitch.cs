using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class PitchRecog : MonoBehaviour
{
    public struct PeakData
    {
        public int sampleIndex;
        public float amplitude;
    }

    private AudioSource audioSource;
    int frequency = 44100;
    public float sensitivity = 100;
    public float loudness = 0;

    private float[] samples;//FFT결과를 저장할 배열, 길이는 2의 제곱 수, 길수록 정확
    private float[] smoothSamples;

    private float maxFreq;
    public float threshold = 0.01f;

    const int sampleCount = 4096;
    public List<PeakData> peaks;
    private float[] noteFreqs;
    private string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    [SerializeField] TextMeshProUGUI soundHZText;

    bool isPlay;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        isPlay = false;
        //가능한 모든 음높이의 주파수를 구하고, 반복문으로 범위 체크하기
        noteFreqs = new float[108]; // 12 * 9 = 108
        for (int i = 0; i < 12 * 9; i++)
        {
            noteFreqs[i] = 440f * Mathf.Pow(2, (i - 57) / 12.0f);
        }
       
    }
    void Start()
    {
        /*
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 100, frequency);
        while (!(Microphone.GetPosition(null) > 0)) { }
        audioSource.Play();
        */

        // 최대 주파수: (Sample Rate) / 2
        maxFreq = AudioSettings.outputSampleRate * 0.5f;//24000
        samples = new float[sampleCount];
        smoothSamples = new float[sampleCount];
        peaks = new List<PeakData>();
    }

    // Update is called once per frame
    void Update()
    {
        PitchRecord();
    }
    void PitchRecord()
    {
        if (!isPlay) return;//재생중일 경우만
       
        audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);//주파수 대역의 진폭 가져오기
        peaks.Clear(); // 피크 리스트 초기화

        for (int i = 0; i < sampleCount; i++)
        {
            smoothSamples[i] = Mathf.Lerp(smoothSamples[i], samples[i], Time.deltaTime * 10);
            if (samples[i] > threshold)//일정 값을 넘을 경우 (인덱스, 진폭)를 인덱스에 추가
            {
                peaks.Add(new PeakData
                {
                    sampleIndex = i,
                    amplitude = smoothSamples[i]
                });
            }
        }


        if (peaks.Count > 0)
        {
            // 진폭을 기준으로 내림차순 정렬
            peaks = peaks.OrderByDescending(obj => obj.amplitude).ToList();
            //peaks.Sort((a, b) => -a.amplitude.CompareTo(b.amplitude));
            Debug.Log(peaks[0].sampleIndex);
            //리스트의 첫 번째 요소를 피크로 정한다.
            float peakFreq = ((float)peaks[0].sampleIndex / (float)sampleCount) * maxFreq;
            Debug.Log(maxFreq+"@@@@" + peakFreq);
            int noteNumber = ToNoteNumber(peakFreq); //피크 데이터의 주파수를 구한다.

            string note = noteNames[noteNumber % 12];//음
            int octave = noteNumber / 12;//옥타브

            string text = $"{note}{octave}"+" high "+ noteNumber;
            soundHZText.text = text;
        }
        
    }
    
    private int ToNoteNumberLog(float freq)
    {
        return Mathf.RoundToInt(57 + 12 * Mathf.Log(freq / 440.0f, 2));
    }
    private int ToNoteNumber(float freq)
    {

        for (int i = 1; i < 107; i++)
        {
            float prev = noteFreqs[i - 1];
            float next = noteFreqs[i + 1];
            float current = noteFreqs[i];
            float min = (prev + current) / 2;
            float max = (next + current) / 2;


            if (min <= freq && freq <= max)
            {
                return i;
            }
        }

        return -1;
    }
    //재생
    public void PlaySound()
    {
        isPlay = true;
        audioSource.Play();
        Debug.Log("재생중");
    }

    //녹음
    public void RecordSound()
    {
        //기기 이름, 반복 여부, 녹음 시간, 주파수
        isPlay = false;
        audioSource.clip = Microphone.Start(Microphone.devices[0].ToString(), false, 100, frequency);
        Debug.Log("녹음중");
    }

}
