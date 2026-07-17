using Rotativa.AspNetCore;

namespace FBSC.HTMLTemplate
{
    public static class Initializer
    {
        public static void Main(string rotativaRootPath)
        {
            RotativaConfiguration.Setup(rotativaRootPath, "Rotativa");
        }
    }
}
