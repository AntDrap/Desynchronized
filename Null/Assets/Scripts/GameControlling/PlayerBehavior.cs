using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBehavior : MonoBehaviour
{
    public Camera playerCamera;
    public float lookSpeed, lookXLimit, speed, health, maxHealth, stamina, maxStamina, bobMod, shaking, shakeIntensity;
    public Transform playerHead;
    public GameObject highlight, ammoTextHolder, currentEquip, reticle, playerHead2, pda;
    public int accessLevel, moneyCount, sconeCount;
    public TextMeshProUGUI ammoText, totalAmmoText, moneyText, sconeText, accessLevelText, fps;
    public Slider healthSlider, staminaSlider;
    public List<GameObject> equipment;
    public bool[] equipmentUnlocked;
    public bool reloading;

    List<Vector3> equipmentStartPositions;
    Rigidbody rb;
    float rotationX, staminaCooldown, moneyLerp;
    int layerMask;
    bool moving, sprinting, gravity;

    void Awake()
    {
        equipmentStartPositions = new List<Vector3>();
        
        for (int i = 0; i < equipment.Count; i++)
        {
            equipmentStartPositions.Add(equipment[i].transform.localPosition);
            equipment[i].SetActive(false);
        }

        layerMask = LayerMask.GetMask(new string[] { "Default", "Target", "Interact", "Floor", "Enemy" });
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
    }

    void Update()
    {
        if(PlayerPrefs.HasKey("showFPS") && bool.Parse(PlayerPrefs.GetString("showFPS")))
        {
            fps.text = Mathf.Round((1.0f / Time.deltaTime)).ToString();
        }
        else
        {
            fps.text = "";
        }

        moneyLerp = Mathf.Lerp(moneyLerp, moneyCount, Time.deltaTime * 8);
        moneyText.text = "Ξ " + Mathf.RoundToInt(moneyLerp).ToString();

        playerHead.transform.localPosition = new Vector3(0, 1f + (moving ? ((Mathf.PingPong(Time.time * 0.75f, 0.2f) - 0.1f) * bobMod) : (Mathf.PingPong(Time.time * 0.08f, 0.08f) - 0.04f)), 0);

        if (pda.GetComponent<PDABehavior>().open)
        {
            reticle.SetActive(false);
            highlight.SetActive(false);
            return;
        }
        else
        {
            reticle.SetActive(true);
        }

        if(shaking > 0)
        {
            shaking -= Time.deltaTime;
            shakeIntensity = Mathf.Clamp(Mathf.Lerp(shakeIntensity, 0, Time.deltaTime), 0, 2);
            float tempShakeParam = shakeIntensity * 0.4f;
            playerHead2.transform.localPosition = playerHead2.transform.localPosition + (playerHead2.transform.right * Random.Range(-tempShakeParam, tempShakeParam)) + (playerHead2.transform.up * Random.Range(-tempShakeParam, tempShakeParam));
            playerHead2.transform.localPosition = new Vector3(Mathf.Clamp(playerHead2.transform.localPosition.x, -tempShakeParam, tempShakeParam), Mathf.Clamp(playerHead2.transform.localPosition.y,  -tempShakeParam, tempShakeParam), 0);
        }
        else
        {
            shakeIntensity = 0;
        }

        playerHead2.transform.localPosition = Vector3.Lerp(playerHead2.transform.localPosition, Vector3.zero, 4 * Time.deltaTime);

        if (currentEquip)
        {
            WeaponBehavior wb = currentEquip.GetComponent<WeaponBehavior>();

            if(wb.ammo >= 0 && !reloading)
            {
                ammoText.fontSize = Mathf.Lerp(ammoText.fontSize, 24, Time.deltaTime * 4);
                totalAmmoText.fontSize = Mathf.Lerp(totalAmmoText.fontSize, 24, Time.deltaTime * 4);
            }
 
            currentEquip.transform.localRotation = Quaternion.Lerp(currentEquip.transform.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), Time.deltaTime * 6);

            if(equipment.IndexOf(currentEquip) == 3 || equipment.IndexOf(currentEquip) == 4)
            {
                if (Input.GetMouseButton(0) && !reloading)
                {
                    wb.Attack();
                    wb.attacking = true;
                }
                else
                {
                    wb.attacking = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !reloading && !wb.cooldown)
                {
                    wb.Attack();
                }
            }

            if(wb.toggleAlt)
            {
                ResetView();
                if (Input.GetMouseButtonDown(1) && !reloading && !wb.cooldown)
                {
                    wb.AltAttack();
                }
            }
            else
            {
                if (Input.GetMouseButton(1) && !reloading && !wb.cooldown) 
                { 
                    wb.AltAttack(); 
                }
                else
                {
                    ResetView();
                }
                if (Input.GetMouseButtonUp(1) && !reloading && !wb.cooldown) 
                { 
                    wb.EndAltAttack(); 
                }

            }

            if (Input.GetKeyDown(KeyCode.R) && wb.ammo < wb.maxAmmo && wb.totalAmmo > 0 && !reloading && !wb.cooldown) { wb.Reload(); }
        }
        else
        {
            ammoTextHolder.SetActive(false);
        }

        if(!reloading)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { ChangeEquipment(0); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { ChangeEquipment(1); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { ChangeEquipment(2); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { ChangeEquipment(3); }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { ChangeEquipment(4); }
        }

        staminaSlider.value = Mathf.Lerp(staminaSlider.value, stamina, Time.deltaTime * 5);
        healthSlider.value = Mathf.Lerp(healthSlider.value, health, Time.deltaTime * 5);

        sprinting = Input.GetKey(KeyCode.LeftShift);
        staminaCooldown = Mathf.Clamp(staminaCooldown - Time.deltaTime, 0, 0.5f);

        if(sprinting && moving)
        {
            ChangeStamina(-Time.deltaTime);
        }
        else if(!Input.GetKey(KeyCode.LeftShift) && staminaCooldown <= 0)
        {
            ChangeStamina(Time.deltaTime);
        }

        MouseLook();

        RaycastHit hit;

        bool objHit = Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit, 3f, layerMask) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Interact");

        highlight.SetActive(objHit);

        if (objHit && Input.GetKeyDown(KeyCode.E))
        {
            hit.collider.GetComponent<TriggerBehavior>().Trigger();
        }

        gravity = Physics.Raycast(new Ray(transform.position, -Vector3.up), out hit, 1.25f, LayerMask.GetMask("Floor"));
    }

    public void ResetView()
    {
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 80, Time.deltaTime * 6);
        currentEquip.transform.localPosition = Vector3.Lerp(currentEquip.transform.localPosition, reloading ? equipmentStartPositions[equipment.IndexOf(currentEquip)] - Vector3.up * 0.25f : equipmentStartPositions[equipment.IndexOf(currentEquip)], 8 * Time.deltaTime);
    }

    public void ChangeStamina(float amount)
    {
        stamina = Mathf.Clamp(stamina + amount, 0, maxStamina);

        if(amount < 0)
        {
            staminaCooldown = 0.5f;
        }
    }

    public void ChangeHealth(float amount)
    {
        if(amount < 0)
        {
            shakeIntensity += 1;
            shaking += 0.375f;
        }

        health = Mathf.Clamp(health + amount, 0, maxHealth);

        if(health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void ChangeMoney(int amount)
    {
        moneyCount += amount;
    }

    public void ChangeScone()
    {
        sconeCount++;
        updateUI();
    }

    public void changeAccessLevel(int level)
    {
        accessLevel = Mathf.Max(accessLevel, level);
        updateUI();
    }

    public void updateUI()
    {
        accessLevelText.text = "A" + accessLevel;
        sconeText.text = sconeCount.ToString();
    }

    #region movement

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, playerHead.transform.position, 15 * Time.deltaTime);
        playerCamera.transform.rotation = playerHead.transform.rotation;
    }

    void MouseLook()
    {
        rotationX = Mathf.Clamp(rotationX + (-Input.GetAxis("Mouse Y") * lookSpeed), -lookXLimit, lookXLimit);
        playerHead.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    void MovePlayer()
    {
        Vector3 movement = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

        movement = movement.magnitude > 1 ? movement.normalized : movement;

        if(pda.GetComponent<PDABehavior>().open)
        {
            movement = Vector3.zero;
        }

        if (sprinting && stamina > 0)
        {
            movement *= 1.5f;
        }

        bobMod = movement.magnitude;

        if (rb.velocity.y > 0)
        {
            movement *= 1.25f;
        }

        moving = movement.magnitude > 0;

        movement = movement * speed;
        movement.y = rb.velocity.y;
        rb.velocity = movement;

        if (!gravity)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(rb.velocity.x, -8, rb.velocity.z), Time.deltaTime * 10);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }


    }

    #endregion
    #region combat

    public void ChangeEquipment(int index, bool ignore = false)
    {
        if (index < 0 || equipment.IndexOf(currentEquip) == index)
        {
            for (int i = 0; i < equipment.Count; i++)
            {
                equipment[i].transform.localPosition = equipmentStartPositions[i] - Vector3.up * 0.375f;
                equipment[i].SetActive(false);
            }
            currentEquip = null;
            return;
        }

        if (!equipmentUnlocked[index] || (currentEquip && !ignore && (!equipmentUnlocked[index])))
        { 
            return; 
        };

        for (int i = 0; i < equipment.Count; i++)
        {
            equipment[i].transform.localPosition = equipmentStartPositions[i] - Vector3.up * 0.375f;
            equipment[i].SetActive(false);
        }

        FindObjectOfType<PDABehavior>().TogglePDA(false, true);
        currentEquip = equipment[index];
        currentEquip.SetActive(true);
        currentEquip.GetComponent<WeaponBehavior>().Equip();
        WeaponBehavior wb = currentEquip.GetComponent<WeaponBehavior>();
        if (wb.ammo >= 0)
        {
            ammoTextHolder.SetActive(true);
            ammoText.text = wb.ammo.ToString();
            totalAmmoText.text = wb.totalAmmo.ToString();
        }
        else
        {
            ammoTextHolder.SetActive(false);
        }
    }

    public void UnlockEquipment(int index)
    {
        equipmentUnlocked[index] = true;
        ChangeEquipment(index, true);
    }

    #endregion

}