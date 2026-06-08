namespace JobRunner.Core.Platform
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommandLineEscaper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        string EscapeArgument(string argument);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string EscapePath(string path);
    }

}
