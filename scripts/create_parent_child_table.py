import sqlite3

def create_parent_child_table():
    """Create a relationship between the parent and child card"""
    child_card_names = [
        "Mandragora Minion",
        "Moogle",
        "Moogle Mage",
        "Moogle Bard",
        "Donberry",
        "Grangalan Junior",
        "Baby Grangalan",
        "The Tiny Bronco",
        "Galian Beast",
        "Heatseeker Minion",
        "Resurrected Amalgam",
        "Cacneo",
        "Elemental",
        "Hype Johnny",
        "Diamond Dust"
    ]

    conn = sqlite3.connect('QB_card_info.db')
    cursor = conn.cursor()

    try:
        # Create the ParentChild table
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS ParentChild (
                ParentCardID INT,
                ChildCardID INT
            )
        ''')

        # Get the child cards
        cards_placeholder = ', '.join(['?'] * len(child_card_names))
        get_child_cards_query = f'SELECT ID, Name FROM Cards WHERE Name IN ({cards_placeholder})'
        cursor.execute(get_child_cards_query, child_card_names)
        child_cards = cursor.fetchall()

        # Get the parent cards
        cursor.execute('SELECT ID, Ability FROM Cards WHERE `Action`="add" OR `Action`="spawn";')
        parent_cards = cursor.fetchall()

        parent_child_pairs = []

        for child_id, child_name in child_cards:
            for parent_id, ability in parent_cards:
                if child_name == "Moogle" and int(parent_id) != 92:
                    continue

                if child_name in ability:
                    parent_child_pairs.append((parent_id, child_id))

        # Populate the ParentChild table
        for parent_id, child_id in parent_child_pairs:
            insert_parent_child_query = f'INSERT INTO ParentChild (ParentCardID, ChildCardID) VALUES ("{parent_id}", "{child_id}")'
            cursor.execute(insert_parent_child_query)

    except Exception as e:
        print(f"An error occurred: {e}")
    
    finally:
        conn.commit()
        conn.close()