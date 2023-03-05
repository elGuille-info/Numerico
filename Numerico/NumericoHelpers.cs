﻿//--------------------------------------------------------------------------
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
                // Cambiar las vocales con tilde y diéresis por vocales normales
                if ("ÁÄÀ".IndexOf(s) > -1) s = "A";
                if ("ÉËÈ".IndexOf(s) > -1) s = "E";
                if ("ÍÏÌ".IndexOf(s) > -1) s = "I";
                if ("ÓÖÒ".IndexOf(s) > -1) s = "O";
                if ("ÚÜÙ".IndexOf(s) > -1) s = "U";
                sb.Append(s);
            }
            i += 2;
        }

        return sb.ToString() == texto.ToUpper();
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
            totalColumnas = 8;
            anchoNum = 40;
        }
        else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            totalColumnas = 16;
            anchoNum = 40;
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
            // el formato es nn o espacio char
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
}