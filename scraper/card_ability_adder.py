import sqlite3

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

    for column in columns_to_add:
        try:
            cursor.execute(column)
        except sqlite3.OperationalError as e:
            print(f"Column: {e} already exists")



    query = "SELECT id, Ability FROM Cards;"
    cursor.execute(query)


    data = cursor.fetchone()



    cursor.close()
    connection.close()


