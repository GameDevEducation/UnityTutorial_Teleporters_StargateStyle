using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RingTeleporter : MonoBehaviour
{
    [SerializeField] string _UniqueID;
    [SerializeField] string _DisplayName;
    [SerializeField] Animator LinkedAnimController;
    [SerializeField] TextMeshProUGUI NameDisplay;
    [SerializeField] Transform DestinationButtonRoot;
    [SerializeField] GameObject DestinationButtonPrefab;
    [SerializeField] List<string> SupportedTags;

    public string UniqueID => _UniqueID;
    public string DisplayName => _DisplayName;
    public RingTeleporter CurrentDestination { get; private set; }
    public bool IsLocked { get; private set; } = false;

    List<UI_DestinationButton> DestinationButtons = new List<UI_DestinationButton>();
    List<GameObject> TrackedObjects = new List<GameObject>();

    System.Action RingsRaisedAction;
    System.Action RingsLoweredAction;

    // Start is called before the first frame update
    void Start()
    {
        TeleporterController.Instance.RegisterTeleporter(this);
        NameDisplay.text = DisplayName;
    }

    private void OnDestroy()
    {
        if (TeleporterController.Instance != null)
            TeleporterController.Instance.DeregisterTeleporter(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRaiseRings(System.Action onComplete = null)
    {
        LinkedAnimController.ResetTrigger("Lower");
        LinkedAnimController.SetTrigger("Raise");

        RingsRaisedAction = onComplete;
    }

    public void OnLowerRings(System.Action onComplete = null)
    {
        LinkedAnimController.ResetTrigger("Raise");
        LinkedAnimController.SetTrigger("Lower");

        RingsLoweredAction = onComplete;
    }

    public void OnRingsRaised()
    {
        if (RingsRaisedAction != null)
            RingsRaisedAction();
    }

    public void OnRingsLowered()
    {
        if (RingsLoweredAction != null)
            RingsLoweredAction();
    }

    public void SetDestination(RingTeleporter destination)
    {
        CurrentDestination = destination;
        foreach(var button in DestinationButtons)
        {
            button.SetSelectedDestination(destination);
        }
    }

    public void RefreshDestinationList()
    {
        // remove existing destinations
        for (int index = DestinationButtonRoot.childCount - 1; index >= 0; index--)
        {
            var childGO = DestinationButtonRoot.GetChild(index).gameObject;
            Destroy(childGO);
        }

        // add in the destination buttons
        DestinationButtons.Clear();
        foreach (var teleporter in TeleporterController.Instance.Teleporters)
        {
            if (teleporter == this)
                continue;

            // add in the destination button
            var destinationButtonGO = Instantiate(DestinationButtonPrefab, DestinationButtonRoot);
            var destinationLogic = destinationButtonGO.GetComponent<UI_DestinationButton>();
            destinationLogic.Bind(this, teleporter);

            DestinationButtons.Add(destinationLogic);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (SupportedTags.Contains(other.tag))
        {
            TrackedObjects.Add(other.gameObject);

            if (!IsLocked && CurrentDestination != null)
                TeleporterController.Instance.PerformTeleport(this, CurrentDestination);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (SupportedTags.Contains(other.tag))
        {
            TrackedObjects.Remove(other.gameObject);
        }
    }

    public void Lock()
    {
        IsLocked = true;
    }

    public void Unlock()
    {
        IsLocked = false;
    }

    public List<GameObject> GatherObjects()
    {
        return TrackedObjects;
    }
}
