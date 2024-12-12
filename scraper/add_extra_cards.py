import sqlite3

"""
Extra cards refer to the cards which aren't 'base' cards.
This includes cards that are added to your hand or spawned via an ability.
"""

STANDARD = "Standard"
LEGENDARY = "Legendary"

"""Added to your hand"""
added_cards = [
  # Name, Rank, Power, Rarity, Ability
  ["Mandragora Minion", 1, 1, STANDARD, "This card has no abilities."],
  ["Moogle", 2, 2, LEGENDARY, "Raise the power of allied cards on affected tiles by 3 while this card is in play."],
  ["Moogle Mage", 1, 1, LEGENDARY, "When played, lower the power of enemy cards on affected tiles by 4."],
  ["Moogle Trio", 1, 1, LEGENDARY, "Raise the power of allied cards on affected tiles by 2 while this card is in play."],
  ["Donberry", 2, 2, STANDARD, "When played, destroy enemy cards on affected tiles."],
  ["Grangalan Junior", 2, 2, STANDARD, "When played add Baby Grangalan to your hand."],
  ["Baby Grangalan", 1, 1, STANDARD, "When destroyed, lower the power of allied and enemy cards on affected tiles by 5."],
  ["The Tiny Bronco", 2, 2, LEGENDARY, "When played, raise position ranks by 2."],
  ["Galian Beast", 2, 4, LEGENDARY, "When played, lower the power of allied and enemy cards on affected tiles by 1."],
  ["Heatseeker Minion", 1, 1, STANDARD, "This card has no abilities."],
  ["Resurrected Amalgam", 1, 2, STANDARD, "Lower the power of allied and enemy cards on affected tiles by 2 while this card is in play."],
]

"""Spawned into the board"""
spawned_cards = [
  # Name, Rank, Power, Rarity, Ability
  ["Elemental", 1, 1, LEGENDARY, "When destroyed, raise the power of allied and enemy cards on affected tiles by 1."],
  ["Elemental", 2, 1, LEGENDARY, "When destroyed, raise the power of allied and enemy cards on affected tiles by 2."],
  ["Elemental", 3, 1, LEGENDARY, "When destroyed, raise the power of allied and enemy cards on affected tiles by 3."],
  ["Hype Johnny", 1, 1, LEGENDARY, "Raise the power of allied and enemy cards on affected tiles by 1 while this card is in play."],
  ["Hype Johnny", 2, 1, LEGENDARY, "Raise the power of allied and enemy cards on affected tiles by 2 while this card is in play."],
  ["Hype Johnny", 3, 1, LEGENDARY, "Raise the power of allied and enemy cards on affected tiles by 4 while this card is in play."],
  ["Diamond Dust", 1, 2, STANDARD, "This card has no abilities."],
  ["Diamond Dust", 2, 4, STANDARD, "This card has no abilities."],
  ["Diamond Dust", 3, 6, STANDARD, "This card has no abilities."],
]

conn = sqlite3.connect('QB_card_info.db')
cursor = conn.cursor()

for added_card in added_cards:
  name, rank, power, rarity, ability = added_card
  image = f'player-{name.replace(" ", "-").lower()}.webp'

  cursor.execute('''
    INSERT INTO Cards (Name,Rank,Power,Rarity,Ability,Image) VALUES (?,?,?,?,?,?)
  ''', (name,rank,power,rarity,ability,image))

for spawned_card in spawned_cards:
  name, rank, power, rarity, ability = spawned_card
  image = f'player-{name.replace(" ", "-").lower()}-{rank}.webp'

  cursor.execute('''
    INSERT INTO Cards (Name,Rank,Power,Rarity,Ability,Image) VALUES (?,?,?,?,?,?)
  ''', (name,rank,power,rarity,ability,image))

conn.commit()
conn.close()