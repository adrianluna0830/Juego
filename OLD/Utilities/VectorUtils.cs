using UnityEngine;

public static class VectorUtils
{
    /// <summary>
    /// Rota un vector alrededor del eje Y con un ángulo aleatorio dentro del rango de dispersión especificado.
    /// </summary>
    /// <param name="originalVector">Vector original a rotar</param>
    /// <param name="spreadAngle">Ángulo máximo de dispersión (en grados)</param>
    /// <returns>Vector rotado aleatoriamente en el eje Y</returns>
    public static Vector3 GetVectorWithYSpread(Vector3 originalVector, float spreadAngle)
    {
        // Obtener un ángulo aleatorio entre -spreadAngle/2 y +spreadAngle/2
        float randomAngle = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
        
        // Crear una rotación en el eje Y con el ángulo aleatorio
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        
        // Aplicar la rotación al vector original
        return rotation * originalVector;
    }
    
    public static Vector3 GetVectorWithAsymmetricSpread(Vector3 originalVector, float leftSpreadAngle, float rightSpreadAngle)
    {
        // Asegurarse de que los ángulos sean positivos
        leftSpreadAngle = Mathf.Abs(leftSpreadAngle);
        rightSpreadAngle = Mathf.Abs(rightSpreadAngle);

        // Decidir aleatoriamente si rotará a la izquierda o derecha
        bool rotateLeft = Random.value > 0.5f;
        
        float randomAngle;
        if (rotateLeft)
        {
            // Rotar a la izquierda (ángulos positivos)
            randomAngle = Random.Range(0f, leftSpreadAngle);
        }
        else
        {
            // Rotar a la derecha (ángulos negativos)
            randomAngle = -Random.Range(0f, rightSpreadAngle);
        }

        // Crear y aplicar la rotación
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        return rotation * originalVector;
    }

    /// <summary>
    /// Sobrecarga que permite especificar el ángulo exacto de rotación en lugar de uno aleatorio
    /// </summary>
    /// <param name="originalVector">Vector original a rotar</param>
    /// <param name="angle">Ángulo específico de rotación (en grados)</param>
    /// <returns>Vector rotado en el eje Y con el ángulo especificado</returns>
    public static Vector3 GetVectorWithYRotation(Vector3 originalVector, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        return rotation * originalVector;
    }
}