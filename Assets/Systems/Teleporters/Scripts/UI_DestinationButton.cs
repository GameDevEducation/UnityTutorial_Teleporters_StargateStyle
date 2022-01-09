using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DestinationButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DestinationLabel;
    [SerializeField] Image DestinationButtonImage;
    [SerializeField] Color SelectedColour = Color.green;
    [SerializeField] Color DefaultColour = Color.white;

    RingTeleporter OwningTeleporter;
    RingTeleporter DestinationTeleporter;

    public void Bind(RingTeleporter owner, RingTeleporter destination)
    {
        OwningTeleporter = owner;
        DestinationTeleporter = destination;
        DestinationLabel.text = DestinationTeleporter.DisplayName;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressed()
    {
        OwningTeleporter.SetDestination(DestinationTeleporter);
    }

    public void SetSelectedDestination(RingTeleporter teleporter)
    {
        DestinationButtonImage.color = teleporter == DestinationTeleporter ? SelectedColour : DefaultColour;
    }
}
