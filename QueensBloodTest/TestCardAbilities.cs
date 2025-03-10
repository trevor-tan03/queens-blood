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
        public void PlaceReplaceCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 0);
            AddToHandAndPlaceCard(game, Cards.InsectoidChimera, 1, 0);

            // Ensure affected tiles are updated correctly
            AssertTileState(game.Player1Grid, 0, 0, 3, "Player1");
            AssertTileState(game.Player1Grid, 1, 1, 2, "Player1");
            AssertTileState(game.Player1Grid, 2, 0, 3, "Player1");
        }

        [Fact]
        public void PlaceCardWithRaisePositionRankAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place Twin Brain to raise position rank of tile 
            AddToHandAndPlaceCard(game, Cards.TwinBrain, 0, 0);
            AssertTileState(game.Player1Grid, 2, 0, 3, "Player1");
        }

        [Fact]
        public void PlaceCardWithRaisePositionRankAbilityButAlreadyCardOnTile()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place security officer at [2,0]
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 2, 0);

            // Place Twin Brain but it shouldn't change the position rank of the affected tiles since there's already a card there
            AddToHandAndPlaceCard(game, Cards.TwinBrain, 0, 0);
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
            AddToHandAndPlaceCard(game, Cards.SeaDevil, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

            // Player 2 places sea devil which raises its power by 1 when an enemy card is played (EP)
            game.SwapPlayerTurns();
            AddToHandAndPlaceCard(game, Cards.Bagnadrana, 1, 0);
            /* Player 1 places an allied card (security officer) in the first column. This is an enemy card for Player 2.
             * This should raise the power of both the Sea Devil and Bagnadrana.
             */
            game.SwapPlayerTurns();
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);

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
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 1, 0);
            game.PlaceCard(0, 1, 0);

            // Tile above should have its power enhanced by 2
            Assert.Equal(2, game.Player1Grid[0, 0].TileBonusPower);

            // When an allied security officer is placed, its cumulative power should be 3 (1+2)
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);
            Assert.Equal(3, game.Player1Grid[0, 0].GetCumulativePower());

            // When Crystalline Crab is destroyed, the security officer should lose its enhancement
            AddToHandAndPlaceCard(game, Cards.InsectoidChimera, 1, 0);
            Assert.Equal(1, game.Player1Grid[0, 0].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardThatEnhanceWhenFirstPlayed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            ForcePlace(game, Cards.SecurityOfficer, game.Players[0], 0, 0); // Allied card
            ForcePlace(game, Cards.SecurityOfficer, game.Players[1], 0, 1); // Enemy card

            // Loveless is the only card with first played (P) condition with the enhance abiltiy
            AddToHandAndPlaceCard(game, Cards.Loveless, 1, 0);

            Assert.Equal(2, game.Player1Grid[0, 0].GetCumulativePower()); // Allied card enhanced
            Assert.Equal(2, game.Player1Grid[0, 1].GetCumulativePower()); // Enemy card enhanced

            ForcePlace(game, Cards.SecurityOfficer, game.Players[1], 1, 1); // Place enemy card beneath the first enemy card
            // Card shouldn't have bonus power since it wasn't present at the time of Loveless being placed
            Assert.Equal(1, game.Player1Grid[1, 1].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardThatEnfeeblesWhileInPlay()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            ForcePlace(game, Cards.Elphadunk, game.Players[0], 0, 0);
            Assert.Equal(4, game.Player1Grid[0, 0].GetCumulativePower());

            AddToHandAndPlaceCard(game, Cards.ResurrectedAmalgram, 1, 0);
            // Elphadunk's cumulative power should be reduced to 2
            Assert.Equal(2, game.Player1Grid[0, 0].GetCumulativePower());

            // Replace the Resurrect Amalgram card
            AddToHandAndPlaceCard(game, Cards.InsectoidChimera, 1, 0);

            // Elphadunk's power should be back to 4
            Assert.Equal(4, game.Player1Grid[0, 0].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardThatEnfeeblesWhenFirstPlayed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            ForcePlace(game, Cards.Elphadunk, game.Players[0], 0, 0);

            AddToHandAndPlaceCard(game, Cards.Capparwire, 1, 0);
            // The Capparwire's ability should have affected the Elphadunk
            Assert.Equal(3, game.Player1Grid[0, 0].GetCumulativePower());

            ForcePlace(game, Cards.SecurityOfficer, game.Players[0], 2, 0);
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
            game.Player1Grid[0, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.Ifrit, 0, 0);

            // Place security officer in middle of first column
            ForcePlace(game, Cards.SecurityOfficer, game.Players[0], 1, 0);

            // Place crystalline crab at bottom of first column, enhancing security officer
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0);

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

            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0);
            // Security officer should be enhanced 
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 0);
            Assert.Equal(3, game.Player1Grid[1, 0].GetCumulativePower());

            game.Player1Grid[0, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.Ifrit, 0, 0);
            // Ifrit should enhance its own power by 2
            Assert.Equal(2, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnfeebledAsFirstCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            game.Player1Grid[0, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.ShadowBloodQueen, 0, 0);
            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            AddToHandAndPlaceCard(game, Cards.GrasslandsWolf, 1, 0);
            // Place Capparwire to enfeeble Grasslands Wolf
            AddToHandAndPlaceCard(game, Cards.Capparwire, 2, 0);
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

            AddToHandAndPlaceCard(game, Cards.GrasslandsWolf, 1, 0);

            // Place Capparwire to enfeeble Grasslands Wolf
            AddToHandAndPlaceCard(game, Cards.Capparwire, 2, 0);
            Assert.Equal(-1, game.Player1Grid[1, 0].CardBonusPower);

            // Shadowblood Queen should have enhanced its own power by 3 due to the enfeeble
            game.Player1Grid[0, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.ShadowBloodQueen, 0, 0);
            Assert.Equal(3, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void CheckCardWithAEConditionIsWorkingCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            game.Player1Grid[0, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.Dio, 0, 0);
            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            // Place security officers for both players
            ForcePlace(game, Cards.SecurityOfficer, game.Players[1], 0, 4);
            ForcePlace(game, Cards.SecurityOfficer, game.Players[0], 1, 0);

            // Enhance player 2's (enemy) security officer using a Crystalline Crab
            game.SwapPlayerTurns();
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 1, 0);
            Assert.Equal(2, game.Player2Grid[0, 0].TileBonusPower);

            // Dio should've received its own power by 1
            Assert.Equal(1, game.Player1Grid[0, 0].SelfBonusPower);

            // Enhancing Player 1's (ally) security officer using a Crystalline Crab
            game.SwapPlayerTurns();
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0);
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

            ForcePlace(game, Cards.SecurityOfficer, game.Players[0], 0, 0);
            AddToHandAndPlaceCard(game, Cards.SandhogPie, 1, 0);

            // Destroy the Sandhog Pie using a replace card (Insectoid Chimera)
            AddToHandAndPlaceCard(game, Cards.InsectoidChimera, 1, 0);

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
            ForcePlace(game, Cards.SecurityOfficer, game.Players[0], 1, 1);
            Assert.Equal("Security Officer", game.Player1Grid[1, 1].Card!.Name);

            // Place the Elena card so that it targets the Security Officer
            AddToHandAndPlaceCard(game, Cards.Elena, 1, 0);

            // Place Crystalline Crab to enhance Elena
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0);

            // Security Officer card is gone
            Assert.Null(game.Player1Grid[1, 1].Card);

            // The "CardBonusPower" of the Tile (1, 1) should be reset to 0 since the Security Officer card was destroyed
            Assert.Equal(0, game.Player1Grid[1, 1].CardBonusPower);
        }

        [Fact]
        public void CheckWhenFirstEnfeebledAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place an enemy Archdragon in 1, 1 of Player1Grid
            ForcePlace(game, Cards.Archdragon, game.Players[1], 1, 1);

            // Place Reapertail card
            AddToHandAndPlaceCard(game, Cards.Reapertail, 2, 0);

            // Enfeeble the Reapertail using a Capparwire
            AddToHandAndPlaceCard(game, Cards.Capparwire, 1, 0);

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
            ForcePlace(game, Cards.SecurityOfficer, game.Players[0], 0, 0);

            game.Player1Grid[1, 0].RankUp(1);
            AddToHandAndPlaceCard(game, Cards.TonberryKing, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

            // Place a Gigantoad to replace (and destroy) the allied Security Officer
            AddToHandAndPlaceCard(game, Cards.Gigantoad, 0, 0);

            // Tonberry King's power should raise by 2
            Assert.Equal(2, game.Player1Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardWithEDTriggerCondition()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place an enemy Security Officer
            ForcePlace(game, Cards.SecurityOfficer, game.Players[1], 0, 4);

            AddToHandAndPlaceCard(game, Cards.DeathClaw, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

            // Place a Gigantoad to replace (and destroy) the enemy Security Officer, enhancing Deathclaw
            game.SwapPlayerTurns();
            AddToHandAndPlaceCard(game, Cards.Gigantoad, 0, 0);

            Assert.Equal(1, game.Player1Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardWithAEDTriggerCondition()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Player 1: Place Joker
            game.Player1Grid[1, 0].RankUp(1);
            AddToHandAndPlaceCard(game, Cards.Joker, 1, 0);

            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            // Player 1: Place Security Officer
            ForcePlace(game, Cards.SecurityOfficer, game.Players[0], 2, 4);

            // Player 2: Place Security Officer
            ForcePlace(game, Cards.SecurityOfficer, game.Players[1], 0, 4);

            // Player 2: Place Capparwire to enfeeble/destroy both the Security Officers
            game.SwapPlayerTurns();
            AddToHandAndPlaceCard(game, Cards.Capparwire, 1, 0);

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

            game.Player1Grid[0, 0].RankUp(1);
            AddToHandAndPlaceCard(game, Cards.MoogleTrio, 0, 0);

            // Confirm Player 1 has Moogle Mage & Moogle Bard added to hand
            var moogleMage = _cards[(int) Cards.MoogleMage];
            var moogleBard = _cards[(int) Cards.MoogleBard];

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
            AddToHandAndPlaceCard(game, Cards.Shiva, 0, 0);

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

            // After placing Tifa, ensure bonus points are added
            AddToHandAndPlaceCard(game, Cards.Tifa, 0, 0);
            Assert.Equal(5, game.Players[0].Scores[0].winBonus);
        }

        [Fact]
        public void VerifyBonusPointsRemovedWhenCardDestroyed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);
            AddToHandAndPlaceCard(game, Cards.Tifa, 0, 0);

            // Destroy Tifa card and ensure bonus points are removed
            AddToHandAndPlaceCard(game, Cards.InsectoidChimera, 0, 0);
            Assert.Equal(0, game.Players[0].Scores[0].winBonus);
        }

        [Fact]
        public void TestUltimatePartyAnimalAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.UltimatePartyAnimal, 0, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 1);
            Assert.Equal(2, game.GetPlayerLaneScore(0, 0));

            game.SwapPlayerTurns();
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);

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
            ForcePlace(game, Cards.SecurityOfficer, game.Players[1], 0, 0);
            ForcePlace(game, Cards.SecurityOfficer, game.Players[1], 2, 0);

            // Place down Sephiroth card
            game.Player1Grid[1, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.Sephiroth, 1, 0);

            // Security Officers should be destroyed
            Assert.Null(game.Player1Grid[0, 0].Card);
            Assert.Null(game.Player1Grid[2, 0].Card);
        }

        [Fact]
        public void TestAbilityTriggerOnceCardReachesPowerThreshold()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            game.Player1Grid[1, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.Cloud, 1, 0);
            Assert.Equal(3, game.Player1Grid[1, 0].GetCumulativePower());

            // Enhance Cloud (5)
            AddToHandAndPlaceCard(game, Cards.Spearhawk, 0, 0);
            Assert.Equal(1, game.Player1Grid[0, 0].GetCumulativePower());

            // Enhance Cloud (7)
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0);

            // Cloud's power should be raised to 7, and it will boost the power of adjacent cards by 2
            Assert.Equal(7, game.Player1Grid[1, 0].GetCumulativePower());
            Assert.Equal(3, game.Player1Grid[0, 0].GetCumulativePower());
            Assert.Equal(3, game.Player1Grid[2, 0].GetCumulativePower());
        }

        [Fact]
        public void TestTwoFaceEnhanced()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            game.Player1Grid[0, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.TwoFace, 0, 0);

            // Enhance Two Face to trigger its ability when enhanced (+P)
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 1, 0);

            // Two Face should have enhanced the card's power by 4
            Assert.Equal(4, game.Player1Grid[1, 0].CardBonusPower);
        }

        [Fact]
        public void TestTwoFaceEnfeebled()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            game.Player1Grid[0, 0].RankUp(2);
            AddToHandAndPlaceCard(game, Cards.TwoFace, 0, 0);

            // Capparwire enfeeble Two Face to trigger its -P ability
            AddToHandAndPlaceCard(game, Cards.Capparwire, 1, 0);

            // Two Face should have enfeebled capparwire, destroying it
            Assert.Null(game.Player1Grid[1, 0].Card);
        }

        [Fact]
        public void TestCactuarEnhance()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.Cactuar, 0, 0);
            Assert.Equal(3, game.Player1Grid[2, 1].GetCumulativePower());
        }

        [Fact]
        public void TestReplaceEnhancedCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 0);
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0);

            Assert.Equal(0, game.Player1Grid[0, 0].GetCumulativePower());
            AddToHandAndPlaceCard(game, Cards.InsectoidChimera, 1, 0);
            Assert.Equal(0, game.Player1Grid[0, 0].GetCumulativePower());
        }

        [Fact]
        public void TrackEnhancedCardsCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.Cactuar, 0, 0); // Enhance Security Officer
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 1, 0); // Enhance Cactuar
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0); // Enhance previously placed Crystalline Crab
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 2, 1);
            AddToHandAndPlaceCard(game, Cards.ChocoboMoogle, 1, 1);

            Assert.Equal(3, game.EnhancedCards.Count);
        }

        [Fact]
        public void ChocoboMoogleEnhancesCorrectlyWhenPlacedBeforeEnhancements()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.Cactuar, 0, 0); // Enhance Grasslands Wolf
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 0);
            AddToHandAndPlaceCard(game, Cards.ChocoboMoogle, 1, 1);
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0); // Enhance Security Officer
            AddToHandAndPlaceCard(game, Cards.GrasslandsWolf, 2, 1);

            Assert.Equal(2, game.EnhancedCards.Count);
            Assert.Equal(2, game.Player1Grid[1, 1].SelfBonusPower);
        }
    }
}
