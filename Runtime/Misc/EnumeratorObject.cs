using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LordSheo
{
    public class EnumeratorObject : MonoBehaviour
    {
        public static EnumeratorObject DDOLInstance
        {
            get
            {
                if (_ddolInstance == null)
                {
                    _ddolInstance = new GameObject(nameof(EnumeratorObject))
                        .AddComponent<EnumeratorObject>();
                    
                    GameObject.DontDestroyOnLoad(_ddolInstance.gameObject);
                }

                return _ddolInstance;
            }
        }
        private static EnumeratorObject _ddolInstance;

        public static EnumeratorObject Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(nameof(EnumeratorObject))
                        .AddComponent<EnumeratorObject>();
                }

                return _instance;
            }
        }
        private static EnumeratorObject _instance;
    }
}
