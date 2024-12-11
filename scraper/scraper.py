import requests
import sqlite3
from bs4 import BeautifulSoup

response = requests.get("https://game8.co/games/Final-Fantasy-VII-Rebirth/archives/Queens-Blood")

if response.status_code == 200:
    soup = BeautifulSoup(response.content, 'html.parser')
    div = soup.find("div", class_='scroll--table table-header--fixed table-left--fixed')

    conn = sqlite3.connect('QB_card_info.db')
    cursor = conn.cursor()
    cursor.execute('''
                CREATE TABLE IF NOT EXISTS Cards (
                    ID INTEGER PRIMARY KEY,
                    Name TEXT,
                    Rank TEXT,
                    Power TEXT,
                    Rarity TEXT,
                    Ability TEXT,
                   Image TEXT
                )
            ''')
    trs = div.find_all("tr")
    for tr in trs[1::]:
        tds = tr.find_all("td")
        name = tds[0].getText().strip("\n")
        rank = tds[1].getText().strip("\n")
        power = tds[2].getText().strip("\n")
        ability = tds[3].getText().strip("\n")
        Id = tds[5].getText().strip("\n")
        rarity = tds[6].getText().strip("\n")

        image = f'player-{name.replace(" ", "-").lower()}.webp'

        cursor.execute('''
            INSERT INTO Cards (ID, Name,Rank,Power,Rarity,Ability,Image) VALUES (?,?,?,?,?,?,?)
        ''', (int(Id),name,rank,power,rarity,ability,image))
    conn.commit()
    conn.close()



