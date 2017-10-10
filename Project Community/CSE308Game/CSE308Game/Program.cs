namespace CSE308Game
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
           
            //log4net.Config.BasicConfigurator.Configure();
            //log4net.ILog log = log4net.LogManager.GetLogger("logger");
            using (ProjectCommunity game = new ProjectCommunity())
            {
                game.Run();
            }
        }
    }
#endif
}

