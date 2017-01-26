using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScannerEffectDemo : MonoBehaviour
{
	public Transform ScannerOrigin;
	public Material EffectMaterial;
	public float ScanDistance;
    public float ScanSpeed;
    public float movespeed;
    public float rotspeed;

    private Transform GoalOrigin;
    public float GoalScanDistance;
    public float GoalScanSpeed;
	private Camera _camera;
    private float MaxScan;
    private AudioSource pingsound;
    private AudioSource goalpingsound;

    private Transform rig;
    private SteamVR_Controller.Device handL, handR;
    ulong trigger  = SteamVR_Controller.ButtonMask.Trigger;
    ulong touchpad = SteamVR_Controller.ButtonMask.Touchpad;

    private Vector3 ScannerOriginPosition;
    private float goaldistance;
    private bool goalpinged;
    
	bool _scanning;
    bool scandelay;
    float scandelaytime;

    private void Awake()
    {
        int index = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        handL = SteamVR_Controller.Input(index);

        index = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        handR = SteamVR_Controller.Input(index);

        rig = transform.parent;
    }

	void Start()
	{
        GameObject goal  = GameObject.FindGameObjectWithTag("Finish");
        GoalOrigin       = goal.transform;
        ScanDistance     = 0;
        ScanSpeed        = 3;
        GoalScanDistance = 0;
        GoalScanSpeed    = 7;
        goaldistance     = 0;
        MaxScan          = -1;
        pingsound        = gameObject.GetComponent<AudioSource>();
        goalpingsound    = goal.GetComponent<AudioSource>();
        scandelay        = false;
        scandelaytime    = 1f;
        movespeed        = 1f;
        rotspeed         = 1f;
    }

	void Update()
	{
        GoalScanDistance += Time.deltaTime * GoalScanSpeed;

        if (_scanning && (MaxScan == -1 || ScanDistance < MaxScan))
        {
            ScanDistance += Time.deltaTime * ScanSpeed / (MaxScan == -1 ? 1 : 3);
            if (ScanDistance > goaldistance - 2.2 && ScanDistance < goaldistance + 2.2)
            {
                goalpingsound.pitch = Random.Range(0.8f, 1f);
                goalpingsound.Play();
                GoalScanDistance = 0;
            }
        }
        
        if ((handL.GetPressDown(trigger) || handR.GetPressDown(trigger)) && !scandelay)
		{
            handL.TriggerHapticPulse(1000);
            handR.TriggerHapticPulse(1000);
            scandelay = true;
			_scanning = true;
			ScanDistance = 0;
            ScannerOriginPosition = ScannerOrigin.position;
            pingsound.pitch = Random.Range(0.8f, 1.1f);
            pingsound.Play();

            goaldistance = Vector3.Distance(transform.position, GoalOrigin.position);
            scandelaytime = MaxScan == -1 ? (goaldistance / ScanSpeed + goaldistance / GoalScanSpeed) : 1f;
            StartCoroutine(WaitForGoalPing());
		}

        if (handL.GetPress(touchpad))
        {
            Transform corrected = transform;
            corrected.Rotate(-corrected.rotation.eulerAngles.x, 0f, -corrected.rotation.eulerAngles.z);
            float deltaX = movespeed * Time.deltaTime * handL.GetAxis().x;
            float deltaY = movespeed * Time.deltaTime * handL.GetAxis().y;
            rig.Translate(deltaX, 0f, 0f, corrected);
            rig.Translate(0f, 0f, deltaY, corrected);
        }

        if (handR.GetPress(touchpad))
        {
            if      (handR.GetAxis().x > 0.2f)  transform.Rotate(0f, rotspeed * Time.deltaTime, 0f);
            else if (handR.GetAxis().x < -0.2f) transform.Rotate(0f, -rotspeed * Time.deltaTime, 0f);
        }
    }

    private IEnumerator WaitForGoalPing()
    {
        yield return new WaitForSeconds(scandelaytime);
        scandelay = false;
    }

    public void OnTriggerEnter(Collider jammer)
    {
        if (jammer.gameObject.name.Equals("Jammer(Clone)"))
            MaxScan = 3;
        else if (jammer.gameObject.name.Equals("JammerTrig"))
        {
            Destroy(jammer.transform.parent.gameObject);
            MaxScan = -1;
        }
        //else if (jammer.gameObject.name.Equals("Goal"))
        //{
        //    SceneManager.LoadScene(1);
        //}
    }

    public void OnTriggerExit(Collider jammer)
    {
        if (jammer.gameObject.tag.Equals("Jammer"))
            MaxScan = -1;
    }

    void OnEnable()
	{
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
	}

	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		EffectMaterial.SetVector("_WorldSpaceScannerPos", ScannerOriginPosition); //////////////////////////////////////
		EffectMaterial.SetFloat("_ScanDistance", ScanDistance);
        EffectMaterial.SetVector("_GoalWorldSpaceScannerPos", GoalOrigin.position);
        EffectMaterial.SetFloat("_GoalScanDistance", GoalScanDistance);
        RaycastCornerBlit(src, dst, EffectMaterial);
	}

	void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
	{
		// Compute Frustum Corners
		float camFar = _camera.farClipPlane;
		float camFov = _camera.fieldOfView;
		float camAspect = _camera.aspect;

		float fovWHalf = camFov * 0.5f;

		Vector3 toRight = _camera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
		Vector3 toTop = _camera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

		Vector3 topLeft = (_camera.transform.forward - toRight + toTop);
		float camScale = topLeft.magnitude * camFar;

		topLeft.Normalize();
		topLeft *= camScale;

		Vector3 topRight = (_camera.transform.forward + toRight + toTop);
		topRight.Normalize();
		topRight *= camScale;

		Vector3 bottomRight = (_camera.transform.forward + toRight - toTop);
		bottomRight.Normalize();
		bottomRight *= camScale;

		Vector3 bottomLeft = (_camera.transform.forward - toRight - toTop);
		bottomLeft.Normalize();
		bottomLeft *= camScale;

		// Custom Blit, encoding Frustum Corners as additional Texture Coordinates
		RenderTexture.active = dest;

		mat.SetTexture("_MainTex", source);

		GL.PushMatrix();
		GL.LoadOrtho();

		mat.SetPass(0);

		GL.Begin(GL.QUADS);

		GL.MultiTexCoord2(0, 0.0f, 0.0f);
		GL.MultiTexCoord(1, bottomLeft);
		GL.Vertex3(0.0f, 0.0f, 0.0f);

		GL.MultiTexCoord2(0, 1.0f, 0.0f);
		GL.MultiTexCoord(1, bottomRight);
		GL.Vertex3(1.0f, 0.0f, 0.0f);

		GL.MultiTexCoord2(0, 1.0f, 1.0f);
		GL.MultiTexCoord(1, topRight);
		GL.Vertex3(1.0f, 1.0f, 0.0f);

		GL.MultiTexCoord2(0, 0.0f, 1.0f);
		GL.MultiTexCoord(1, topLeft);
		GL.Vertex3(0.0f, 1.0f, 0.0f);

		GL.End();
		GL.PopMatrix();
	}

    public void OnDestroy()
    {
        GoalOrigin = null;
        ScanDistance = 0;
        ScanSpeed = 4;
        GoalScanDistance = 0;
        GoalScanSpeed = 10;
        goaldistance = 0;
        MaxScan = -1;
        scandelay = false;
        scandelaytime = 1f;
    }
}
