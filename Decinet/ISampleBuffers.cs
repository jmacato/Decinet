/**************************************************************
 * License detail will be added later
 * 
 **************************************************************/
namespace LibResample.Sharp
{
    /// <summary>
    /// Callback for producing and consuming samples. Enalbes on-the-fly conversion between sample types
    /// (signed 16-bit integers to floats, for example) and/or writing directly to an output stream.
    /// </summary>
    public interface ISampleBuffers
    {
        /// <summary>
        /// Get the number of input samples available
        /// </summary>
        /// <returns>number of input samples available</returns>
        int GetInputBufferLenght();
        /// <summary>
        /// Get the number of samples the output buffer has room for
        /// </summary>
        /// <returns>number of samples the output buffer has room for</returns>
        int GetOutputBufferLength();
        /// <summary>
        /// Copy lenght samples from the input buffer to the given array, starting at the 
        /// given offset. Samples should be in the range -1.0f to 1.0f
        /// </summary>
        /// <param name="array">array to hold samples from the input buffer</param>
        /// <param name="offset">start writing samples here</param>
        /// <param name="length">write this many samples</param>
        void ProduceInput(float[] array, int offset, int length);
        /// <summary>
        /// Copy length samples from the given array to the output buffer, starting at the given offset.
        /// </summary>
        /// <param name="array">array to read from</param>
        /// <param name="offset">start reading samples here</param>
        /// <param name="length">read this many samples</param>
        void ConsumeOutput(float[] array, int offset, int length);
    }
}
