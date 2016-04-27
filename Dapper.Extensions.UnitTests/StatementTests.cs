using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dapper
{
    [TestClass]
    public class StatementTests
    {
        private StarShip CreateStarShip()
        {
            var starShip = new StarShip();

            starShip.Code = Guid.NewGuid();

            starShip.Serial = "YT-1300";

            starShip.Name = "Millennium Falcon";

            starShip.Pilot = "Han Solo";

            return starShip;
        }

        [TestMethod]
        public void StatementFactory_BuilSelectStatementTest()
        {
            var expectedStatement = @"select [ID] as Code, [Serial], [NAME] as Name, [Pilot] from [STAR_SHIP] where [NAME] = @Name";

            var statement = StatementFactory.Select<StarShip>(new { Name = "Millennium Falcon" });

            Assert.AreEqual(expectedStatement, statement);
        }
    }
}
