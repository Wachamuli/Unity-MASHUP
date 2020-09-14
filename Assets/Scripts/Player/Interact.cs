﻿using UnityEngine;

public class Interact : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager input;
    [SerializeField] private GameObject gunZone = null;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform objectZone = null;
    [SerializeField] private Inventory inventory;
    [SerializeField] private float throwForce = 4f;
    [SerializeField] private GameObject interactCrosshair = null;
    [SerializeField] private  GameObject weaponCrosshair = null;

    private bool DropOrThrow => (input.interact || input.throwItem) && Item.inUse;
    private bool Grab => input.interact && Item.inUse == false; 

    void Start()
    {
        input = GetComponent<InputManager>();
        inventory = GetComponent<Inventory>();
        cam = GetComponent<Transform>().GetChild(0).GetComponent<Camera>();
        interactCrosshair.SetActive(true);
    }

    void Update()
    {
        InteractWithObject();
    }

    private void InteractWithObject()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hit;
        
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Item.efectiveRadius))
        {
            Item getItem = hit.collider.GetComponent<Item>();
            if (getItem != null)
            {
                if (Grab)
                {
                    GrabItem(getItem);
                    gunZone.SetActive(false);

                    weaponCrosshair.SetActive(false);
                    interactCrosshair.SetActive(true);
                }
                else if (DropOrThrow)
                {
                    DropItem(getItem);
                    gunZone.SetActive(true);

                    weaponCrosshair.SetActive(true);
                    interactCrosshair.SetActive(false);
                }
                
                if (input.saveInInventoy)
                {
                    inventory.AddToInventory(getItem);
                    gunZone.SetActive(true);

                    weaponCrosshair.SetActive(true);
                    interactCrosshair.SetActive(false);
                }
            }
        }
    }

    private void GrabItem(Item item)
    {
        Item.inUse = true;

        item.rb.useGravity = false;
        item.rb.isKinematic = true;
        item.rb.velocity = Vector3.zero;
        item.rb.angularVelocity = Vector3.zero;

        item.transform.position = objectZone.transform.position;
        item.transform.SetParent(objectZone);
    }

    private void DropItem(Item item)
    {
        Item.inUse = false;

        item.rb.useGravity = true;
        item.rb.isKinematic = false;

        if (input.throwItem)
            item.rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);

        item.transform.SetParent(null);
        item = null;
    }
}