using UnityEngine;
using UnityEngine.Rendering;

public class OptionManager : MonoBehaviour {
    [SerializeField] Volume _brightnessVolume;
    private static OptionManager _instance;
    public static OptionManager Instance { get { return _instance; } }

    private void Awake() {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        LoadOptionData();
    }

    public void SetBrightness(float intensity) {
        _brightnessVolume.weight = intensity;
        SaveOptionData();
    }

    public float GetBrightness() {
        return _brightnessVolume.weight;
    }

    public Volume GetBrightnessVolume() {
        return _brightnessVolume;
    }

    private void SaveOptionData() {
        ES3File es3File = new ES3File("OptionData.es3");
        es3File.Save("brightness", _brightnessVolume.weight);
        es3File.Sync();
    }

    public void LoadOptionData() {
        float brightness;
        ES3File es3File = new ES3File("OptionData.es3");
        if (es3File.KeyExists("brightness")) {
            brightness = es3File.Load<float>("brightness");
        }
        else {
            brightness = 0.5f;
        }
        _brightnessVolume.weight = brightness;
    }
}
