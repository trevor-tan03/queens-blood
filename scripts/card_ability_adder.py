import sqlite3

conditions = [" no ", "when played", "is in play"]
c = ["N", "P", "*"]
abilities = ["destroy", "replace", "raise the p", "lower the p", "raise position r"]
a = ["destroy", "replace", "increaseP", "decreaseP", "increaseR"]
targets = ["allied and enemy", "allied", "enemy"]
t = ['ae', 'a', 'e']



def card_ability_adder():
    connection = sqlite3.connect("QB_card_info.db")
    cursor = connection.cursor()

    columns_to_add = [
        "ALTER TABLE Cards ADD COLUMN 'Condition' TEXT",
        "ALTER TABLE Cards ADD COLUMN 'Action' TEXT",
        "ALTER TABLE Cards ADD COLUMN 'Target' TEXT",
        "ALTER TABLE Cards ADD COLUMN 'Value Source' TEXT",
        "ALTER TABLE Cards ADD COLUMN 'Value' TEXT"
    ]

    for column in columns_to_add:  # adds columns if not exist
        try:
            cursor.execute(column)
        except sqlite3.OperationalError as e:
            print(f"Column: {e} already exists")

    query = "SELECT COUNT(*) FROM Cards;"  # find out the total rows in the table
    cursor.execute(query)
    row_count = cursor.fetchone()[0]
    print(f"Total number of rows in the table: {row_count}")


    query = "SELECT ID, Ability FROM Cards;"
    cursor.execute(query)
    cards = cursor.fetchall()

    # query = "SELECT ID, CardID, Colour From Ranges;"
    # cursor.execute(query)
    # ranges = cursor.fetchall()
    nums = ["1", "2", "3", "4", "5", "6", "7", "8", "9"]
    for card in cards:
        cardId = card[0]
        ability = card[1]
        for condition in conditions:
            if condition in ability:
                i = conditions.index(condition)
                cursor.execute(f"UPDATE Cards SET Condition = '{c[i]}' WHERE id = {cardId};")
        for action in abilities:
            if action in ability:
                i = abilities.index(action)
                cursor.execute(f"UPDATE Cards SET Action = '{a[i]}' WHERE id = {cardId};")
        for target in targets:
            if target in ability:
                i = targets.index(target)
                cursor.execute(f"UPDATE Cards SET Target = '{t[i]}' WHERE id = {cardId};")
        if " by " in ability:
            i = ability.index(" by ")
            value = ability[i+4:i+6]
            for num in nums:
                if num in value:
                    if "." == value[1]:
                        cursor.execute(f"UPDATE Cards SET Value = '{int(num)}' WHERE id = {cardId};")
                    else:
                        cursor.execute(f"UPDATE Cards SET Value = '{int(value)}' WHERE id = {cardId};")




    connection.commit()
    cursor.close()
    connection.close()


# card_ability_adder()