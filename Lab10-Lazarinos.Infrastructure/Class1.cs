namespace Lab10_Lazarinos.Infrastructure;

// Hacemos la clase 'static' para que los métodos de utilidad
// puedan llamarse sin crear una instancia de esta clase.
public static class Class1 
{
    /// <summary>
    /// Suma dos números.
    /// </summary>
    public static double Sumar(double a, double b)
    {
        return a + b;
    }

    /// <summary>
    /// Resta el segundo número al primero.
    /// </summary>
    public static double Restar(double a, double b)
    {
        return a - b;
    }

    /// <summary>
    /// Multiplica dos números.
    /// </summary>
    public static double Multiplicar(double a, double b)
    {
        return a * b;
    }

    /// <summary>
    /// Divide el primer número por el segundo. Incluye manejo de error.
    /// </summary>
    public static double Dividir(double a, double b)
    {
        // 🔴 Manejo CRÍTICO de la división por cero
        if (b == 0)
        {
            // Siempre debes lanzar una excepción clara. Nunca ignores este error.
            throw new DivideByZeroException("ERROR: La división por cero es una estupidez matemática que crashea el programa si no la manejas.");
        }
        return a / b;
    }
}