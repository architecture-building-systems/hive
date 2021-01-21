namespace Hive.IO.Forms
{
    public abstract class ViewModelBase
    {
        /// <summary>
        ///     Parses the string to a double or returns the oldValue on error.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        protected double ParseDouble(string value, double oldValue)
        {
            double result;
            if (double.TryParse(value, out result)) return result;

            return oldValue;
        }
    }
}