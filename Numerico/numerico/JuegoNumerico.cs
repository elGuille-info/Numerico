//--------------------------------------------------------------------------
// Pasatiempo numérico para .NET MAUI                           (02/mar/23)
//
// (c) Guillermo Som (Guille), 2023
//-------------------------------------------------------------------------- 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasatiempo.Numerico;

public class JuegoNumerico
{
    static JuegoNumerico()
    {
        NumJuegoMin = JuegosNumericos.Min();
        NumJuegoMax = JuegosNumericos.Max();
    }

    /// <summary>
    /// Los juegos numéricos ya definidos.
    /// </summary>
    public static List<int> JuegosNumericos { get; set; } = new List<int>() { 1, 2, 3, 4, 5 };

    public static int NumJuegoMin { get; set; } = 1;
    public static int NumJuegoMax { get; set; } = 1;
    

    /// <summary>
    /// Crear el pasatiempo numérico, sustituyendo las letras de contenido por números según el ordenLentras.
    /// </summary>
    /// <param name="autor">El autor</param>
    /// <param name="titulo">El título</param>
    /// <param name="contenido">El texto a codificar</param>
    /// <param name="ordenLetras">El orden de las letras</param>
    /// <param name="numJuego">El número de juego</param>
    /// <returns>Un objeto de tipo DatosNumerico con los datos originales y codificados (las letras sustituidas por números que ocupan 2 posiciones: 010203...2728)</returns>
    /// <remarks>Los caracteres no codificados empezarán con un espacio y el carácter a mostrar.</remarks>
    public static DatosNumerico CrearNumerico(string autor, string titulo, string contenido, string ordenLetras, int numJuego)
    {
        DatosNumerico elNumerico = new()
        {
            Autor = autor,
            Contenido = contenido,
            OrdenLetras = ordenLetras,
            Titulo = titulo,
            NumJuego = numJuego,
            // Codificar cada parte
            Autor_N = Codificar(autor, ordenLetras),
            Titulo_N = Codificar(titulo, ordenLetras),
            Contenido_N = Codificar(contenido, ordenLetras)
        };

        return elNumerico;
    }

    /// <summary>
    /// Codificar la cadena indicada según el orden de las letras.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="ordenLetras"></param>
    /// <returns>Una cadena con valores numéricos de dos cifras (o el símbolo sin codificar empezando con espacio).</returns>
    private static string Codificar(string original, string ordenLetras)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in original)
        {
            int i = ordenLetras.IndexOf(c, StringComparison.OrdinalIgnoreCase);
            // Tener en cuenta las vocales con tilde            (06/mar/23 16.42)
            if (i == -1)
            {
                if (EsVocalConTilde(c))
                {
                    i = ordenLetras.IndexOf(CambiarVocal(c), StringComparison.OrdinalIgnoreCase);
                    sb.Append(i.ToString("00"));
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(c);
                }
            }
            else
            {
                sb.Append(i.ToString("00"));
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Comprueba si el carácter indicado es una vocal con tilde o diéresis.
    /// </summary>
    /// <param name="vocal"></param>
    /// <returns></returns>
    public static bool EsVocalConTilde(char vocal)
    {
        return "ÁÉÍÓÚÀÈÌÒÙÄËÏÖÜ".IndexOf(vocal, StringComparison.OrdinalIgnoreCase) > -1;
    }

    /// <summary>
    /// Si la letra indicada es una vocal con tilde (o diéresis) cambiarla sin tilde.
    /// </summary>
    /// <param name="vocal"></param>
    /// <returns></returns>
    public static string CambiarVocal(string vocal)
    {
        return CambiarVocal(vocal[0]).ToString();
        //int n = "ÁÉÍÓÚÀÈÌÒÙÄËÏÖÜ".IndexOf(vocal, StringComparison.OrdinalIgnoreCase);
        //if (n > -1)
        //{
        //    return "AEIOUAEIOUAEIOU"[n].ToString();
        //}
        //return vocal;
    }

    /// <summary>
    /// Si la letra indicada es una vocal con tilde (o diéresis) cambiarla sin tilde.
    /// </summary>
    /// <param name="vocal"></param>
    /// <returns></returns>
    public static char CambiarVocal(char vocal)
    {
        int n = "ÁÉÍÓÚÀÈÌÒÙÄËÏÖÜ".IndexOf(vocal, StringComparison.OrdinalIgnoreCase);
        if (n > -1)
        {
            return "AEIOUAEIOUAEIOU"[n];
        }
        return vocal;
    }

    /// <summary>
    /// Leer el juego indicado, el fichero se llamará numerico_NUM.txt
    /// </summary>
    /// <param name="numJuego"></param>
    /// <returns>Un objeto de tipo DatosNumerico con los datos originales y codificados.</returns>
    public static async Task<DatosNumerico> LeerJuego(int numJuego)
    {
        // Leer el fichero indicado por NumeroJuego
        string fic = $"numerico_{numJuego}.txt";
        using var stream = await FileSystem.OpenAppPackageFileAsync(fic);
        using var reader = new StreamReader(stream);

        string autor = null;
        string titulo = null;
        string contenido = null;
        string orden_letras = null;
        while (!reader.EndOfStream)
        {
            var s = reader.ReadLine();
            if (string.IsNullOrEmpty(s)) continue;
            if (s.TrimStart().StartsWith('#')) continue;
            if (string.IsNullOrEmpty(autor))
            {
                //autor = QuitarTildes(s.ToUpper());
                autor = s;
                continue;
            }
            if (string.IsNullOrEmpty(titulo))
            {
                //titulo = QuitarTildes(s.ToUpper());
                titulo = s;
                continue;
            }
            if (string.IsNullOrEmpty(contenido))
            {
                //contenido = QuitarTildes(s.ToUpper());
                contenido = s;
                continue;
            }
            if (string.IsNullOrEmpty(orden_letras))
            {
                orden_letras = s;
                continue;
            }
        }
        return CrearNumerico(autor, titulo, contenido, orden_letras, numJuego);
    }

    ///// <summary>
    ///// Cambiar las vocales con tilde y la u con diéresis por vocales normales.
    ///// </summary>
    ///// <param name="texto">El texto a comprobar</param>
    ///// <returns>El nuevo texto con los cambios hechos</returns>
    //private static string QuitarTildes(string texto)
    //{
    //    return texto.Replace("Á", "A", StringComparison.OrdinalIgnoreCase).
    //                 Replace("É", "E", StringComparison.OrdinalIgnoreCase).
    //                 Replace("Í", "I", StringComparison.OrdinalIgnoreCase).
    //                 Replace("Ó", "O", StringComparison.OrdinalIgnoreCase).
    //                 Replace("Ú", "U", StringComparison.OrdinalIgnoreCase).
    //                 Replace("Ü", "U", StringComparison.OrdinalIgnoreCase);
    //}

    public class DatosNumerico
    {
        public int NumJuego;
        public string Autor;
        public string Titulo;
        public string Contenido;
        public string OrdenLetras;
        // Los valores codificados
        public string Autor_N;
        public string Titulo_N;
        public string Contenido_N;
    }
}
