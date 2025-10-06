using NUnit.Framework;

[TestFixture]
public class MapLogicTest
{
    [Test]
    public void CheckIfTheReturnStringContainsX()
    {
        string mapOutput = MapLogic.MapBuilder(5);

        bool result = mapOutput.Contains("X");

        Assert.IsTrue(result, "The map output should contain an 'X'.");
    }

    [Test]
    public void CheckForCorrectOutput()
    {

        string expectedOutput = @"                               
                                                           ╔═══════╦══╦══╦══╦══╦═════╗
                            ╔════════════╦════════════╗    ║       │  │  │  │  │     ║
                            ║            ║            ║    ║    ╔══╩══╩══╩══╩══╩═╗   ║
                            ║            ║            ║    ╠════╣                ║═══╣
                            ║            ║            ║    ║    ║                ║   ║
                            ║            ║            ║    ╠════╣                ║═══╣
                            ║            ║            ║    ║    ║                ║   ║
                            ╠════════ WC ╩ WC ═════════════╩════╝                ║═══╣
                            ║                                                    ║   ║
                            ║                                                    ╚═══╣
                            ║  ┌─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┐                           ║
                            ║  │ │ │ │ │ │ │ │ │ │ │ │ │ │                           ║
                            ║  └─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┘                           ║
                            ║  ┌─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┐                           ║
                            ║  │ │ │ │ │ │ │ │ │ │ │ │ │ │                           ║
                            ║  └─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┘                  ▓▓▓▓▓▓▓▓▓║
                            ║  ┌─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┐                ▓▓▓▓▓▓▓▓▓▓▓║
                            ║  │ │ │ │ │ │ │ │ │ │ │ │ │ │               ▓▓▓▓▓       ║
                            ║  └─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┘               ▓▓▓▓        ║
                            ║                                            ▓▓▓▓        ║
                            ║          CHECKOUT                          ▓▓▓▓        ║
                            ║    ░░   ░░   ░░   ░░   ░░                  ▓▓▓▓        ║
                            ║    ░░   ░░   ░░   ░░   ░░                  ▓▓▓▓  ╔══╗  ║
                            ║    ░░   ░░   ░░   ░░   ░░                  ▓▓▓▓  ║  ║  ║
                            ╚═════════════════════════════ ENTER ══ EXIT ══════╩══╩══╝";
        string actualOutput = MapLogic.MapBuilder(1);

        Assert.AreEqual(expectedOutput, actualOutput, "ERROR - The map output is not as expected.");
    }
}
