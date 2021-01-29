using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lasp;
using WindowsInput;
using WindowsInput.Native;
using System.Linq;

public class PitchTracker : MonoBehaviour
{
    public float pitchValue;
    public float highestPeak;

    public int binSize = 2048; // you can change this up, I originally used 8192 for better resolution, but I stuck with 1024 because it was slow-performing on the phone
    public float refValue = 0.1f;
    public float threshold = 0.01f;

    private SpectrumAnalyzer _spectrumAnalyzer;
    private AudioLevelTracker _audioLevelTracker;

    private List<Peak> peaks = new List<Peak>();
    float[] spectrum;
    int samplerate;
    public float DynRangeDB { get; set; }

    public Text display; // drag a Text object here to display values
    public bool mute = true;
    //public AudioMixer masterMixer; // drag an Audio Mixer here in the inspector

    void Start()
    {
        Application.targetFrameRate = 60;
        samplerate = AudioSettings.outputSampleRate;
        _spectrumAnalyzer = this.GetComponent<SpectrumAnalyzer>();
        _audioLevelTracker = this.GetComponent<AudioLevelTracker>();

        var devices = Lasp.AudioSystem.InputDevices;
        foreach (var device in devices)
        {
            Debug.Log($"Device: {device.Name}");
            if (device.Name.Contains("Focusrite"))
            {
                _audioLevelTracker.deviceID = device.ID;
                _audioLevelTracker.channel = 1;
                _spectrumAnalyzer.deviceID = device.ID;
                _spectrumAnalyzer.channel = 1;
            }
        }

        // Mutes the mixer. You have to expose the Volume element of your mixer for this to work. I named mine "masterVolume".
        // masterMixer.SetFloat("masterVolume", -80f);
        display = GameObject.Find("Text").GetComponent<Text>();
    }

    void Update()
    {
        AnalyzeSound();
        display.text = $"{DynRangeDB.ToString("F1")} dB\n" +
                $"Pitch: {(DynRangeDB > 0 ? pitchValue.ToString("F2") : "0")} Hz\n" +
                $"Highest Peak: {(DynRangeDB > 0 ? highestPeak.ToString() : "0")}";
        if (DynRangeDB > 0)
            Bistoukette();
    }

    void Bistoukette()
    {
        //var test = new InputSimulator();
        //test.Keyboard.KeyPress(VirtualKeyCode.SPACE);
    }

    void AnalyzeSound()
    {
        // get sound spectrum
        spectrum = _spectrumAnalyzer.logSpectrumArray.ToArray();
        //GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0f;
        for (int i = 0; i < binSize; i++)
        { // find max
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                peaks.Add(new Peak(spectrum[i], i));
                if (peaks.Count > 5)
                { // get the 5 peaks in the sample with the highest amplitudes
                    peaks.Sort(new AmpComparer()); // sort peak amplitudes from highest to lowest
                    peaks.Remove (peaks [5]); // remove peak with the lowest amplitude
                }
            }
        }
        float freqN = 0f;
        if (peaks.Count > 0)
        {
            //peaks.Sort (new IndexComparer ()); // sort indices in ascending order
            maxV = peaks[0].amplitude;
            int maxN = peaks[0].index;
            freqN = maxN; // pass the index to a float variable
            if (maxN > 0 && maxN < binSize - 1)
            { // interpolate index using neighbours
                var dL = spectrum[maxN - 1] / spectrum[maxN];
                var dR = spectrum[maxN + 1] / spectrum[maxN];
                freqN += 0.5f * (dR * dR - dL * dL);
            }
            highestPeak = peaks[peaks.Count -1].index;
        }
        else
            highestPeak = 0.0f;
        pitchValue = freqN * (samplerate / 2f) / binSize; // convert index to frequency
        pitchValue /= 100.0f;
        peaks.Clear();
    }
}

//Save somewhere the frequency + highest peak, if it is a one tap or a pressed or a pressed for a number of seconds input
//Then, when playing something, looking for the closest one