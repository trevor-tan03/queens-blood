using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Models;
using backend.Utility;
using Microsoft.Data.Sqlite;

namespace QueensBloodTest
{
    public abstract class TestBase
    {
        protected List<Card> _cards = new List<Card>();

        public TestBase()
        {
            SQLitePCL.Batteries.Init();

            var connectionString = "Data Source=../../../../backend/QB_card_info.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                ReadDatabase.PopulateCards(connection, _cards);
                ReadDatabase.SetChildCards(connection, _cards);

                CreateGameWithPlayers();
            }
        }

        public Game CreateGameWithPlayers()
        {
            Game game = new Game("myGameId");

            game.AddPlayer("Player1_ID", "Player1");
            game.AddPlayer("Player2_ID", "Player2");

            int[] defaultDeck = { 1, 1, 2, 7, 7, 8, 8, 11, 11, 12, 12, 13, 13, 98, 107 };

            for (int i = 0; i < 15; i++)
            {
                Card card = _cards[defaultDeck[i] - 1];

                foreach (Player player in game.Players)
                {
                    player.Deck.Add(card);
                }
            }

            return game;
        }

        public void AssertTileState(Tile[,] grid, int row, int column, int expectedRank, string expectedOwner)
        {
            Assert.Equal(expectedRank, grid[row, column].Rank);
            Assert.Equal(expectedOwner, grid[row, column].Owner!.Name);
        }

        public void SetPlayer1Start(Game game)
        {
            game.CurrentPlayer = game.Players[0];
        }

        public void AddToHandAndPlaceCard(Game game, Cards cardEnum, int row, int col)
        {
            var card = _cards[(int) cardEnum];
            game.CurrentPlayer!.Hand.Add(card);

            var lastCardInHandIndex = game.CurrentPlayer.Hand.Count - 1;
            game.PlaceCard(lastCardInHandIndex, row, col);
        }
    }
}
