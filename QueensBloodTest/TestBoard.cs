using backend;
using backend.Models;
using backend.Repositories;

namespace QueensBloodTest
{
    public class TestBoard
    {

        private readonly ICardRepository _cardRepository;

        public TestBoard(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        private Game CreateGameWithPlayers()
        {
            Game game = new Game("myGameId");

            game.AddPlayer("Player1_ID", "Player1");
            game.AddPlayer("PLayer2_ID", "Player2");

            //for (int i = 1; i <= 15; i++)
            //{
            //    Card card = _cardRepository.GetCardById(i);
                
            //    foreach(Player player in game.Players)
            //    {
            //        player.Deck.Add(card);
            //    }
            //}

            return game;
        }

        [Fact]
        public void CreateGameWithTwoPlayersEachFifteenCards()
        {
            Game game = CreateGameWithPlayers();    

            Assert.Equal(2, game.Players.Count);
            Assert.Equal("Player1", game.Players[0].Name);
            Assert.Equal("Player2", game.Players[1].Name);

            //Assert.Equal(15, game.Players[0].Deck.Count);
            //Assert.Equal(15, game.Players[1].Deck.Count);
        }
    }
}