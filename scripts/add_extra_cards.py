import sqlite3

def add_extra_cards():
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
    ["Moogle Bard", 1, 1, LEGENDARY, "Raise the power of allied cards on affected tiles by 2 while this card is in play."],
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

  # Create a table for the extra cards
  cursor.execute('''
                  CREATE TABLE IF NOT EXISTS 'Extra Cards' (
                      ID INTEGER PRIMARY KEY,
                      Parent INTEGER,
                      Name TEXT,
                      Rank TEXT,
                      Power TEXT,
                      Rarity TEXT,
                      Ability TEXT,
                      Image TEXT,
                      FOREIGN KEY("Parent") REFERENCES Cards("ID")
                  )
              ''')

  for added_card in added_cards:
    name, rank, power, rarity, ability = added_card
    image = f'player-{name.replace(" ", "-").lower()}.webp'

    cursor.execute('''
      INSERT INTO 'Extra Cards' (Name,Rank,Power,Rarity,Ability,Image) VALUES (?,?,?,?,?,?)
    ''', (name,rank,power,rarity,ability,image))

    cursor.execute('''
        INSERT INTO Cards (Name,Rank,Power,Rarity,Ability,Image) VALUES (?,?,?,?,?,?)
      ''', (name, rank, power, rarity, ability, image))

  for spawned_card in spawned_cards:
    name, rank, power, rarity, ability = spawned_card
    image = f'player-{name.replace(" ", "-").lower()}-{rank}.webp'

    cursor.execute('''
      INSERT INTO 'Extra Cards' (Name,Rank,Power,Rarity,Ability,Image) VALUES (?,?,?,?,?,?)
    ''', (name,rank,power,rarity,ability,image))

    cursor.execute('''
      INSERT INTO Cards (Name,Rank,Power,Rarity,Ability,Image) VALUES (?,?,?,?,?,?)
    ''', (name, rank, power, rarity, ability, image))

  cursor.execute(f"UPDATE Cards SET Ability =  'Raise the power of allied cards on affected tiles by 3.' WHERE id = 24;")
  cursor.execute(f"UPDATE Cards SET Ability =  'Raise power by 1 for each other enhanced allied card.' WHERE id = 107;")
  cursor.execute(f"UPDATE Cards SET Ability =  'The first time this card is enhanced, raise the power of allied cards on affected tiles by 4.' WHERE id = 81;")

  conn.commit()
  conn.close()