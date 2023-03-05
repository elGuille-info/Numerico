namespace Numerico;

public partial class App : Application
{

#if WINDOWS
    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        // Add here your sizing code

        window.Width = 1100;
        window.Height = 900;

        // Add here your positioning code

        DisplayInfo disp = DeviceDisplay.Current.MainDisplayInfo;
        window.X = (disp.Width / disp.Density - window.Width * disp.Density) / 2;
        window.Y = (disp.Height / disp.Density - window.Height * disp.Density) / 2;

        window.Title = "Pasatiempo Numérico para .NET MAUI";

        // Si se van a usar páginas a la que navegar, no asignar el color al título.

        //// Cambiar el color de la barra de Windows
        //Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        //{
        //    var mauiWindow = handler.VirtualView;
        //    var nativeWindow = handler.PlatformView;
        //    nativeWindow.Activate();
        //    IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
        //    var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
        //    var window = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        //    // dispatcher is used to give the window time to actually resize
        //    Dispatcher.Dispatch(() =>
        //    {
        //        // Este es el color que tiene en mi equipo la barra de título.
        //        window.TitleBar.BackgroundColor = Microsoft.UI.ColorHelper.FromArgb(255, 0, 120, 212);
        //        window.TitleBar.ForegroundColor = Microsoft.UI.Colors.White;
        //    });
        //});

        return window;
    }
#endif

    public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
