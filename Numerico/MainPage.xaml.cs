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
        if (int.TryParse(txtNumJuego.Text, out numJuego) == false)
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

        NumericoHelpers.NumeroJuego = numJuego;
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

    private void BtnSolucion_Clicked(object sender, EventArgs e)
    {
        // Mostrar la solución sin tener que crear los controles,   (05/mar/23 15.40)
        // al estilo de como se comprueba si lo ha resuelto.

        NumericoHelpers.SolucionMostrada = !NumericoHelpers.SolucionMostrada;
        //MostrarJuego(conSolucion: SolucionMostrada);
        MostrarSolucion(NumericoHelpers.SolucionMostrada);

        if (NumericoHelpers.SolucionMostrada)
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
            LabelInfo.Text = "¡Enhorabuena! El juego está resuelto.";
        }
        else
        {
            LabelInfo.Text = "El juego NO está resuelto.";
            if (resueltos > 0)
            {
                LabelInfo.Text += $" Has resuelto {resueltos} de las 3 partes.";
            }
        }
        grbBotones.IsEnabled = true;
    }

    /// <summary>
    /// Leer y mostrar el juego indicado en NumeroJuego.
    /// </summary>
    private async void MostrarJuegoNumerico()
    {
        LabelExpander_Tapped(null, null);
        LabelExpander2_Tapped(null, null);

        if (NumericoHelpers.ElJuego == null)
        {
            LabelInfo.Text = "Leyendo los datos del juego...";
            LabelInfo.IsVisible = true;
            grbNumerico.IsVisible = false;
            grbBotones.IsEnabled = false;

            NumericoHelpers.ElJuego = await JuegoNumerico.LeerJuego(NumericoHelpers.NumeroJuego);

            LabelInfo.IsVisible = false;
        }

        // Asignar los datos sin mostrar la solución
        MostrarJuego(conSolucion: false);
    }

    /// <summary>
    /// Mostrar u ocultar la solución.
    /// </summary>
    /// <param name="mostrar">True si se debe mostrar la solución, false si solo se deja lo escrito por el usuario</param>
    private void MostrarSolucion(bool mostrar)
    {
        NumericoHelpers.AsignarSolucion(grbAutor, NumericoHelpers.ElJuego.Autor, mostrar);
        NumericoHelpers.AsignarSolucion(grbTitulo, NumericoHelpers.ElJuego.Titulo, mostrar);
        NumericoHelpers.AsignarSolucion(grbContenido, NumericoHelpers.ElJuego.Contenido, mostrar);
    }

    /// <summary>
    /// Comprobar si se ha resuelto el juego
    /// </summary>
    /// <returns>El número de secciones resueltas</returns>
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
            if (NumericoHelpers.ComprobarContenido(grbAutor, NumericoHelpers.ElJuego.Autor)) resueltos++;
            if (NumericoHelpers.ComprobarContenido(grbTitulo, NumericoHelpers.ElJuego.Titulo)) resueltos++;
            if (NumericoHelpers.ComprobarContenido(grbContenido, NumericoHelpers.ElJuego.Contenido)) resueltos++;
        });

        return resueltos;
    }

    /// <summary>
    /// Mostrar el juego creando los controles con las letras y los números
    /// </summary>
    /// <param name="conSolucion">True si se debe mostrar la solución</param>
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

        LabelTitulo.Text = $"Pasatiempo numérico, juego actual: {NumericoHelpers.NumeroJuego}";
        if (conSolucion)
        {
            LabelTitulo.Text += " (solución)";
        }
        grbNumerico.IsVisible = false;

        await Task.Run(async () =>
        {
            await Task.Run(() => NumericoHelpers.AsignarContenido(grbAutor, NumericoHelpers.ElJuego.Autor_N, conSolucion));
            await Task.Run(() => NumericoHelpers.AsignarContenido(grbTitulo, NumericoHelpers.ElJuego.Titulo_N, conSolucion));
            await Task.Run(() => NumericoHelpers.AsignarContenido(grbContenido, NumericoHelpers.ElJuego.Contenido_N, conSolucion));
        });

        grbNumerico.IsVisible = true;
        LabelInfo.IsVisible = false;
        grbBotones.IsEnabled = true;

        NumericoHelpers.SolucionMostrada = conSolucion;
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

