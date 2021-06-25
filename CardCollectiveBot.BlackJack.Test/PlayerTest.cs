using CardCollectiveBot.DeckOfCards;
using NUnit.Framework;

namespace CardCollectiveBot.BlackJack.Test
{
    [TestFixture]
    public class PlayerTest
    {

        private Player PlayerToTest;

        [SetUp]
        public void SetUp()
        {
            PlayerToTest = new Player(1, "TestNickname");
        }

        [Test]
        public void PlayerConstructor_Should_Set_Properties_Correctly()
        {
            ulong id = 1;
            var nickname = "TestNickname";

            PlayerToTest = new Player(id, nickname);

            Assert.AreEqual(id, PlayerToTest.Id);
            Assert.AreEqual(nickname, PlayerToTest.Nickname);
            Assert.IsTrue(PlayerToTest.Hand != null);
            Assert.IsTrue(PlayerToTest.Hand.Count == 0);
            Assert.AreEqual(PlayerState.Choosing, PlayerToTest.State);
        }

        [Test]
        public void Stand_Should_Set_State_To_Stand()
        {
            Assert.AreNotEqual(PlayerState.Stand, PlayerToTest.State);

            PlayerToTest.Stand();

            Assert.AreEqual(PlayerState.Stand, PlayerToTest.State);
        }

        [Test]
        public void ResetState_Should_Set_State_To_Choosingd()
        {
            PlayerToTest.Stand();

            Assert.AreNotEqual(PlayerState.Choosing, PlayerToTest.State);

            PlayerToTest.ResetState();

            Assert.AreEqual(PlayerState.Choosing, PlayerToTest.State);
        }

        [TestCase(PlayerState.Hit, 20, 8, 12, 2)]
        [TestCase(PlayerState.BlackJack, 21, 10, 14)]
        [TestCase(PlayerState.Hit, 12, 14, 14)]
        [TestCase(PlayerState.Bust, 26, 8, 8, 12)]
        public void TakeCard_Given_Cards_Set_Expected_State_And_Expected_CountScore_Value(PlayerState expectedState, int expectedScore, params int[] cardValaues)
        {
            foreach(var val in cardValaues)
            {
                PlayerToTest.TakeCard(new PlayingCard(SuitEnum.Club, val));
            }

            Assert.AreEqual(expectedState, PlayerToTest.State);
            Assert.AreEqual(expectedScore, PlayerToTest.CountScore());
        }
    }
}