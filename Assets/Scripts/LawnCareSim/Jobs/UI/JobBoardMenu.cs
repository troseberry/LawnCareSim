using LawnCareSim.UI;

namespace LawnCareSim.Jobs
{
    public class JobBoardMenu : BaseMenu
    {
        public static JobBoardMenu Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}
