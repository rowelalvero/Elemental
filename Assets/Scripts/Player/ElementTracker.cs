using UnityEngine;
using UnityEngine.UI;

public class ElementSeekerTracker : Singleton<ElementSeekerTracker>
{
    [Header("Seeker UI Sliders")]
    public Slider fireSeeker;
    public Slider waterSeeker;
    public Slider earthSeeker;
    public Slider windSeeker;

    private void Start()
    {
        ResetAllSliders();
    }
    public void OnPlayerAttack()
    {
        MonoBehaviour currentWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
        if (currentWeapon == null) return;

        WeaponInfo weaponInfo = (currentWeapon as IWeapon).GetWeaponInfo();
        if (weaponInfo == null) return;

        // Apply seeker logic
        if (weaponInfo.isFire)
            AdjustSliders(fireSeeker, earthSeeker, waterSeeker, windSeeker);
        else if (weaponInfo.isWater)
            AdjustSliders(waterSeeker, fireSeeker, earthSeeker, windSeeker);
        else if (weaponInfo.isEarth)
            AdjustSliders(earthSeeker, windSeeker, fireSeeker, waterSeeker);
        else if (weaponInfo.isWind)
            AdjustSliders(windSeeker, waterSeeker, fireSeeker, earthSeeker);
    }

    private void AdjustSliders(Slider main, Slider opposite, Slider fade1, Slider fade2)
    {
        main.value += 1f;
        opposite.value -= 2f;
        fade1.value -= 1f;
        fade2.value -= 0.5f;

        ClampSlider(main);
        ClampSlider(opposite);
        ClampSlider(fade1);
        ClampSlider(fade2);
    }

    private void ClampSlider(Slider slider)
    {
        slider.value = Mathf.Clamp(slider.value, 0f, slider.maxValue);
    }

    private void ResetAllSliders()
    {
        fireSeeker.value = 0;
        waterSeeker.value = 0;
        earthSeeker.value = 0;
        windSeeker.value = 0;
    }
}
