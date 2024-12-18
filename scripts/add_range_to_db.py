import os
import json
import sqlite3

def get_important_tiles(grid):
  """
  Grabs the orange/red tiles in the grid and returns the offset relative to the center white tile
  """
  important_tiles = []

  for i in range(5):
    for j in range(5):
      if grid[i][j] in ["O", "R", "OR"]:
        important_tiles.append({
          "colour": grid[i][j],
          "offset": (j-2,i-2)
        })

  return important_tiles

def add_range_to_db():
  curr_dir = os.path.dirname(__file__)
  json_path = os.path.join(curr_dir, "expected_ranges.json")

  with open(json_path, "r") as openfile:
    entries = json.load(openfile)

  conn = sqlite3.connect('QB_card_info.db')
  cursor = conn.cursor()

  cursor.execute("""
    CREATE TABLE IF NOT EXISTS Ranges (
                 ID INTEGER PRIMARY KEY,
                 CardID INTEGER,
                 Offset TEXT,
                 Colour TEXT,
                 FOREIGN KEY("CardID") REFERENCES Card("ID")
    )
  """)

  for (id, processed_name) in enumerate(entries.keys()):
    grid = entries[processed_name]["grid"]
    important_tiles = get_important_tiles(grid)

    for important_tile in important_tiles:
      colour = important_tile["colour"]
      offset = important_tile["offset"]

      cursor.execute("INSERT INTO Ranges (CardID, Offset, Colour) VALUES (?,?,?)", (id+1,f"{offset[0],offset[1]}",colour))

  conn.commit()
  conn.close()

if __name__ == "__main__":
  add_range_to_db()