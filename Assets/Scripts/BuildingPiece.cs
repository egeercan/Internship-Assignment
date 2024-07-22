using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPiece : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _pickUpClip, _dropClip;

    private bool _dragging, _placed;
    private Vector2 _offset, _originalPosition;

    // Reference group
    [SerializeField] private Transform gridSlotGroup;
    [SerializeField] private float _snapDistance = 2.0f; // Adjust this value as needed

    private List<Transform> _gridSlots;

    // Timer and resource generation
    private float _timer;
    private float _generationInterval = 1.0f; // 1 second for per second generation
    public int goldPerSecond = 1; // Adjusted value for per second generation
    public int gemsPerSecond = 1; // Adjusted value for per second generation

    // Reference to the resource counter UI
    public Text resourceCounterText;

    // References to player resources
    public int playerGold = 0;
    public int playerGems = 0;

    // Building price
    public int buildingPriceGold = 10; // Example price for gold
    public int buildingPriceGems = 5;  // Example price for gems

    private Transform _currentSlot = null;

    void Awake()
{
    _originalPosition = transform.position;

    // Get grid slots
    _gridSlots = new List<Transform>();
    foreach (Transform slot in gridSlotGroup)
    {
        _gridSlots.Add(slot);
    }

    // Initialize the timer
    _timer = 0;

    // Initialize player resources
    playerGold = 100; // Oyuncunun başlangıçta 100 altını olacak
    playerGems = 0; // İsteğe bağlı olarak başlangıçta 0 mücevher

    // Initial UI update
    UpdateResourceCounter();
}


    void Update()
{
    if (!_dragging && _placed)
    {
        _timer += Time.deltaTime;

        if (_timer >= _generationInterval)
        {
            playerGold += goldPerSecond;
            playerGems += gemsPerSecond;

            _timer = 0;
            UpdateResourceCounter(); // UI'yi güncelle
        }
    }

    if (_dragging)
    {
        var mousePosition = GetMousePos();
        transform.position = mousePosition - _offset;
    }
}

    void OnMouseDown()
    {
        if (!_placed)
        {
            _dragging = true;
            _source.PlayOneShot(_pickUpClip);
            _offset = GetMousePos() - (Vector2)transform.position;
        }
        else
        {
            // Allow re-dragging of placed building
            _dragging = true;
            _placed = false; // Set placed to false to allow movement
            _source.PlayOneShot(_pickUpClip);
            _offset = GetMousePos() - (Vector2)transform.position;
            
            // Mark the current slot as unoccupied
            if (_currentSlot != null)
            {
                _currentSlot.GetComponent<GridSlot>().isOccupied = false;
                _currentSlot = null;
            }
        }
    }

    void OnMouseUp()
{
    _dragging = false;
    _source.PlayOneShot(_dropClip);

    Transform closestSlot = null;
    float closestDistance = float.MaxValue;

    foreach (var slot in _gridSlots)
    {
        float distance = Vector2.Distance(transform.position, slot.position);
        if (distance < closestDistance && distance < _snapDistance && !slot.GetComponent<GridSlot>().isOccupied)
        {
            closestDistance = distance;
            closestSlot = slot;
        }
    }

    if (closestSlot != null)
    {
        if (playerGold >= buildingPriceGold && playerGems >= buildingPriceGems)
        {
            playerGold -= buildingPriceGold;
            playerGems -= buildingPriceGems;

            transform.position = closestSlot.position;
            _placed = true;
            closestSlot.GetComponent<GridSlot>().isOccupied = true;
            _currentSlot = closestSlot;

            UpdateResourceCounter(); // UI'yi güncelle
        }
        else
        {
            transform.position = _originalPosition;
        }
    }
    else
    {
        transform.position = _originalPosition;
    }
}

    Vector2 GetMousePos()
    {
        return (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void UpdateResourceCounter()
{
    if (resourceCounterText != null)
    {
        resourceCounterText.text = "Gold: " + playerGold + "\nGems: " + playerGems;
    }
}

}
