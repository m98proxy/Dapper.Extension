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
        public void StatementFactory_BuildSelectStatementTest()
        {
            var expectedStatement = @"select [ID] as Code, [Serial], [NAME] as Name from [STAR_SHIP] where [NAME] = @Name";

            var statement = StatementFactory.Select<StarShip>(Dialect.MSSQL, new { Name = "Millennium Falcon" });

            Assert.AreEqual(expectedStatement, statement);
        }

        [TestMethod]
        public void StatementFactory_BuildInsertStatementTest()
        {
            var expectedStatement = @"insert into [STAR_SHIP]([Serial], [NAME]) values (@Serial, @Name)";

            var statement = StatementFactory.Insert<StarShip>(Dialect.MSSQL);

            Assert.AreEqual(expectedStatement, statement);
        }

        [TestMethod]
        public void StatementFactory_BuildUpdateStatementTest()
        {
            var expectedStatement = @"update [STAR_SHIP] set [Serial] = @Serial , [NAME] = @Name where [ID] = @Code";

            var statement = StatementFactory.Update<StarShip>(Dialect.MSSQL);

            Assert.AreEqual(expectedStatement, statement);
        }

        [TestMethod]
        public void StatementFactory_BuildDeleteStatementTest()
        {
            var expectedStatement = @"delete from [STAR_SHIP] where [ID] = @Code";

            var statement = StatementFactory.Delete<StarShip>(Dialect.MSSQL);

            Assert.AreEqual(expectedStatement, statement);
        }
    }
}
