namespace FFMSSharp
{
    /// <summary>
    /// Simple representation of a selection rectangle
    /// </summary>
    public class Selection
    {
        /// <summary>
        /// Amount of Top to crop
        /// </summary>
        public int Top { get; private set; }

        /// <summary>
        /// Amount of Left to crop
        /// </summary>
        public int Left { get; private set; }

        /// <summary>
        /// Amount of Right to crop
        /// </summary>
        public int Right { get; private set; }

        /// <summary>
        /// Amount of Bottom to crop
        /// </summary>
        public int Bottom { get; private set; }

        internal Selection(int top, int left, int right, int bottom)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }
    }
}