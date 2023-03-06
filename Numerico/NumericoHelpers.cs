//--------------------------------------------------------------------------
// Métodos compartidos para el pasatiempo numérico               (05/mar/23)
//
// (c) Guillermo Som (Guille), 2023
//-------------------------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;

namespace Pasatiempo.Numerico;

public static class NumericoHelpers
{
    public static JuegoNumerico.DatosNumerico ElJuego { get; set; } = null;
    public static int NumeroJuego { get; set; }
    public static bool SolucionMostrada { get; set; }  = false;

    /// <summary>
    /// Asignar la solución de la sección indicada.
    /// </summary>
    /// <param name="grb">La sección a comprobar</param>
    /// <param name="texto">El texto correcto de la sección</param>
    /// <param name="mostrar">Si se debe mostrar u ocultar</param>
    public static void AsignarSolucion(StackLayout grb, string texto, bool mostrar)
    {
        int i = 0;
        int n = 0;

        while (i < grb.Children.Count)
        {
            // La fila de letras contiene Entry
            StackLayout grbFilaLetra = (StackLayout)grb.Children[i + 1];
            //foreach (Entry vLetra in grbFilaLetra)
            foreach (Entry vLetra in grbFilaLetra.Cast<Entry>())
            {
                var c = texto[n];
                // Si es una de las letras a cambiar, y no es mostrar, asignar un espacio
                // Salvo que sea una letra escrita por el usuario
                if (mostrar == false && char.IsLetter(c))
                {
                    if (string.IsNullOrEmpty(vLetra.ClassId))
                    {
                        c = ' ';
                    }
                }
                n++;
                vLetra.Text = c.ToString();
            }
            i += 2;
        }
    }

    /// <summary>
    /// Asignar la letra indicada.
    /// </summary>
    /// <param name="grb">La sección a comprobar</param>
    /// <param name="texto">El texto correcto de la sección</param>
    /// <param name="letra">La letra a comprobar</param>
    /// <param name="todas">True si se asignan todas, false si se asigna la primera no asignada</param>
    public static void AsignarLetra(StackLayout grb, string texto, string letra, bool todas)
    {
        int i = 0;
        int n = 0;

        while (i < grb.Children.Count)
        {
            // La fila de letras contiene Entry
            StackLayout grbFilaLetra = (StackLayout)grb.Children[i + 1];
            //foreach (Entry vLetra in grbFilaLetra)
            foreach (Entry vLetra in grbFilaLetra.Cast<Entry>())
            {
                //var s = texto[n].ToString().ToUpper();
                string s = JuegoNumerico.CambiarVocal(texto[n].ToString().ToUpper());
                if (letra.ToUpper() == s) 
                {
                    // solo las no asignadas
                    if (string.IsNullOrEmpty(vLetra.ClassId))
                    {
                        vLetra.ClassId = letra;
                        vLetra.Text = letra;
                        if (todas == false)
                            return;
                    }
                }
                n++;
            }
            i += 2;
        }
    }

    /// <summary>
    /// Cambiar las letras con el número indicado por la letra indicada.
    /// </summary>
    /// <param name="grb">La sección a comprobar</param>
    /// <param name="texto">El texto correcto de la sección</param>
    /// <param name="letras">Las letras en el orden de los números mostrados</param>
    /// <param name="numero">El número a cambiar</param>
    /// <param name="letra">La letra a poner en las que estén en ese número</param>
    /// <returns></returns>
    public static bool CambiarNumeroLetra(StackLayout grb, string texto, string letras, int numero, string letra)
    {
        // Tomar de OrdenLetras la letra con el índice indicado en numero
        // y asignar todas las que tengan ese número con la letra indicada
        numero -= 1;
        // Si el número no está en rango, nada que hacer
        if (numero < 0 || numero >= letras.Length) return false;
        var cual = letras[numero].ToString();
        int i = 0;
        int n = 0;

        while (i < grb.Children.Count)
        {
            // La fila de letras contiene Entry
            StackLayout grbFilaLetra = (StackLayout)grb.Children[i + 1];
            //foreach (Entry vLetra in grbFilaLetra)
            foreach (Entry vLetra in grbFilaLetra.Cast<Entry>())
            {
                //var s = texto[n].ToString().ToUpper();
                string s = JuegoNumerico.CambiarVocal(texto[n].ToString().ToUpper());
                if (cual == s)
                {
                    vLetra.ClassId = letra;
                    vLetra.Text = letra;
                }
                n++;
            }
            i += 2;
        }
        return true;
    }

    /// <summary>
    /// Comprobar si el contenido de la sección indicada está resuelto
    /// </summary>
    /// <param name="grb">Sección a comprobar</param>
    /// <param name="texto">El texto correcto de esa sección</param>
    /// <returns>True si lo escrito coincide con el texto indicado</returns>
    public static bool ComprobarContenido(StackLayout grb, string texto)
    {
        StringBuilder sb = new StringBuilder();

        int i = 0;

        while (i < grb.Children.Count)
        {
            // La fila de letras contiene Entry
            StackLayout grbFilaLetra = (StackLayout)grb.Children[i + 1];
            //foreach (Entry vLetra in grbFilaLetra)
            foreach (Entry vLetra in grbFilaLetra.Cast<Entry>())
            {
                string s = vLetra.Text.Trim();
                if (string.IsNullOrEmpty(s))
                {
                    sb.Append(' ');
                    continue;
                }
                s = JuegoNumerico.CambiarVocal(s);
                // Cambiar las vocales con tilde y diéresis por vocales normales
                //if ("ÁÄÀ".IndexOf(s) > -1) s = "A";
                //if ("ÉËÈ".IndexOf(s) > -1) s = "E";
                //if ("ÍÏÌ".IndexOf(s) > -1) s = "I";
                //if ("ÓÖÒ".IndexOf(s) > -1) s = "O";
                //if ("ÚÜÙ".IndexOf(s) > -1) s = "U";
                sb.Append(s);
            }
            i += 2;
        }

        return sb.ToString() == texto.ToUpper();
    }

    /// <summary>
    /// Mostrar u ocultar la solución.
    /// </summary>
    /// <param name="grbAutor">La sección del autor</param>
    /// <param name="grbTitulo">La sección del título</param>
    /// <param name="grbContenido">La sección del contenido</param>
    /// <param name="mostrar">True si se debe mostrar la solución, false si solo se deja lo escrito por el usuario</param>
    public static void MostrarSolucion(StackLayout grbAutor, StackLayout grbTitulo, StackLayout grbContenido, bool mostrar)
    {
        AsignarSolucion(grbAutor, ElJuego.Autor, mostrar);
        AsignarSolucion(grbTitulo, ElJuego.Titulo, mostrar);
        AsignarSolucion(grbContenido, ElJuego.Contenido, mostrar);
    }

    /// <summary>
    /// Comprobar si se ha resuelto el juego
    /// </summary>
    /// <param name="grbAutor">La sección del autor</param>
    /// <param name="grbTitulo">La sección del título</param>
    /// <param name="grbContenido">La sección del contenido</param>
    /// <returns>El número de secciones resueltas</returns>
    public static async Task<int> ComprobarJuego(StackLayout grbAutor, StackLayout grbTitulo, StackLayout grbContenido)
    {
        // Comprobar si se ha resuelto el pasatiempo numérico
        // En grbContenido estará el contenido del texto
        // En grbAutor estará el nombre del autor
        // En grbTitulo estará el título
        // En cada grupo (StackLayout) habrá filas de dos StackLayout con los números y las letras
        // Si en la fila de números hay un signo, es que no se debe tener en cuenta
        // En la fila de las letras solo se debe comprobar si en la de números es un número
        // Cada número corresponde con el orden en ElJuego.OrdenLetras

        int resueltos = 0;

        await Task.Run(() =>
        {
            if (ComprobarContenido(grbAutor, ElJuego.Autor)) resueltos++;
            if (ComprobarContenido(grbTitulo, ElJuego.Titulo)) resueltos++;
            if (ComprobarContenido(grbContenido, ElJuego.Contenido)) resueltos++;
        });

        return resueltos;
    }

    /// <summary>
    /// Asignar el contenido del juego a los StackLayout de la página.
    /// </summary>
    /// <param name="grb">Grupo al que añadir los controles</param>
    /// <param name="texto">El texto a resolver</param>
    /// <param name="conSolucion">Si se muestra la solución</param>
    public static void AsignarContenido(StackLayout grb, string texto, bool conSolucion)
    {
        string ordenLetras = ElJuego.OrdenLetras;
        int altoNum = 25;
        int altoLetra = 40;
        int anchoEntry = 40;
        int anchoNum = 44;
        // Mostrar 20 columnas y las filas que sean necesarias
        int totalColumnas = 20;
        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            // En Phone Android (Pixel 4a) va bien
            // En iPhone 7 plus, se cierra al cargar el programa
            totalColumnas = 10; // 9;
            anchoNum = 36; // 40;
            anchoEntry = anchoNum;
        }
        else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            totalColumnas = 22; // 16;
            anchoNum = 35; // 40;
            //anchoEntry = 35;
            anchoEntry = anchoNum;
        }
        else if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            totalColumnas = 24;
            anchoNum = 44;
        }

        _ = grb.Dispatcher.Dispatch(() =>
        {
            grb.Children.Clear();
            int n = -2;
            int col = 0;
            StackLayout grbFilaNum = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            StackLayout grbFilaLetra = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            char elChar = ' ';
            // el formato es 'nn' o 'espacio char'
            while (true)
            {
                n += 2;
                if (n >= texto.Length)
                {
                    break;
                }
                var s = texto.Substring(n, 2);
                string elNum;
                string laLetra;
                if (s.StartsWith(" "))
                {
                    elNum = s.Substring(1, 1);
                    laLetra = elNum;
                    elChar = elNum[0];
                }
                else
                {
                    int i = Convert.ToInt32(s);
                    elChar = ordenLetras[i];
                    if (conSolucion)
                    {
                        laLetra = ordenLetras[i].ToString();
                    }
                    else
                    {
                        laLetra = "";
                    }
                    elNum = (i + 1).ToString();
                }
                // Para la fila de los números usar etiquetas ya que solo es para mostrar el número
                Label celdaNum = new Label
                //Entry celdaNum = new Entry
                {
                    FontFamily = "Consolas",
                    WidthRequest = anchoNum, // anchoEntry + 4,
                    HeightRequest = altoNum,
                    Text = elNum,
                    IsVisible = true,
                    BackgroundColor = Colors.WhiteSmoke
                };
                // Para la fila del texto, usar Entry para poder escribir
                Entry celdaLetra = new Entry
                {
                    FontFamily = "Consolas",
                    HorizontalTextAlignment = TextAlignment.Center,
                    WidthRequest = anchoEntry,
                    HeightRequest = altoLetra,
                    Text = laLetra,
                    IsVisible = true,
                    ClassId = ""
                };

                // Solo añadir los eventos en las letras
                // Los espacios mostrarlos oscuros y de solo lectura
                //if (laLetra == " ")
                if (char.IsLetter(elChar))
                {
                    celdaLetra.Completed += Entry_Completed;
                    celdaLetra.Unfocused += Entry_Unfocused;
                    celdaLetra.Focused += Entry_Focused;
                }
                else
                {
                    celdaLetra.IsReadOnly = true;
                    if (laLetra == " ")
                    {
                        celdaLetra.BackgroundColor = Colors.Gray;
                    }
                    else
                    {
                        celdaLetra.BackgroundColor = Colors.WhiteSmoke;
                    }
                }

                grbFilaNum.Children.Add(celdaNum);
                grbFilaLetra.Children.Add(celdaLetra);
                col++;
                if (col >= totalColumnas)
                {
                    // Nueva fila
                    grbFilaNum.IsVisible = true;
                    grbFilaLetra.IsVisible = true;
                    grb.Children.Add(grbFilaNum);
                    grb.Children.Add(grbFilaLetra);
                    grbFilaNum = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal
                    };
                    grbFilaLetra = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal
                    };
                    col = 0;
                }
            }
            if (col > 0)
            {
                grb.Children.Add(grbFilaNum);
                grb.Children.Add(grbFilaLetra);
                grbFilaNum.IsVisible = true;
                grbFilaLetra.IsVisible = true;
            }
        });
    }

    private static bool yaEstoy = false;
    private static void Entry_Completed(object sender, EventArgs e)
    {
        if (yaEstoy) return;

        Entry vLetra = (Entry)sender;
        string s = vLetra.Text.Trim();
        if (string.IsNullOrEmpty(s)) { return; }

        yaEstoy = true;

        // Cambiar las vocales con tilde y diéresis por vocales normales
        if ("ÁÄÀ".IndexOf(s) > -1) s = "A";
        if ("ÉËÈ".IndexOf(s) > -1) s = "E";
        if ("ÍÏÌ".IndexOf(s) > -1) s = "I";
        if ("ÓÖÒ".IndexOf(s) > -1) s = "O";
        if ("ÚÜÙ".IndexOf(s) > -1) s = "U";
        vLetra.Text = s;

        // Indicar que ya está asignada esta letra,
        // con idea de que al limpiar se limpien las que no tengan contenido escrito por el usuario
        vLetra.ClassId = s;

        yaEstoy = false;
    }
    private static void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        Entry_Completed(sender, e);
    }
    private static void Entry_Focused(object sender, FocusEventArgs e)
    {
        Entry vLetra = (Entry)sender;
        if (vLetra == null) return;
        if (string.IsNullOrEmpty(vLetra.Text)) return;
        vLetra.CursorPosition = 0;
        //vLetra.SelectionLength = 1;
        vLetra.SelectionLength = vLetra.Text.Length;
    }
}