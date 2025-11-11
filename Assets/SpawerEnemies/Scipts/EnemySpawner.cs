using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    // El prefab del enemigo que se va a generar
    public GameObject enemyPrefab;

    // Área rectangular donde se pueden generar enemigos (visible en el Inspector)
    public Vector2 spawnAreaSize = new Vector2(20f, 5f);

    // Número máximo de enemigos que puede haber en el nivel a la vez
    public int maxEnemies = 5;

    // Intervalo de tiempo para intentar generar un nuevo enemigo
    public float spawnInterval = 3f;

    // Contador del tiempo
    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        // Solo intentamos generar si el tiempo ha transcurrido
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Resetear el temporizador
            timer = spawnInterval;

            // Intentar generar si no se ha alcanzado el límite
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // Contar cuántos objetos tienen la etiqueta "Enemy"
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemies < maxEnemies)
        {
            // 1. Calcular una posición aleatoria dentro del área
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

            // La posición final es la posición del spawner + la posición aleatoria
            Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, 0f);

            // 2. Instanciar el enemigo
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            Debug.Log("Enemigo generado en: " + spawnPosition);
        }
    }

    // Dibujar el área de generación en el Editor para facilitar la visualización
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.5f); // Naranja semitransparente
        Gizmos.DrawCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));
    }
}
