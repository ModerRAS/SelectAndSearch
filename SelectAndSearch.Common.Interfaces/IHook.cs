namespace SelectAndSearch.Common.Interfaces {
    public interface IHook {
        public void StartHook();
        public void StopHook();

        /// <summary>
        /// 声明委托
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        public delegate int GlobalHookProc(int nCode, int wParam, IntPtr lParam);
    }
}
