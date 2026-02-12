using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleTuningUI : MonoBehaviour
{
    public VehicleRigidBody vehicle;
    public VehicleController vehicleController;

    public Slider gripSlider;
    public Slider cgSlider;
    public Slider stabilitySlider;

    public TextMeshProUGUI yawText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI steerInputText;
    public TextMeshProUGUI frontSlipText;
    public TextMeshProUGUI rearSlipText;
    public TextMeshProUGUI understeerText;

    void Update()
    {
        if (vehicle == null) return;

        if (gripSlider != null)
            vehicle.TireGrip = gripSlider.value;

        if (cgSlider != null)
            vehicle.CgHeight = cgSlider.value;

        if (stabilitySlider != null)
            vehicle.StabilityGain = stabilitySlider.value;

        if (yawText != null)
            yawText.text = vehicle.YawRate.ToString("F2");

        if (velocityText != null)
        {
            velocityText.text = vehicle.Speed.ToString("F0");
        }
        if(steerInputText != null)
        {
            steerInputText.text = vehicleController.SteerInput.ToString();
        }
        if (frontSlipText != null)
        {
            frontSlipText.text = vehicle.FrontSlip.ToString();
        }
        if (rearSlipText != null)
        {
            rearSlipText.text = vehicle.RearSlip.ToString();
        }
        if(understeerText != null)
        {
            float understeerIndex = vehicle.RearSlip - vehicle.FrontSlip;
            understeerText.text= understeerIndex.ToString("F3");
        }
    }
}
