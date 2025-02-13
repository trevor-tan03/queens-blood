﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Models;

namespace QueensBloodTest
{
    public class TestCardAbilities : TestBase
    {
        [Fact]
        public void PlaceCardWithRaisePositionRankAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place Twin Brain to raise position rank of tile 
            var twinBrain = _cards[70];
            SetFirstCardInHand(game, twinBrain);
            game.PlaceCard(0, 0, 0);

            AssertTileState(game.Player1Grid, 2, 0, 3, "Player1");
        }

        [Fact]
        public void PlaceCardWithRaisePositionRankAbilityButAlreadyCardOnTile()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place security officer at [2,0]
            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 2, 0);

            // Place Twin Brain but it shouldn't change the position rank of the affected tiles since there's already a card there
            var twinBrain = _cards[70];
            SetFirstCardInHand(game, twinBrain);
            game.PlaceCard(0, 0, 0);

            AssertTileState(game.Player1Grid, 2, 0, 1, "Player1");
        }

        [Fact]
        public void PlaceCardsThatEnhanceWhenCardsArePlayed()
        {
            // This refers to the two cards: Sea Devil and Bagnadrana
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Set middle row of each each player's side have a starting rank of 3
            game.Player1Grid[1, 0].RankUp(2);
            game.Player2Grid[1, 0].RankUp(2);

            // Player 1 places sea devil which raises its power by 1 when an allied card is played (AP)
            var seaDevil = _cards[31];
            SetFirstCardInHand(game, seaDevil);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

            // Player 2 places sea devil which raises its power by 1 when an enemy card is played (EP)
            game.SwapPlayerTurns();
            var bagnadrana = _cards[37];
            SetFirstCardInHand(game, bagnadrana);
            game.PlaceCard(0, 1, 0);

            /* Player 1 places an allied card (security officer) in the first column. This is an enemy card for Player 2.
             * This should raise the power of both the Sea Devil and Bagnadrana.
             */
            game.SwapPlayerTurns();
            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(1, game.Player1Grid[1, 0].SelfBonusPower);
            Assert.Equal(1, game.Player2Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhanceWhileInPlay()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Play Crystalline Crab
            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 1, 0);

            // Tile above should have its power enhanced by 2
            Assert.Equal(2, game.Player1Grid[0, 0].TileBonusPower);

            // When an allied security officer is placed, its cumulative power should be 3 (1+2)
            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(3, game.Player1Grid[0, 0].GetCumulativePower());

            // When Crystalline Crab is destroyed, the security officer should lose its enhancement
            var insectoidChimera = _cards[51];
            SetFirstCardInHand(game, insectoidChimera);
            game.PlaceCard(0, 1, 0);

            Assert.Equal(1, game.Player1Grid[0, 0].GetCumulativePower());
        }

        // Used to place cards in "illegal" places and ignore abilities for more efficient testing
        private void ForcePlace(Game game, Card card, Player owner, int row, int col)
        {
            game.Player1Grid[row, col].Owner = owner;
            game.Player1Grid[row, col].Card = card;
        }

        [Fact]
        public void PlaceCardThatEnhanceWhenFirstPlayed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var securityOfficer = _cards[0];
            ForcePlace(game, securityOfficer, game.Players[0], 0, 0); // Allied card
            ForcePlace(game, securityOfficer, game.Players[1], 0, 1); // Enemy card

            // Loveless is the only card with first played (P) condition with the enhance abiltiy
            var loveless = _cards[138];
            SetFirstCardInHand(game, loveless);
            game.PlaceCard(0, 1, 0);

            Assert.Equal(2, game.Player1Grid[0, 0].GetCumulativePower()); // Allied card enhanced
            Assert.Equal(2, game.Player1Grid[0, 1].GetCumulativePower()); // Enemy card enhanced

            ForcePlace(game, securityOfficer, game.Players[1], 1, 1); // Place enemy card beneath the first enemy card
            // Card shouldn't have bonus power since it wasn't present at the time of Loveless being placed
            Assert.Equal(1, game.Player1Grid[1, 1].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardThatEnfeeblesWhileInPlay()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var elphadunk = _cards[10];
            ForcePlace(game, elphadunk, game.Players[0], 0, 0);
            Assert.Equal(4, game.Player1Grid[0, 0].GetCumulativePower());

            var resurrectedAmalgram = _cards[155];
            SetFirstCardInHand(game, resurrectedAmalgram);
            game.PlaceCard(0, 1, 0);

            // Elphadunk's cumulative power should be reduced to 2
            Assert.Equal(2, game.Player1Grid[0, 0].GetCumulativePower());

            var insectoidChimera = _cards[50];
            SetFirstCardInHand(game, insectoidChimera);
            game.PlaceCard(0, 1, 0); // Replace the Resurrect Amalgram card

            // Elphadunk's power should be back to 4
            Assert.Equal(4, game.Player1Grid[0, 0].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardThatEnfeeblesWhenFirstPlayed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var elphadunk = _cards[10];
            ForcePlace(game, elphadunk, game.Players[0], 0, 0);

            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 1, 0);

            // The Capparwire's ability should have affected the Elphadunk
            Assert.Equal(3, game.Player1Grid[0, 0].GetCumulativePower());

            var securityOfficer = _cards[0];
            ForcePlace(game, securityOfficer, game.Players[0], 2, 0);

            /* Security Officer shouldn't be affected since it wasn't present at the
             * time of the Capparwire's ability being executed.
             */
            Assert.Equal(1, game.Player1Grid[2, 0].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnhancedAsFirstCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place Ifrit at the top of the first column
            var ifrit = _cards[94];
            SetFirstCardInHand(game, ifrit);
            game.Player1Grid[0, 0].RankUp(2);
            game.PlaceCard(0, 0, 0);

            // Place security officer in middle of first column
            ForcePlace(game, _cards[0], game.Players[0], 1, 0);

            // Place crystalline crab at bottom of first column, enhancing security officer
            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 2, 0);

            // Security Officer's power should be enhanced by 2
            Assert.Equal(3, game.Player1Grid[1, 0].GetCumulativePower());
            // Ifrit should enhance its own power because an allied card has been enhanced
            Assert.Equal(2, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnhancedAfterCardsHaveAlreadyBeenPlaced()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 2, 0);

            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(3, game.Player1Grid[1, 0].GetCumulativePower());

            var ifrit = _cards[94];
            SetFirstCardInHand(game, ifrit);
            game.Player1Grid[0, 0].RankUp(2);
            game.PlaceCard(0, 0, 0);
            Assert.Equal(2, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnfeebledAsFirstCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var shadowBloodQueen = _cards[144];
            SetFirstCardInHand(game, shadowBloodQueen);
            game.Player1Grid[0, 0].RankUp(2);
            game.PlaceCard(0, 0, 0);
            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            var grasslandsWolf = _cards[7];
            SetFirstCardInHand(game, grasslandsWolf);
            game.PlaceCard(0, 1, 0);

            // Place Capparwire to enfeeble Grasslands Wolf
            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 2, 0);
            Assert.Equal(-1, game.Player1Grid[1, 0].CardBonusPower);

            // Shadowblood Queen should have enhanced its own power by 3 due to the enfeeble
            Assert.Equal(3, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnfeebledAfterCardsHaveAlreadyBeenPlaced()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var grasslandsWolf = _cards[7];
            SetFirstCardInHand(game, grasslandsWolf);
            game.PlaceCard(0, 1, 0);

            // Place Capparwire to enfeeble Grasslands Wolf
            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 2, 0);
            Assert.Equal(-1, game.Player1Grid[1, 0].CardBonusPower);
            ;
            var shadowBloodQueen = _cards[144];
            SetFirstCardInHand(game, shadowBloodQueen);
            game.Player1Grid[0, 0].RankUp(2);
            game.PlaceCard(0, 0, 0);

            // Shadowblood Queen should have enhanced its own power by 3 due to the enfeeble
            Assert.Equal(3, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void CheckCardWithAEConditionIsWorkingCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            game.Player1Grid[0, 0].RankUp(2);
            var dio = _cards[140];
            SetFirstCardInHand(game, dio);
            game.PlaceCard(0, 0, 0);
            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            // Place security officers for both players
            var securityOfficer = _cards[0];
            ForcePlace(game, securityOfficer, game.Players[1], 0, 4);
            ForcePlace(game, securityOfficer, game.Players[0], 1, 0);

            // Enhance player 2's (enemy) security officer using a Crystalline Crab
            game.SwapPlayerTurns();
            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(2, game.Player2Grid[0, 0].TileBonusPower);

            // Dio should've received its own power by 1
            Assert.Equal(1, game.Player1Grid[0, 0].SelfBonusPower);

            // Enhancing Player 1's (ally) security officer using a Crystalline Crab
            game.SwapPlayerTurns();
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 2, 0);
            Assert.Equal(2, game.Player1Grid[1, 0].TileBonusPower);

            // Dio should have raise its own power by 1 again
            Assert.Equal(2, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void CheckAbilityTriggerWhenItIsDestroyed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var securityOfficer = _cards[0];
            ForcePlace(game, securityOfficer, game.Players[0], 0, 0);

            var sandhogPie = _cards[35];
            SetFirstCardInHand(game, sandhogPie);
            game.PlaceCard(0, 1, 0);

            // Destroy the Sandhog Pie using a replace card (Insectoid Chimera)
            var insectoidChimera = _cards[50];
            SetFirstCardInHand(game, insectoidChimera);
            game.PlaceCard(0, 1, 0);

            // Security Officer should have its power increased by 3 from the destruction of Sandhog Pie
            Assert.Equal(3, game.Player1Grid[0, 0].CardBonusPower);
        }

        [Fact]
        public void CheckWhenFirstEnhancedAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place a Security Officer in 1, 1 of Player1Grid
            ForcePlace(game, _cards[0], game.Players[0], 1, 1);
            Assert.Equal("Security Officer", game.Player1Grid[1, 1].Card!.Name);

            // Place the Elena card so that it targets the Security Officer
            var elena = _cards[129];
            SetFirstCardInHand(game, elena);
            game.PlaceCard(0, 1, 0);

            // Place Crystalline Crab to enhance Elena
            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 2, 0);

            // Security Officer card is gone
            Assert.Null(game.Player1Grid[1, 1].Card);

            /* The "CardBonusPower" of the Tile (1, 1) should be reset to 0 since the
             * Security Officer card was destroyed
             */
            Assert.Equal(0, game.Player1Grid[1, 1].CardBonusPower);
        }

        [Fact]
        public void CheckWhenFirstEnfeebledAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place an enemy Archdragon in 1, 1 of Player1Grid
            ForcePlace(game, _cards[19], game.Players[1], 1, 1);

            // Place Reapertail card
            var reapertail = _cards[60];
            SetFirstCardInHand(game, reapertail);
            game.PlaceCard(0, 2, 0);

            // Enfeeble the Reapertail using a Capparwire
            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 1, 0);

            // The Archdragon's power should be reduced by 2 (final power of 1)
            Assert.Equal(-2, game.Player1Grid[1, 1].CardBonusPower);
            Assert.Equal(1, game.Player1Grid[1, 1].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardWithADTriggerCondition()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place an allied Security Officer
            ForcePlace(game, _cards[0], game.Players[0], 0, 0);

            var tonberryKing = _cards[34];
            SetFirstCardInHand(game, tonberryKing);
            game.Player1Grid[1, 0].RankUp(1);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

            // Place a Gigantoad to replace (and destroy) the allied Security Officer
            var gigantoad = _cards[51];
            SetFirstCardInHand(game, gigantoad);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(2, game.Player1Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardWithEDTriggerCondition()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place an enemy Security Officer
            ForcePlace(game, _cards[0], game.Players[1], 0, 4);

            var deathClaw = _cards[42];
            SetFirstCardInHand(game, deathClaw);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

            // Place a Gigantoad to replace (and destroy) the enemy Security Officer
            game.SwapPlayerTurns();
            var gigantoad = _cards[51];
            SetFirstCardInHand(game, gigantoad);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(1, game.Player1Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardWithAEDTriggerCondition()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Player 1: Place Joker
            var joker = _cards[46];
            SetFirstCardInHand(game, joker);
            game.Player1Grid[1, 0].RankUp(1);
            game.PlaceCard(0, 1, 0);

            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            // Player 1: Place Security Officer
            ForcePlace(game, _cards[0], game.Players[0], 2, 4);

            // Player 2: Place Security Officer
            ForcePlace(game, _cards[0], game.Players[1], 0, 4);

            // Player 2: Place Capparwire to enfeeble/destroy both the Security Officers
            game.SwapPlayerTurns();
            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 1, 0);

            // Confirm the Security Officer cards are gone
            Assert.Null(game.Player1Grid[2, 4].Card);
            Assert.Null(game.Player1Grid[0, 4].Card);

            Assert.Equal(2, game.Player1Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardWithChildrenCards()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var moogleTrio = _cards[109];
            SetFirstCardInHand(game, moogleTrio);
            game.Player1Grid[0, 0].RankUp(1);
            game.PlaceCard(0, 0, 0);

            // Confirm Player 1 has Moogle Mage & Moogle Bard added to hand
            var moogleMage = _cards[147];
            var moogleBard = _cards[148];

            Assert.Contains<Card>(moogleMage, game.Players[0].Hand);
            Assert.Contains<Card>(moogleBard, game.Players[0].Hand);
        }

        [Fact]
        public void PlaceCardWithSpawnAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Have 3 tiles, each with different ranks (1,2,3)
            game.Player1Grid[0, 0].RankUp(1); // Shiva's position

            game.Player1Grid[0, 1].Owner = game.Players[0];
            game.Player1Grid[0, 1].RankUp(1); // Diamond Dust (2)
            game.Player1Grid[1, 0].RankUp(1); // Diamond Dust (4)
            game.Player1Grid[2, 0].RankUp(2); // Diamond Dust (6)

            // Place Shiva
            var shiva = _cards[95];
            SetFirstCardInHand(game, shiva);
            game.PlaceCard(0, 0, 0);

            // Confirm that the different Diamond Dust cards are spawned
            Assert.NotNull(game.Player1Grid[0, 1]);
            Assert.Equal(2, game.Player1Grid[0, 1].Card!.Power);

            Assert.NotNull(game.Player1Grid[1, 0]);
            Assert.Equal(4, game.Player1Grid[1, 0].Card!.Power);

            Assert.NotNull(game.Player1Grid[2, 0]);
            Assert.Equal(6, game.Player1Grid[2, 0].Card!.Power);
        }

        [Fact]
        public void VerifyBonusLanePointsWhenPlayed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var tifa = _cards[87];
            SetFirstCardInHand(game, tifa);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(5, game.Players[0].Scores[0].winBonus);
        }

        [Fact]
        public void VerifyBonusPointsRemovedWhenCardDestroyed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var tifa = _cards[87];
            SetFirstCardInHand(game, tifa);
            game.PlaceCard(0, 0, 0);

            // Destroy Tifa card
            var insectoidChimera = _cards[51];
            SetFirstCardInHand(game, insectoidChimera);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(0, game.Players[0].Scores[0].winBonus);
        }

        [Fact]
        public void TestUltimatePartyAnimalAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var UPA = _cards[141];
            SetFirstCardInHand(game, UPA);
            game.PlaceCard(0, 0, 0);

            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 0, 1);
            Assert.Equal(2, game.GetPlayerLaneScore(0, 0));

            game.SwapPlayerTurns();
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 0, 0);

            game.EndGame();
            var finalScores = game.GetFinalScores();

            Assert.Equal(3, finalScores.player1Score);
            Assert.Equal(1, finalScores.player2Score);
        }

        [Fact]
        public void TestCardWithDestroyAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place down enemy Security Officer cards
            ForcePlace(game, _cards[0], game.Players[1], 0, 0);
            ForcePlace(game, _cards[0], game.Players[1], 2, 0);

            // Place down Sephiroth card
            var sephiroth = _cards[142];
            game.Player1Grid[1, 0].RankUp(2);
            SetFirstCardInHand(game, sephiroth);
            game.PlaceCard(0, 1, 0);

            // Security Officers should be destroyed
            Assert.Null(game.Player1Grid[0, 0].Card);
            Assert.Null(game.Player1Grid[2, 0].Card);
        }
    }
}
