using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour{

    private Camera mainCamera;
    private RectTransform m_icon;
    private Image m_iconImage;
    private Canvas mainCanvas;
    private Vector3 m_cameraOffsetUp;
    private Vector3 m_cameraOffsetRight;
    private Vector3 m_cameraOffsetForward;
    public Sprite m_targetIconOnScreen;
    
    [Space]
    [Range(0, 100)]
    public float m_edgeBuffer;
    public Vector3 m_targetIconScale;
    [Space]
    public bool PointTarget = true;
    //Indicates if the object is out of the screen
    private bool m_outOfScreen;

    void Start() {
        mainCamera = Camera.main;
        mainCanvas = FindObjectOfType<Canvas>();
        Debug.Assert((mainCanvas != null), "There needs to be a Canvas object in the scene for the OTI to display");
        InstantiateTargetIcon();
    }

    void Update() {
        if(m_icon != null){
            UpdateTargetIconPosition();
        }
    }

    private void InstantiateTargetIcon() {
        m_icon = new GameObject().AddComponent<RectTransform>();
        m_icon.transform.SetParent(mainCanvas.transform);
        m_icon.localScale = m_targetIconScale;
        m_icon.name = name + ": Indicator icon";
        m_iconImage = m_icon.gameObject.AddComponent<Image>();
        m_iconImage.sprite = m_targetIconOnScreen;
    }

    private void UpdateTargetIconPosition() {

        Vector3 newPos = transform.position;
        newPos = mainCamera.WorldToViewportPoint(newPos);

        //Simple check if the target object is out of the screen or inside
        if (newPos.x > 1 || newPos.y > 1 || newPos.x < 0 || newPos.y < 0) {
            m_outOfScreen = true;
        } else {
            m_outOfScreen = false;
        }

        if (newPos.z < 0) {
            newPos.x = 1f - newPos.x;
            newPos.y = 1f - newPos.y;
            newPos.z = 0;
            newPos = Vector3Maximize(newPos);
        }

        newPos = mainCamera.ViewportToScreenPoint(newPos);
        newPos.x = Mathf.Clamp(newPos.x, m_edgeBuffer, Screen.width - m_edgeBuffer);
        newPos.y = Mathf.Clamp(newPos.y, m_edgeBuffer, Screen.height - m_edgeBuffer);
        newPos.z = 0;
        m_icon.transform.position = newPos;
        //Operations if the object is out of the screen
        if (m_outOfScreen) {
            m_icon.gameObject.SetActive(true);
            if (PointTarget) {
                //Rotate the sprite towards the target object
                var targetPosLocal = mainCamera.transform.InverseTransformPoint(transform.position);
                var targetAngle = -Mathf.Atan2(targetPosLocal.x, targetPosLocal.y) * Mathf.Rad2Deg - 90;
                //Apply rotation
                m_icon.transform.eulerAngles = new Vector3(0, 0, targetAngle);
            }
             
        } else {
            // Turn off sprite
            m_icon.gameObject.SetActive(false);
        }
         
    }

    public Vector3 Vector3Maximize(Vector3 vector) {
        Vector3 returnVector = vector;
        float max = 0;
        max = vector.x > max ? vector.x : max;
        max = vector.y > max ? vector.y : max;
        max = vector.z > max ? vector.z : max;
        returnVector /= max;
        return returnVector;
    }

    void OnDisable(){
        if(m_icon != null)
            Destroy(m_icon.gameObject);
    }

    void OnDestroy() {
        if(m_icon != null)
            Destroy(m_icon.gameObject);
    }

}