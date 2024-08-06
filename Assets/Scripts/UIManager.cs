using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        //Screen object variables
        public GameObject loginUI;
        public GameObject registerUI;
        public GameObject SelectionPanel;

        private void Awake()
        {
        loginUI.SetActive(true);
        if (instance == null)
            {
                instance = this;
            }
            else if (instance != null)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        //Functions to change the login screen UI

        public void ClearScreen() //Turn off all screens
        {
            loginUI.SetActive(false);
            registerUI.SetActive(false);
            SelectionPanel.SetActive(false);
    }

        public void LoginScreen() //Back button
        {
            ClearScreen();
            loginUI.SetActive(true);
        }
        public void RegisterScreen() // Regester button
        {
            ClearScreen();
            registerUI.SetActive(true);
        }

        public void SelectionScreen() //Scoreboard button
        {
            ClearScreen();
            SelectionPanel.SetActive(true);
        }

 



    }