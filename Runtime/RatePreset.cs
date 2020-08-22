namespace UniRate {

    public struct RatePreset {

        #region <<---------- Initializers ---------->>
        
        public RatePreset(int updateRate, int fixedUpdateRate, int renderInterval) {
            this.UpdateRate = updateRate;
            this.FixedUpdateRate = fixedUpdateRate;
            this.RenderInterval = renderInterval;
        }
        
        #endregion <<---------- Initializers ---------->>
        
        
        
        
        #region <<---------- Properties and Fields ---------->>
        
        /// <summary>
        /// Update rate.
        /// </summary>
        public int UpdateRate { get; set; }

        /// <summary>
        /// Fixed Update rate.
        /// </summary>
        public int FixedUpdateRate { get; set; }

        /// <summary>
        /// Render Interval.
        /// </summary>
        public int RenderInterval { get; set; }
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Default Presets ---------->>
        
        /// <summary>
        /// Ultra preset with update rate of 120, fixed update rate of 100 and render interval of 1.
        /// </summary>
        public static readonly RatePreset Ultra = new RatePreset(120, 100, 1);

        /// <summary>
        /// Very high preset with update rate of 90, fixed update rate of 75 and render interval of 1.
        /// </summary>
        public static readonly RatePreset VeryHigh = new RatePreset(90, 75, 1);

        /// <summary>
        /// High preset with update rate of 60, fixed update rate of 50 and render interval of 1.
        /// </summary>
        public static readonly RatePreset High = new RatePreset(60, 50, 1);

        /// <summary>
        /// Medium preset with update rate of 30, fixed update rate of 40 and render interval of 2.
        /// </summary>
        public static readonly RatePreset Medium = new RatePreset(30, 40, 2);

        /// <summary>
        /// Low preset with update rate of 24, fixed update rate of 30 and render interval of 3.
        /// </summary>
        public static readonly RatePreset Low = new RatePreset(24, 30, 3);

        /// <summary>
        /// Very low preset with update rate of 20, fixed update rate of 20 and render interval of 4.
        /// </summary>
        public static readonly RatePreset VeryLow = new RatePreset(20, 20, 4);
        
        #endregion <<---------- Default Presets ---------->>
    }
}