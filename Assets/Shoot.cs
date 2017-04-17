using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public struct RoundStatus {
    public bool isTargetSet, isPowerSet;
    public Vector3 targetCoordinates;
    public float powerValue;

    public RoundStatus(bool isTarget, bool isPower, Vector3 targetCoord, float powerVal) {
        isTargetSet = isTarget;
        isPowerSet = isPower;
        targetCoordinates = targetCoord;
        powerValue = powerVal;
    }
}

public class Shoot : MonoBehaviour {
    public ArrayList ballCloneList;
    public static int maxShoot = 5;
    public int noShot = 0;
    public static float startValue = 1000;
    public Scrollbar powerSetting;
    public static float basePower = 20;
    public RoundStatus roundStatus;
    public Text powerCount;
    public Text prompt;
    public Button replay;
    public Button shoot;
    public RawImage target;
    public Text shotCount;

    // Use this for initialization
    void Start () {
        Physics.gravity = new Vector3(0, -20, 0);
        roundStatus = new RoundStatus(false, false, Vector3.zero, 0);
        powerSetting.onValueChanged.AddListener(delegate { powerValueChangeCheck(); });
        powerCount.text = calculatePowerSetting().ToString();
        prompt.text = "Select Target";
        shoot.gameObject.SetActive(false);
        shoot.onClick.AddListener(delegate { shootClicked(); });
        replay.onClick.AddListener(delegate { replayClicked(); });
        ballCloneList = new ArrayList();
        updateShotCount();
    }

    // Invoked when the value of the slider changes.
    public void powerValueChangeCheck() {
        if (canShoot()) {
            setTargetData();
            shoot.gameObject.SetActive(true);
            prompt.text = "";
            EventSystem.current.SetSelectedGameObject(null);
        }
        powerCount.text = calculatePowerSetting().ToString();
    }

    public void shootClicked() {
        doShoot();
    }

    public void replayClicked() {
        reset();
    }

    void reset() {
        roundStatus = new RoundStatus(false, false, Vector3.zero, 0);
        prompt.text = "Select Target";
        shoot.gameObject.SetActive(false);
        target.gameObject.SetActive(false);
        powerSetting.value = 0;
        noShot = 0;
        foreach (var ball in ballCloneList) {
            Destroy(ball as GameObject);
        }
        ballCloneList.Clear();
    }

    // Update is called once per frame
    void Update() {
        if (!EventSystem.current.IsPointerOverGameObject() &&
            (Input.GetMouseButtonDown(0) || Input.touchCount == 1) && canShoot()) {

            if (!isRoundStatusComplete(roundStatus)) {
                if (roundStatus.isTargetSet) {
                    roundStatus.isPowerSet = true;
                    roundStatus.powerValue = calculatePowerSetting();
                    prompt.text = "";
                    shoot.gameObject.SetActive(true);
                } else {
                    roundStatus.isTargetSet = true;
                    Vector3 position = getClickedWorldPosition();
                    position.z = 10;
                    roundStatus.targetCoordinates = position;
                    prompt.text = "Set Power";
                    setTargetPosition(getCurrentMousePosition());
                }
            } else {
                prompt.text = "";
                shoot.gameObject.SetActive(true);
            }
        }

        checkRoundComplete();
    }

    Vector3 getCurrentMousePosition() {
        return new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
    }

    void setTargetPosition(Vector3 pos) {
        target.transform.position = pos;
        target.gameObject.SetActive(true);
    }

    Vector3 getClickedWorldPosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }

    bool isRoundStatusComplete(RoundStatus status) {
        return status.isPowerSet && status.isTargetSet;
    } 

    void doShoot() {
        Vector3 camPos = Camera.main.transform.position;

        GameObject ballPrefab = Resources.Load("Ball") as GameObject;
        GameObject ball = Instantiate(ballPrefab, camPos, Quaternion.identity) as GameObject;

        ball.transform.LookAt(roundStatus.targetCoordinates);
        Vector3 force = ball.transform.forward;
        ball.GetComponent<Rigidbody>().AddForce(force * (roundStatus.powerValue + startValue)); //AddForce(new Vector3(clickedWorldPosition.x, power + startValue, (power + basePower) * -1), ForceMode.Impulse);
        noShot++;
        ballCloneList.Add(ball);

        resetStatus();
    }

    void resetStatus() {
        roundStatus = new RoundStatus(false, false, Vector3.zero, 0);
        prompt.text = "Select Target";
        shoot.gameObject.SetActive(false);
        target.gameObject.SetActive(false);
    }

    float calculatePowerSetting() {
        return powerSetting.value * 400;
    }

    bool canShoot() {
        return noShot < maxShoot;
    }

    void updateShotCount() {
        shotCount.text = "Shots Left: " + (maxShoot - noShot).ToString();
    }

    void checkRoundComplete() {
        if (canShoot()) {
            replay.gameObject.SetActive(false);
        }
        else {
            prompt.text = "Game Over";
            replay.gameObject.SetActive(true);
        }
        updateShotCount();
    }

    void setTargetData(bool isShoot = false) {
        if (roundStatus.isTargetSet) {
            roundStatus.isPowerSet = true;
            roundStatus.powerValue = calculatePowerSetting();
            if (isShoot) {
                doShoot();
            }
        }
    }
}
