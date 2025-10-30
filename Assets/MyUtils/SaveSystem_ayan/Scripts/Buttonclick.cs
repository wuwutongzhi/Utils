using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SaveSystem_ayan
{
    public class Buttonclick : MonoBehaviour
    {
        [SerializeField] Property property;

        [SerializeField] Button buttonSave;
        [SerializeField] Button buttonLoad;

        private void Start()
        {
            buttonSave.onClick.AddListener(() => property.Save());
            buttonLoad.onClick.AddListener(() => property.Load());
        }
    }

}