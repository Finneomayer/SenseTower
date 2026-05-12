using System;
using UnityEngine;


namespace SenseAI.Robot
{

    [System.Serializable]
    class NavigationPoint
    { 
        public string _pointName;
        public Transform _pointTransform; 

        public NavigationPoint( Transform pointTransform)
        { 
            _pointName = pointTransform.gameObject.name;
            _pointTransform = pointTransform;
        }

        internal void InnitAndChek()
        {
            if (_pointTransform !=null)
            {
                if (_pointName.Length < 1)
                { 
                    _pointName = _pointTransform.gameObject.name;
                } 
            } 
        }
    }
}