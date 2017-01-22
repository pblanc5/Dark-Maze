using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScannerEffectDemo : MonoBehaviour
{
	public Transform ScannerOrigin;
	public Material EffectMaterial;
	public float ScanDistance;
    public float ScanSpeed;

    private Transform GoalOrigin;
    public float GoalScanDistance;
    public float GoalScanSpeed;
	private Camera _camera;
    private float MaxScan;
    private AudioSource pingsound;
    private AudioSource goalpingsound;

    private Vector3 ScannerOriginPosition;
    private float goaldistance;
    private bool goalpinged;
    
	bool _scanning;
    bool scandelay;
    float scandelaytime;

	void Start()
	{
        GameObject goal  = GameObject.FindGameObjectWithTag("Finish");
        GoalOrigin       = goal.transform;
        ScanDistance     = 0;
        ScanSpeed        = 4;
        GoalScanDistance = 0;
        GoalScanSpeed    = 6;
        goaldistance     = 0;
        MaxScan          = -1;
        pingsound        = gameObject.GetComponent<AudioSource>();
        goalpingsound    = goal.GetComponent<AudioSource>();
        scandelay        = false;
        scandelaytime    = 1f;
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

        if (OVRInput.GetDown(OVRInput.Button.One) && !scandelay)
		{
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
        else if (jammer.gameObject.name.Equals("Goal"))
        {
            SceneManager.LoadScene(1);
        }
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
