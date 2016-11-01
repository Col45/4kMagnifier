namespace _4kMagnifier
{
    using System.Windows.Input;

    public class EntryPoint
    {
        [System.STAThreadAttribute]
        static void Main()
        {
            using (var hook = new KeyboardHook { HideKey = Key.F4, CloseKey = Key.F5 })
            {
                var app = new App(hook);
                //app.InitializeComponent();
                app.Run();
            }
        }
    }
}
