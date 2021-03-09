using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    /*
     * nowCube - тот кубик, который был расположен в игре самым последним
     *
     * 0, 1, 0 - координаты Main Cube при старте игры
     */
    private CubePosition nowCube = new CubePosition(0, 1, 0);
    
    
    /*
     * cubeChangePlaceSpeed отвечает за то, как быстро будет меняться позиция куба,
     * который показывет место следующего нового куба
     */
    public float cubeChangePlaceSpeed = 0.5f;

    public Transform cubeToPlace;

    public GameObject[] cubesToCreate;
    
    // vfx - эффект установки кубика
    public GameObject allCubes, vfx;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRb;

    private float cameraMoveToYPosition, cameraMoveSpeed = 2f;

    public Text scoreTxt;

    public Color[] bgColors;
    private Color toCametaColor;
    
    // Чтобы условие проигрыша выполнилось один раз
    private bool IsLose, firstCube;
    
    // Координаты кубов, куда нельзя ставить новые кубы
    private List<Vector3> allCubesPosition = new List<Vector3>
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(1, 0, -1),
        new Vector3(-0, 0, 1),
    };

    private int prevCountMaxHorizontal;
    private Transform mainCamera;
    
    // Храним ссылку на корутину, чтобы ее же потом остановить 
    private Coroutine showCubePlace;

    private List<GameObject> possibleCubesToCreate = new List<GameObject>();
    
    private void Start()
    {
        if (PlayerPrefs.GetInt("score") < 5)
            possibleCubesToCreate.Add(cubesToCreate[0]);
        else if (PlayerPrefs.GetInt("score") < 10)
            AddPossibleCubes(2);
        else if (PlayerPrefs.GetInt("score") < 15)
            AddPossibleCubes(3);
        else if (PlayerPrefs.GetInt("score") < 25)
            AddPossibleCubes(4);
        else if (PlayerPrefs.GetInt("score") < 35)
            AddPossibleCubes(5);
        else if (PlayerPrefs.GetInt("score") < 50)
            AddPossibleCubes(6);
        else if (PlayerPrefs.GetInt("score") < 70)
            AddPossibleCubes(7);
        else if (PlayerPrefs.GetInt("score") < 90)
            AddPossibleCubes(8);
        else if (PlayerPrefs.GetInt("score") < 110)
            AddPossibleCubes(9);
        else
            AddPossibleCubes(10);

        scoreTxt.text = "<size=40><color=#FA423C>Best:</color></size> " + PlayerPrefs.GetInt("score") + "\n<size=25>Now:</size> 0";
        
        toCametaColor = Camera.main.backgroundColor;
        mainCamera = Camera.main.transform;
        cameraMoveToYPosition = 5.9f + nowCube.y - 1;
        
        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        // Проверяем нажал ли пользователь на экран, НО не на элементы интерфейса
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
            // Если игра запущена на телефоне, пк и т.д, но НЕ в Unity editor (НЕ в Unity3D)
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif

            /*
             * Если все таки нажмем не на кнопку (т.е. на экран), то удаляем элементы интерфейса
             * (после перезапуска всё вернется)
             */
            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject canvas in canvasStartPage)
                    Destroy(canvas);
            }
            
            /*
             * Создаем объект newCube
             *
             * Instantiate(
                cubeToCreate,   - сам объект
                cubeToPlace.position,   - позиция, в которой будет создан
                Quaternion.identity);   - поворот (выбрано стандартное вращение, т.е. не меняем вращение объекта)
             */

            GameObject createCube = null;
            if (possibleCubesToCreate.Count == 1)
            {
                createCube = possibleCubesToCreate[0];
            }
            else
            {
                createCube = possibleCubesToCreate[Random.Range(0, possibleCubesToCreate.Count)];
            }
            GameObject newCube = Instantiate(
                createCube,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;
            
            if (PlayerPrefs.GetString("music") != "No")
            {
                GetComponent<AudioSource>().Play();
            }
            
            // Создаем эффект установки кубика
            GameObject newVfx = Instantiate(vfx, newCube.transform.position, Quaternion.identity) as GameObject;
            Destroy(newVfx, 1.5f);
            
            // Устанавливаем родителя для созданного куба
            newCube.transform.SetParent(allCubes.transform);
            
            // Теперь нужно поменять коордитаны "проекционного" куба
            nowCube.setVector(cubeToPlace.position);
            
            // Добавляем координаты нового соданного куба в список недоступных координат
            allCubesPosition.Add(nowCube.getVector());
            
            // Чтобы башня падала обновляем значение в Rigidbody (костыль)
            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;
            
            // На всякий случай вызовем функцию SpawnPositions()
            SpawnPositions();
            
            MoveCameraChangeBg();
        }
        
        /*
         * Проверяем, является ли башня стабильной или падает
         *
         * magnitude - "скорость движения Rigidbody"
         */
        if (!IsLose && allCubesRb.velocity.magnitude > 0.1f)
        {
            // Уничтожаем проектирующий куб
            Destroy(cubeToPlace.gameObject);

            // Устанавливаем проигрыш = true
            IsLose = true;
            
            StopCoroutine(showCubePlace);
        }
        
        /*
         * Vector3.MoveTowards() - с его помощью можно плавно двигать оъекты по координатам
         */
        mainCamera.localPosition = Vector3.MoveTowards(
            mainCamera.localPosition, 
            new Vector3(mainCamera.localPosition.x, cameraMoveToYPosition, mainCamera.localPosition.z),
            cameraMoveSpeed * Time.deltaTime
            );

        
        // Lerp плавно меняет цвет
        if (Camera.main.backgroundColor != toCametaColor)
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCametaColor, Time.deltaTime / 1.5f);
        }
    }

    IEnumerator ShowCubePlace()
    { 
        while (true)
        {
            SpawnPositions();
            
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }

    private void SpawnPositions()
    {
        // Список доступных мест, куда можно поставить новый куб
        List<Vector3> positions = new List<Vector3>();

        // Проверяем каждую позицию около последнего куба, можно ли туда добавить новый куб
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z))
            && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z))
            && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z))
            && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z))
            && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1))
            && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1))
            && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 1)
            // Выбираем случайным образом позицию от 0 (включительно) до positions.Count (количество достапных мест) (НЕвключительно)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];
    }

    private bool IsPositionEmpty(Vector3 targetPosition)
    {
        if (targetPosition.y == 0)
            return false;

        foreach (Vector3 position in allCubesPosition)
        {
            if (position.x == targetPosition.x && position.y == targetPosition.y && position.z == targetPosition.z)
                return false;
        }

        return true;
    }

    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHorizontal;

        foreach (Vector3 position in allCubesPosition)
        {
            if (Mathf.Abs(Convert.ToInt32(position.x)) > maxX)
                maxX = Mathf.Abs(Convert.ToInt32(position.x));
            
            if (Mathf.Abs(Convert.ToInt32(position.y)) > maxY)
                maxY = Mathf.Abs(Convert.ToInt32(position.y));
            
            if (Mathf.Abs(Convert.ToInt32(position.z)) > maxZ)
                maxZ = Mathf.Abs(Convert.ToInt32(position.z));
        }
        
        // Устанавливаем лучший результат
        if (PlayerPrefs.GetInt("score") < maxY - 1)
        {
            PlayerPrefs.SetInt("score", maxY - 1);
        }

        scoreTxt.text = "<size=40><color=#FA423C>Best:</color></size> " + PlayerPrefs.GetInt("score") + "\n<size=25>Now:</size> " +
                        (maxY - 1);
        
        // Поднимаем или опускаем камеру
        cameraMoveToYPosition = 5.9f + nowCube.y - 1;

        // Отдоляем или приближаем камеру, максимально 3 кубика может быть
        maxHorizontal = maxX > maxZ ? maxX : maxZ;
        if (maxHorizontal % 3 == 0 && prevCountMaxHorizontal != maxHorizontal)
        {
            mainCamera.localPosition += new Vector3(0, 0, -2.5f);
            prevCountMaxHorizontal = maxHorizontal;
        }

        if (maxY >= 7)
            toCametaColor = bgColors[2];
        else if (maxY >= 5)
            toCametaColor = bgColors[1];
        else if (maxY >= 2)
            toCametaColor = bgColors[0];
    }
    
    private void AddPossibleCubes(int till) {
        for (int i = 0; i < till + 1; i++)
            possibleCubesToCreate.Add(cubesToCreate[i]);
    }
}

/*
 *  Создаем структуру, которая отвечает за хранение координат какого-либо объекта
 */
struct CubePosition
{
    public int x, y, z;

    public CubePosition(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }
    
    public void setVector(Vector3 position)
    {
        x = Convert.ToInt32(position.x);
        y = Convert.ToInt32(position.y);
        z = Convert.ToInt32(position.z);
    }
}
