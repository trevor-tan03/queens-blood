import AutoPopulateDeck from "../components/AutoPopulateDeck";
import GameBackground from "../components/Game/GameBackground";
import DeckHeader from "../components/Lobby/DeckHeader";
import PlayersSection from "../components/Lobby/PlayersSection";
import SelectedDeck from "../components/Lobby/SelectedDeck";

const Lobby = () => {
  return (
    <div>
      <AutoPopulateDeck />
      <div className="flex p-6 h-dvh overflow-y-visible flex-row max-w-[1280px] mx-auto *:max-h-[740px] *:h-full">
        <div className="overflow-y-hidden">
          <DeckHeader />
          <SelectedDeck />
        </div>
        <PlayersSection />
      </div>
      <GameBackground />
    </div>
  );
};

export default Lobby;
