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
                    Obtain TEXT,
                    Rarity TEXT,
                    Ability TEXT
                )
            ''')
    trs = div.find_all("tr")
    for tr in trs[1::]:
        tds = tr.find_all("td")
        name = tds[0].getText().strip("\n")
        rank = tds[1].getText().strip("\n")
        power = tds[2].getText().strip("\n")
        ability = tds[3].getText().strip("\n")
        obtain = tds[4].getText().strip("\n")
        Id = tds[5].getText().strip("\n")
        rarity = tds[6].getText().strip("\n")
        # print((Id,name,rank,power,obtain,rarity,ability))
        # print(td.getText())
        cursor.execute('''
            INSERT INTO Cards (ID, Name,Rank,Power,Obtain,Rarity,Ability) VALUES (?,?,?,?,?,?,?)
        ''', (int(Id),name,rank,power,obtain,rarity,ability))
    conn.commit()
    conn.close()



