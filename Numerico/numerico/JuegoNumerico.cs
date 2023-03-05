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
    public static int NumJuegoMin = 1;
    public static int NumJuegoMax = 1;

    static JuegoNumerico()
    {
        NumJuegoMin = JuegosNumericos.Min();
        NumJuegoMax = JuegosNumericos.Max();
    }

    /// <summary>
    /// Los juegos numéricos ya definidos.
    /// </summary>
    public static List<int> JuegosNumericos = new List<int>() { 1, 2 };

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

        foreach (var c in original)
        {
            int i = ordenLetras.IndexOf(c, StringComparison.OrdinalIgnoreCase);
            if (i == -1)
            //if (i < 1)
            {
                sb.Append(' ');
                sb.Append(c);
            }
            else
            {
                sb.Append(i.ToString("00"));
            }
        }
        return sb.ToString();
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
                autor = s;
                continue;
            }
            if (string.IsNullOrEmpty(titulo))
            {
                titulo = s;
                continue;
            }
            if (string.IsNullOrEmpty(contenido))
            {
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
