using UnityEngine;

namespace RPG.Control
{
    public enum CursorType
    {
        None,
        Movement,
        Combat,
        UI,
        Pickup
    }

    [System.Serializable]
    struct CursorMapping
    {
        public CursorType type;
        public Texture2D texture;
        public Vector2 hotspot;
    }
}