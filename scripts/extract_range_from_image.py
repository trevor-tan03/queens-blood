from PIL import Image
import sqlite3
import os
import json

"""
NOTE: This is not a perfect solution, but it gets most of the job done.
      We needed to manually fix up about 13 out of the 165 cards.

Grid area is roughly 120x120 pixels

Each square in the grid is approximately 20x20 pixels
- Center point = (10,10)
- Center point of red border = (5,10)

Each gap between squares is approximately
- (120 - 100) / 4 = 5 pixels
"""

def check_list_equality(l1, l2):
  if len(l1) != 5 or len(l2) != 5:
    return False
  
  for i in range(5):
    for j in range(5):
      if l1[i][j] != l2[i][j]:
        return False
  return True

def get_square_colour(pixel):
  if pixel <= 90:
    return "GRAY"
  elif pixel >= 150 and pixel < 200:
    return "ORANGE"
  elif pixel >= 200:
    return "WHITE"
  else:
    return "RED"

# 1. Crop & Grayscale
def crop_image_grayscale(card_path):
  img = Image.open(card_path)
  img = img.crop((135,313,255,433)).convert('L')
  return img

# 2. Iterate through each of the squares in the grid
def get_card_range(img):
  _5x5_grid = [[None for _ in range(5)] for _ in range(5)]
  i = 0

  for row in range(0,120,25):
    j = 0
    for col in range(0,120,25):
      center = (col+10,row+10)
      border = (col+10,row+3)

      center_pixel = img.getpixel(center)
      border_pixel = img.getpixel(border)

      center_colour = get_square_colour(center_pixel)
      border_colour = get_square_colour(border_pixel)

      if (center_colour == "ORANGE" and border_colour == "RED"):
        _5x5_grid[i][j] = "OR"
      elif center_colour == "ORANGE":
        _5x5_grid[i][j] = "O"
      elif border_colour == "RED":
        _5x5_grid[i][j] = "R"
      elif border_colour == "WHITE":
        _5x5_grid[i][j] = "W"

      j += 1
    i += 1

  return _5x5_grid


curr_dir = os.path.dirname(__file__)
conn = sqlite3.connect('QB_card_info.db')
cursor = conn.cursor()

cards = cursor.execute(f"SELECT ID, Name, Rank FROM cards")
count = 0

dictionary = dict()

with open("sample.json", "r") as openfile:
  json_object = json.load(openfile)

for card in cards:
  id, name, rank = card
  processed_name = name.replace(" &","")\
  .replace(".", "")\
  .replace(" ", "-")\
  .lower()

  spawned_cards = ["Elemental", "Hype Johnny", "Diamond Dust"]
  if name in spawned_cards:
    processed_name += f"-{rank}" 

  filepath = os.path.join(curr_dir, f"..\\frontend\public\\assets\cards\player-{processed_name}.webp")

  cropped_image = crop_image_grayscale(filepath)
  _5x5 = get_card_range(cropped_image)

  json_grid = json_object[processed_name]["grid"]

  dictionary[processed_name] = {
    "name": name,
    "grid": _5x5
  }

# We execute this then manually fix the specified cards
"""
Cards to fix manually:
Two Face
Special Forces Operator
Ramuh
Leviathan
Crimson Mare Mk. II
Gi Nattak
Demon Gate
Moogle Mage
Don Berry
Grangalan Junior
Baby Grangalan
Resurrected Amalgam
Diamond Dust
"""
# json_object = json.dumps(dictionary, indent=2)
# with open("sample.json", "w") as outfile:
#   outfile.write(json_object)

conn.close()

