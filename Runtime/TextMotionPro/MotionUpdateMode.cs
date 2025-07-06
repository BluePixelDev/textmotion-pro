namespace BP.TextMotionPro
{
    public enum MotionUpdateMode
    {
        /// <summary>
        /// Updates only during runtime.
        /// </summary>
        Runtime = 0,

        /// <summary>
        /// Updates during both runtime and while editing in the Unity Editor.
        /// </summary>
        Always = 1,

        /// <summary>
        /// Updates during runtime and in the Unity Editor only when the object is selected.
        /// </summary>
        SelectedInEditor = 2,
    }
}
