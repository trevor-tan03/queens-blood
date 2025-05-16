<!-- Banner here -->
<div align="center">
  <h1>Queen's Blood Clone</h1>
  <p>A web-based recreation of the Queen's Blood minigame from <a href="https://ffvii.square-enix-games.com/en-us/games/rebirth">Final Fantasy VII: Rebirth</a></p>
</div>

<div div align="center">
  <b>DISCLAIMER</b>: I am using the free tier of Azure's app service which sleeps after a period of inactivity. It'll take roughly 1-2 minutes for it to be ready.
</div>

<br>

<p align="center">
  Play <a href="https://mango-bush-070d28b00.6.azurestaticapps.net">Here</a>
</p>

<div align="center">
  <img alt="Static Badge" src="https://img.shields.io/badge/.NET-8.0-purple">
  <img alt="Static Badge" src="https://img.shields.io/badge/React-18.3.1-blue">
  <img alt="Static Badge" src="https://img.shields.io/badge/SignalR-2.4.3-cyan">
</div>

<br>

<p align="center">
    <a href="#getting-started">Getting Started</a> •
    <a href="#how-to-play">How To Play</a> •
    <a href="credits">Credits</a>
</p>

![image](https://github.com/user-attachments/assets/f2fa0b1e-22b9-4f36-b816-c0a7f6e9fa6c)


## Getting Started

To clone and run this application, you'll need [Git](https://git-scm.com/), [Node.js 22 LTS](https://nodejs.org/en), and [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed on your computer. From your command line:
```bash
# Clone the repository
$ git clone https://github.com/trevor-tan03/queens-blood.git

# Go into the repository
$ cd queens-blood

# Go into the frontend directory
$ cd frontend

# Install dependencies
$ npm install

# Create a .env file using the contents in .env.example
$ cp .env.example .env

# Run the frontend
$ npm run dev

# Run the backend (from a separate terminal)
$ cd queens-blood/backend
$ dotnet run
```

## How To Play

1. Create or join a game
    - Requires two players to start a game
2. Mulligan phase
    - 30sec to select the cards you wish to redraw
3. Starting player is selected randomly
4. Player with the turn can play a card by dragging and dropping it onto the play area or skip turn
     - Cards can only place cards on tiles owned (marked in green)
5. If both players skip turns consecutively, the game ends and the player with most points wins
     - Points are awarded ONLY if the player has more points than the opponent in that lane.
     - Neither players will receive points if it's a tie

## Credits
- Square Enix (creators of Final Fantasy VII)
- [Queen's Blood Online](https://www.queensbloodonline.com/) (another fan's recreation of the minigame)
- List of Queen's Blood cards from  [Game8](https://game8.co/games/Final-Fantasy-VII-Rebirth/archives/Queens-Blood)
- My friend [Frank](https://github.com/frankpadada) helping with retrieving data of the cards
