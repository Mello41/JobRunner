using JobRunner.Core.Entities.ValueObjects;

namespace JobRunner.Core.Interfaces.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScheduleStringConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IScheduleSettings ConvertFromString(string value);

    }
}
