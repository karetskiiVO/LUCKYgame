using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour {
    [System.Serializable]
    private class SlotCardsRaw {
        public string name; 
        public Material material;
    }

    [SerializeField]
    private GameObject slotInstance;

    [Header("Настройка слотов")]
    [SerializeField]
    private SlotCardsRaw[] slotCardsRaw;

    [SerializeField]
    private GravityField gravityField;

    private Dictionary<string, Material> slotCards = new();
    private List<string> slotNames = new();

    private class SlotLine {
        System.Random randomDevice = new();
        Action afterStop;

        private Vector3 begin, end;
        private float rotationDuration = 0f;

        private float currVelocity = 0f;
        private float currAcceleration = 0f;

        private readonly float maxdiff = 0.02f;
        
        private GameObject slotInstance;
        private SlotMachine machine;

        struct PullInstance {
            public GameObject gameObject;
            public string name;
        }

        private PullInstance[] currPool = new PullInstance[16];
        private int currPoolSize = 0;

        public SlotLine (SlotMachine machine, GameObject slotInstance, Vector3 begin, Vector3 end) {
            this.machine      = machine;
            this.slotInstance = slotInstance;
            this.begin        = begin;
            this.end          = end;
        }

        public void Update () {
            RotationStep();
            DisableFirst();
            SpawnNew();
        }

        private void RotationStep () {
            if (rotationDuration > 0) {
                rotationDuration -= Time.deltaTime;
            } else if (currVelocity > 0) {
                currVelocity = Mathf.Max(currVelocity - currAcceleration * Time.deltaTime, 0);

                if (currVelocity == 0) afterStop?.Invoke();
            }

            for (var i = 0; i < currPoolSize; i++) {
                currPool[i].gameObject.transform.localPosition += currVelocity * Time.deltaTime * (end - begin).normalized;
            }
        }

        private void DisableFirst () {
            var firstElem = currPool[0].gameObject;
            if (firstElem == null) return;
            if (firstElem.transform.localPosition.x > end.x) return;

            Destroy(firstElem);

            for (var i = 0; i < currPoolSize - 1; i++) {
                currPool[i] = currPool[i+1];
            }

            currPoolSize--;
            currPool[currPoolSize].gameObject = null;
        }

        private void SpawnNew () {
            if (currPoolSize != 0 && currPool[currPoolSize-1].gameObject != null) {
                if (Mathf.Max(begin.x - currPool[currPoolSize-1].gameObject.transform.localPosition.x) < maxdiff) return;
            }

            var newInstance = Instantiate(slotInstance, machine.transform);

            var name = machine.slotNames[randomDevice.Next(machine.slotNames.Count)];
            newInstance.transform.localPosition = begin;
            newInstance.SetActive(true);
            newInstance.GetComponent<Renderer>().material = machine.slotCards[name];

            currPool[currPoolSize] = new PullInstance
            {
                gameObject = newInstance,
                name = name
            };

            currPoolSize++;
        }

        public bool rotated { get => currVelocity > 0; }
        public void Rotate (float rotationDuration, float stopDuration, float velocity) {
            currVelocity = velocity;
            currAcceleration = velocity / stopDuration;

            this.rotationDuration = rotationDuration;
        }

        private int RolledIdx () {
            var i1 = (currPoolSize - 1)/2;
            var i2 = i1+1;

            var dist1 = Mathf.Abs(currPool[i1].gameObject.transform.localPosition.x - (-0.0388f));
            var dist2 = Mathf.Abs(currPool[i2].gameObject.transform.localPosition.x - (-0.0388f));

            if (dist1 < dist2) return i1;
            return i2;
        }

        public string value {get => currPool[RolledIdx()].name; }
        public Action AfterStop {set => afterStop = value;  }
    }

    private SlotLine[] slotLines;
    private System.Random randomDevice = new();
    void Start () {
        foreach (var card in slotCardsRaw) {
            slotCards.Add(card.name, card.material);
            slotNames.Add(card.name);
        }

        slotLines = new SlotLine[] {
            new(this, slotInstance, new Vector3(0.0f, 0.1585f, -0.0273f), new Vector3(-0.083f, -0.0589f, -0.0273f)),
            new(this, slotInstance, new Vector3(0.0f, 0.1585f, -0.0820f), new Vector3(-0.083f, -0.0589f, -0.0820f)),
            new(this, slotInstance, new Vector3(0.0f, 0.1585f, -0.1403f), new Vector3(-0.083f, -0.0589f, -0.1403f)),
        };

        foreach (var line in slotLines) line.Rotate(0.5f, 0.5f, ((float)randomDevice.NextDouble() + 1) * 4);
        
        slotLines[2].AfterStop = delegate {
            slotLines[2].AfterStop = ProcessCombination;
        };
    }

    void Update () {
        foreach (var line in slotLines) line.Update();
    }

    private string bet = "";
    public void Rotate () {
        if (slotLines[0].rotated || slotLines[1].rotated || slotLines[2].rotated) return;

        slotLines[0].Rotate(1.0f, 1.0f, ((float)randomDevice.NextDouble() + 1) * 2);
        slotLines[1].Rotate(1.5f, 2.5f, ((float)randomDevice.NextDouble() + 1) * 2);
        slotLines[2].Rotate(2.0f, 3.0f, ((float)randomDevice.NextDouble() + 1) * 2);
    }

    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.TryGetComponent<InteractiveTag>(out var idComponent)) {
            bet = idComponent.id;
            Destroy(idComponent.gameObject);
        }
    }

    private void ProcessCombination () {
        /** Плохо, но заморачиваться не хочется **/
        var res1 = slotLines[0].value;
        var res2 = slotLines[1].value;
        var res3 = slotLines[2].value;

        var res = "";

        switch (bet) {
        case "coin":
            if (gravityField.isActive) break;
            if (!(res1 == "10" && res2 == "10" && res3 == "10")) break;

            res = "cup";

            break;
        case "cup":
            res = "loose";
            GameObject.Find("door").GetComponent<Door>().Open(); // TODO: повесить какие-то реплики
            break;
        case "soda":
            if (!gravityField.isActive) break;
            if (!(res1 == res2 && res2 == res3)) break;

            res = "coin";
            
            break;
        case "":
            if (!gravityField.isActive) {
                if (res1 == res2 && res2 == res3) {
                    res = "fish";
                    break;
                }
                break;
            }


            if (res1 == res2 && res2 == res3) {
                res = "otank";
                break;
            }

            Debug.Log("boba");

            if (res1 != "10" && res2 != "10" && res3 != "10") {
                
                Debug.Log("aboba");
                res = "soda";
                break;
            }
            break;
        case "otank": 
            if (res1 != "10" && res2 != "10" && res3 != "10") {
                res = "soda";
                break;
            }
            break;
        default:
            break;
        }

        /*****************************************/

        if (ResourseDB.resources.ContainsKey(res)) {
            var obj = Instantiate(ResourseDB.resources[res]);
            obj.transform.localPosition = transform.position + UnityEngine.Random.onUnitSphere;
        }

        Debug.Log(String.Format(
            "Bet:{0} Combination:({1},{2},{3}) -> {4}", 
            bet,
            slotLines[0].value,
            slotLines[1].value,
            slotLines[2].value,
            res
        ));

        bet = "";
    }
}
