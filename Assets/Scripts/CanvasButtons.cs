using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.GetString("music") == "No" && gameObject.name == "Music")
        {
            // GetComponent<Image>().sprite = musicOff;
        }
    }

    public void RestartGame()
    {

        if (PlayerPrefs.GetString("music") != "No")
        {
            GetComponent<AudioSource>().Play();
        }
        
        /*
         * SceneManager.LoadScene(id): id - идентификатор сцены, которую ходим загрузить
         * его можно найти в File->Build Settings... и нажимаем на Add Open Scene, и справа от добавленной сцены будет id
         *
         * SceneManager.GetActiveScene().buildIndex запрашиваем текущую сцену и берем у нее id
         */
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadShop()
    {

        if (PlayerPrefs.GetString("music") != "No")
        {
            GetComponent<AudioSource>().Play();
        }
        
        /*
         * Находим и загружаем сцену магазина
         */
        SceneManager.LoadScene("Shop");
    }
    
    public void CloseShop()
    {

        if (PlayerPrefs.GetString("music") != "No")
        {
            GetComponent<AudioSource>().Play();
        }
        
        /*
         * Находим и загружаем сцену магазина
         */
        SceneManager.LoadScene("Main");
    }

    public void MusicWork()
    {
        // Сейчас музыка выключена и её нужно включить
        if (PlayerPrefs.GetString("music") == "No")
        {
            GetComponent<AudioSource>().Play();
            PlayerPrefs.SetString("music", "Yes");
        }
        else
        {
            PlayerPrefs.SetString("music", "No");
        }
    }
}
