using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class FSMTests
    {
        [TestMethod]
        public void Load__d00003_camp()
        {
            FSM fsm = new FSM(Utility.GetFullPath(@"chunk0/quest/d00003/fsm/camp.fsm"));
        }

        [TestMethod]
        public void Load__d00003_DiscoverClear()
        {
            FSM fsm = new FSM(Utility.GetFullPath(@"chunk0/quest/d00003/fsm/DiscoverClear.fsm"));
        }

        [TestMethod]
        public void Load__d00003_main()
        {
            FSM fsm = new FSM(Utility.GetFullPath(@"chunk0/quest/d00003/fsm/main.fsm"));
        }

        [TestMethod]
        public void Load__q00502_main()
        {
            FSM fsm = new FSM(Utility.GetFullPath(@"chunk0/quest/q00502/fsm/main.fsm"));
        }

        [TestMethod]
        public void Load__q00503_main()
        {
            FSM fsm = new FSM(Utility.GetFullPath(@"chunk0/quest/q00503/fsm/main.fsm"));
        }

        [TestMethod]
        public void Load__em002_00()
        {
            FSM fsm = new FSM(Utility.GetFullPath(@"chunk0/quest/q00502/fsm/em/em002_00.fsm"));
        }
    }
}
