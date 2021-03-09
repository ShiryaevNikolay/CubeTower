using UnityEngine;

public class ExplodeCube : MonoBehaviour
{

    // explosion - эффект "взрыва при проигрыше"
    public GameObject restartButton, explosion;

    // Нужно, чтобы условие в OnCollisionEnter сработало ОДИН раз
    private bool _collisionSet;
    
    /*
     * Эта функция срабатывает в тот момент, когда произойдет
     * соприкосновение с текущим объектом (к которому прикреплен скрипт) (Ground)
     *
     * collision - тот объект, с которым мы соприкоснулись (кубик)
     */
    private void OnCollisionEnter(Collision collision)
    {
        // Проверим с каким объектом соприкоснулись
        if (collision.gameObject.CompareTag("Cube") && !_collisionSet)
        {
            // Пробегаем по каждому дочернему объекту (кубику), начиная с конца
            for (int i = collision.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                
                /*
                 * Добавляем каждому дочернему объекту компонент Rigidbody,
                 * чтобы у них появилась физика при столкновении с текущим объектом (Ground)
                 */ 
                child.gameObject.AddComponent<Rigidbody>();
                
                /*
                 * AddExplosionForce - эта функция позволяет нам "взрывную силу"
                 *
                 * 70f - сила взрыва
                 * Vector3.up - вектор, по которому будет добавляться сила (добавление силы по координате Y)
                 * 5f - радиус действия силы
                 */
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(70f, Vector3.up, 5f);
                
                /*
                 * Чтобы объекты не зависили друг от друга, "открепляем" объекты (кубы) от родителя (All Cubes)
                 *
                 * (All Cubes) до этого момента становится пустым
                 */
                child.SetParent(null);
            }
            
            // При проигрыше показывает кнопку перезагрузки
            restartButton.SetActive(true);
            
            // Добавляем тряску камеры при проигрыше
            Camera.main.gameObject.AddComponent<CameraShake>();

            // Создаем объект "взрыв"
            GameObject newVfx = Instantiate(explosion, new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.contacts[0].point.z), Quaternion.identity) as GameObject;
            Destroy(newVfx, 2.5f);
            
            if (PlayerPrefs.GetString("music") != "No")
            {
                GetComponent<AudioSource>().Play();
            }
            
            /*
             * Уничтожаем collision (All Cubes)
             *
             * т.е. после соприкосновения (после добавления всем кубам самостоятельность и добавили Rigidbody)
             * удаляем All Cubes
             */
            Destroy(collision.gameObject);
            
            _collisionSet = true;
        }
    }
}
