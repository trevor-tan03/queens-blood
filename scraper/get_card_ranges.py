from PIL import Image
import sqlite3
import os

"""
Grid area is roughly 120x120 pixels

Each square in the grid is approximately 20x20 pixels
- Center point = (10,10)
- Center point of red border = (5,10)

Each gap between squares is approximately
- (120 - 100) / 4 = 5 pixels
"""

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
def crop_image(card_path):
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

cards = cursor.execute("SELECT ID, Name, Rank FROM cards")
count = 0

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

  cropped_image = crop_image(filepath)
  _5x5 = get_card_range(cropped_image)

  # print(name)
  # for row in _5x5:
  #   print(row)
  # print()

conn.close()