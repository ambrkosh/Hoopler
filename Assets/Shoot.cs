using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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

public static class ScrollbarExtension {
    public static float calculatePowerSetting(this Scrollbar sb) {
        return sb.value * 400;
    }
}

public static class CameraExtension {
    public static Vector3 getClickedWorldPosition(this Camera cam, float posX, float posY) {
        return cam.ScreenToWorldPoint(new Vector3(posX, posY, 10));
    }
}

public static class ArrayExtension {
    public static T[] map<T>(this T[] from, Func<T, T> action) {
        T[] result = from.Clone() as T[];
        for (var i = 0; i < from.Length -1; i++) {
            result[i] = action(from[i]);
        }
        return result;
    }

    public static List<T> map<T>(this List<T> from, Func<T, T> action) {
        T[] result = from.ToArray().map(action);
        return new List<T>(result);
    }
}

public class Shoot : MonoBehaviour {
    public List<GameObject> ballCloneList;
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
        powerCount.text = powerSetting.calculatePowerSetting().ToString();
        prompt.text = "Select Target";
        shoot.gameObject.SetActive(false);
        shoot.onClick.AddListener(delegate { shootClicked(); });
        replay.onClick.AddListener(delegate { replayClicked(); });
        ballCloneList = new List<GameObject>();
        updateShotCount();
    }

    // Invoked when the value of the slider changes.
    public void powerValueChangeCheck() {
        if (canShoot()) {
            roundStatus = setTargetData(roundStatus, () => { doShoot(); });
            shoot.gameObject.SetActive(true);
            prompt.text = "";
            EventSystem.current.SetSelectedGameObject(null);
        }
        powerCount.text = powerSetting.calculatePowerSetting().ToString();
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
        ballCloneList.ForEach(
            (ball) => {
                Destroy(ball);
            });
        ballCloneList.Clear();
    }

    // Update is called once per frame
    void Update() {
        if (!EventSystem.current.IsPointerOverGameObject() &&
            (Input.GetMouseButtonDown(0) || Input.touchCount == 1) && canShoot()) {

            if (!isRoundStatusComplete(roundStatus)) {
                if (roundStatus.isTargetSet) {
                    roundStatus.isPowerSet = true;
                    roundStatus.powerValue = powerSetting.calculatePowerSetting();
                    prompt.text = "";
                    shoot.gameObject.SetActive(true);
                } else {
                    roundStatus.isTargetSet = true;
                    Vector3 position = getClickedWorldPosition();
                    position.z = 10;
                    roundStatus.targetCoordinates = position;
                    prompt.text = "Set Power";
                    target = setTargetPosition(getCurrentMousePosition(Input.mousePosition.x, Input.mousePosition.y), target);
                }
            } else {
                prompt.text = "";
                shoot.gameObject.SetActive(true);
            }
        }

        Action completeAction = checkRoundComplete(canShoot());
        completeAction();
    }

    Vector3 getCurrentMousePosition(float positionX, float positionY) {
        return new Vector3(positionX, positionY, 10);
    }

    RawImage setTargetPosition(Vector3 pos, RawImage t) {
        t.transform.position = pos;
        t.gameObject.SetActive(true);
        return t;
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

    /*float calculatePowerSetting() {
        return powerSetting.value * 400;
    }*/

    bool canShoot() {
        return noShot < maxShoot;
    }

    void updateShotCount() {
        shotCount.text = "Shots Left: " + (maxShoot - noShot).ToString();
    }

    Action checkRoundComplete(bool canContinue) {
        return () => {
            replay.gameObject.SetActive(!canContinue);
            if (!canContinue) {
                prompt.text = "Game Over";
            }
            updateShotCount();
        };
    }

    RoundStatus setTargetData(RoundStatus status, Action onShoot, bool isShoot = false) {
        RoundStatus s = new RoundStatus(status.isTargetSet, status.isPowerSet, status.targetCoordinates, status.powerValue);
        if (status.isTargetSet) {
            s.isPowerSet = true;
            s.powerValue = powerSetting.calculatePowerSetting();
            if (isShoot) {
                onShoot();
            }
        }
        return s;
    }
}
