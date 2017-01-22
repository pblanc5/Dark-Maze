using UnityEngine; 
using System.Collections;

public class AudioVisualizer : MonoBehaviour 
{
	//An AudioSource object so the music can be played
	private AudioSource aSource;
	//A float array that stores the audio samples
	public float[] samples = new float[64];
    public bool boolSoundVal;
    public float averageVal;

	
	void Awake () 
	{
		this.aSource = GetComponent<AudioSource>();
        this.aSource.clip = Microphone.Start("Built-in Microphone", true, 64, 44100);
        this.aSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        Debug.Log("start playing... position is" + Microphone.GetPosition(null));
    }
	
	
	void Update () 
	{
        averageVal = 0;
		//Obtain the samples from the frequency bands of the attached AudioSource
		aSource.GetSpectrumData(samples,0,FFTWindow.BlackmanHarris);
		
		//For each sample
		for(int i=0; i<samples.Length;i++)
		{
            float test = Mathf.Clamp(samples[i] * (50 + i * i), 0, 50);
            averageVal = averageVal + test;
		}
        averageVal = averageVal / 16;
        if (averageVal > .8)
        {
            boolSoundVal = true;
            //do echo stuff
        }
        else { boolSoundVal = false; }
    }
}
