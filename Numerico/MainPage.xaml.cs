//--------------------------------------------------------------------------
// Pasatiempo numérico para .NET MAUI                           (02/mar/23)
//
// (c) Guillermo Som (Guille), 2023
//-------------------------------------------------------------------------- 
using System.Text;

using Pasatiempo.Numerico;

namespace Numerico;

public partial class MainPage : ContentPage
{
    private JuegoNumerico.DatosNumerico ElJuego = null;
    private int NumeroJuego;
    private bool SolucionMostrada = false;

    public MainPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        LabelNumero.Text = $"Indica el número del juego, de {JuegoNumerico.NumJuegoMin} a {JuegoNumerico.NumJuegoMax} (ambos inclusive).";
    }

    private void BtnGenerar_Clicked(object sender, EventArgs e)
    {
        // por ahora solo hay un juego disponible
        int numJuego = 1;
        if (Int32.TryParse(txtNumJuego.Text, out numJuego) == false)
        {
            LabelNumero.Text = "No es un número válido, se utiliza el juego número 1.";
        }
        if (numJuego < JuegoNumerico.NumJuegoMin || numJuego > JuegoNumerico.NumJuegoMax)
        {
            LabelNumero.Text = "No es un número válido, se utiliza el juego número 1.";
            numJuego = 1;
        }
        else
        {
            LabelNumero.Text = $"Se utiliza el juego número {numJuego}.";
        }

        LabelInfo.IsVisible = true;

        NumeroJuego = numJuego;
        MostrarJuegoNumerico();

        LabelInfo.IsVisible = false;
    }

    private void LabelExpander_Tapped(object sender, TappedEventArgs e)
    {
        grbExpander.IsVisible = !grbExpander.IsVisible;
        MostrarImagenExpander(ImgExpander, grbExpander.IsVisible);
    }
    private void LabelExpander2_Tapped(object sender, TappedEventArgs e)
    {
        grbNumerico.IsVisible = !grbNumerico.IsVisible;
        MostrarImagenExpander(ImgExpander2, grbNumerico.IsVisible);
    }

    /// <summary>
    /// Mostrar la imagen que corresponde del expander.
    /// </summary>
    /// <param name="img">El control de imagen a asignar.</param>
    /// <param name="expanded">Si se debe mostrar expanded (false) o collapse (true).</param>
    private static void MostrarImagenExpander(Image img, bool expanded)
    {
        string imgSource;
        if (expanded)
        {
            imgSource = "collapse_white.png";
        }
        else
        {
            imgSource = "expand_white.png";
        }
        // En .NET MAUI solo se indica el nombre del fichero que estará en Resources/Images
        img.Source = imgSource;
    }

    // El código de Numerico.xaml.cs

    private async void MostrarJuegoNumerico()
    {
        LabelExpander_Tapped(null, null);
        LabelExpander2_Tapped(null, null);

        if (ElJuego == null)
        {
            LabelInfo.Text = "Leyendo los datos del juego...";
            LabelInfo.IsVisible = true;
            grbNumerico.IsVisible = false;
            grbBotones.IsEnabled = false;

            ElJuego = await JuegoNumerico.LeerJuego(NumeroJuego);

            LabelInfo.IsVisible = false;
        }

        // Asignar los datos
        MostrarJuego(conSolucion: false);
    }

    private void BtnSolucion_Clicked(object sender, EventArgs e)
    {
        // Mostrar la solución sin tener que crear los controles,   (05/mar/23 15.40)
        // al estilo de como se comprueba si lo ha resuelto.

        SolucionMostrada = !SolucionMostrada;
        //MostrarJuego(conSolucion: SolucionMostrada);
        MostrarSolucion(SolucionMostrada);

        if (SolucionMostrada)
        {
            BtnSolucion.Text = "Ocultar solución";
        }
        else
        {
            BtnSolucion.Text = "Mostrar solución";
        }
    }

    private async void BtnComprobar_Clicked(object sender, EventArgs e)
    {
        // Comprobar si se ha resuelto el pasatiempo numérico
        LabelInfo.Text = "Comprobando si está resuelto el pasatiempo numérico...";
        LabelInfo.IsVisible = true;
        grbBotones.IsEnabled = false;

        // Comprobar
        int resueltos = await ComprobarJuego();
        if (resueltos == 3)
        {
            LabelInfo.Text = "El juego está resuelto.";
        }
        else
        {
            LabelInfo.Text = "El juego NO está resuelto.";
            if (resueltos > 0)
            {
                LabelInfo.Text += $" Has resuelto {resueltos} de 3.";
            }
        }
        //LabelInfo.IsVisible = false;
        grbBotones.IsEnabled = true;
    }

    private void MostrarSolucion(bool mostrar)
    {
        AsignarSolucion(grbAutor, ElJuego.Autor, mostrar);
        AsignarSolucion(grbTitulo, ElJuego.Titulo, mostrar);
        AsignarSolucion(grbContenido, ElJuego.Contenido, mostrar);
    }

    private static void AsignarSolucion(StackLayout grb, string texto, bool mostrar)
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
                // Si no es una de las letras a cambiar
                if (mostrar == false && char.IsLetter(c))
                {
                    c = ' ';
                }
                n++;
                vLetra.Text = c.ToString();
            }
            i += 2;
        }
    }

    private async Task<int> ComprobarJuego()
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

    private static bool ComprobarContenido(StackLayout grb, string texto)
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

    private async void MostrarJuego(bool conSolucion)
    {
        grbBotones.IsEnabled = false;

        if (conSolucion)
        {
            LabelInfo.Text = "Asignando la solución del juego...";
        }
        else
        {
            LabelInfo.Text = "Asignando los datos del juego...";
        }
        LabelInfo.IsVisible = true;

        LabelTitulo.Text = $"Pasatiempo numérico, juego actual: {NumeroJuego}";
        if (conSolucion)
        {
            LabelTitulo.Text += " (solución)";
        }
        grbNumerico.IsVisible = false;

        await Task.Run(async () =>
        {
            await Task.Run(() => AsignarContenido(grbAutor, ElJuego.Autor_N, conSolucion));
            await Task.Run(() => AsignarContenido(grbTitulo, ElJuego.Titulo_N, conSolucion));
            await Task.Run(() => AsignarContenido(grbContenido, ElJuego.Contenido_N, conSolucion));
        });

        grbNumerico.IsVisible = true;
        LabelInfo.IsVisible = false;
        grbBotones.IsEnabled = true;

        SolucionMostrada = conSolucion;
    }

    private void AsignarContenido(StackLayout grb, string texto, bool conSolucion)
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
        }

        _ = grb.Dispatcher.Dispatch(() =>
        {
            grb.Children.Clear();
            int n = -2;
            int col = 0;
            StackLayout grbFilaNum = new StackLayout
            {
                //BackgroundColor = Colors.WhiteSmoke,
                Orientation = StackOrientation.Horizontal
            };
            StackLayout grbFilaLetra = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
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
                }
                else
                {
                    int i = Convert.ToInt32(s);
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
                Label celdaNum = new Label
                //Entry celdaNum = new Entry
                {
                    FontFamily = "Consolas",
                    WidthRequest = anchoNum, // anchoEntry + 4,
                    HeightRequest = altoNum,
                    Text = elNum,
                    IsVisible = true,
                    //IsReadOnly = true,
                    BackgroundColor = Colors.WhiteSmoke
                };
                Entry celdaLetra = new Entry
                {
                    FontFamily = "Consolas",
                    HorizontalTextAlignment = TextAlignment.Center,
                    WidthRequest = anchoEntry,
                    HeightRequest = altoLetra,
                    Text = laLetra,
                    IsVisible = true
                };
                
                if (laLetra == " ")
                {
                    celdaLetra.BackgroundColor = Colors.Gray;
                    celdaLetra.IsReadOnly = true;
                }
                else
                {
                    celdaLetra.Completed += Entry_Completed;
                    celdaLetra.Unfocused += Entry_Unfocused;
                }
                //celdaLetra.Completed += Entry_Completed;
                //celdaLetra.Unfocused += Entry_Unfocused;

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
                        //BackgroundColor = Colors.WhiteSmoke,
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

    private bool yaEstoy = false;
    private void Entry_Completed(object sender, EventArgs e)
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
        
        yaEstoy = false;
    }
    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        Entry_Completed(sender, e);
    }

    private void StackLayout_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsEnabled")
        {
            BtnComprobar.IsEnabled = grbBotones.IsEnabled;
            BtnSolucion.IsEnabled = grbBotones.IsEnabled;
        }
    }
}

