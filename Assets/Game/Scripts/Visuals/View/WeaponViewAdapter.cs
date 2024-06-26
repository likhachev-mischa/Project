﻿using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Visuals
{
    public sealed class WeaponViewAdapter : MonoBehaviour, IViewAdapter
    {
        public Entity Entity { get; set; }

        public event Action VisualsDrawn;

        public void DrawVisuals(ComponentToDraw component, string data)
        {
            VisualsDrawn?.Invoke();
        }

        public void DisableVisuals(ComponentToDraw component)
        {
            
        }

        private void OnEnable()
        {
            VisualsDrawn += OnVisualsDrawn;
        }

        private void Update()
        {
        }

        private void OnVisualsDrawn()
        {
        }

        private void OnDestroy()
        {
            VisualsDrawn -= OnVisualsDrawn;
        }

        private void OnDrawGizmosSelected()
        {
        }
    }
}