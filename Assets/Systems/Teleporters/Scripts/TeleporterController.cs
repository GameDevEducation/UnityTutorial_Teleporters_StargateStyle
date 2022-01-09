using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleporterController : MonoBehaviour
{
    [SerializeField] UnityEvent OnTeleport;

    public static TeleporterController Instance { get; private set; }

    public List<RingTeleporter> Teleporters { get; private set; } = new List<RingTeleporter>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found a duplicate TeleporterController on {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterTeleporter(RingTeleporter teleporter)
    {
        Teleporters.Add(teleporter);

        foreach (var knownTeleporter in Teleporters)
            knownTeleporter.RefreshDestinationList();
    }

    public void DeregisterTeleporter(RingTeleporter teleporter)
    {
        Teleporters.Remove(teleporter);

        foreach (var knownTeleporter in Teleporters)
            knownTeleporter.RefreshDestinationList();
    }

    public void PerformTeleport(RingTeleporter sender, RingTeleporter receiver)
    {
        // lock sender and receiver
        sender.Lock();
        receiver.Lock();

        // raise the rings on the sender
        sender.OnRaiseRings(() =>
        {
            // raise rings on the receiver
            receiver.OnRaiseRings(() =>
            {
                OnTeleport.Invoke();

                var senderObjects = sender.GatherObjects();
                var receiverObjects = receiver.GatherObjects();

                // swap the objects
                MoveObjects(senderObjects, sender, receiver);
                MoveObjects(receiverObjects, receiver, sender);

                // lower the rings on the receiver
                receiver.OnLowerRings(() =>
                {
                    receiver.Unlock();
                });

                // lower the rings on the sender and complete the teleport
                sender.OnLowerRings(() =>
                {
                    sender.Unlock();
                });
            });
        });
    }

    void MoveObjects(List<GameObject> toMove, RingTeleporter sender, RingTeleporter receiver)
    {
        foreach(var mover in toMove)
        {
            Vector3 offset = mover.transform.position - sender.transform.position;

            mover.transform.position = receiver.transform.position + offset;
        }
    }
}
