import GameBackground from "../components/Game/GameBackground";
import DeckHeader from "../components/Lobby/DeckHeader";
import PlayersSection from "../components/Lobby/PlayersSection";
import SelectedDeck from "../components/Lobby/SelectedDeck";

const Lobby = () => {
  return (
    <div className="flex gap-4 p-6 h-dvh overflow-y-visible md:overflow-y-hidden md:grid-cols-1 flex-col-reverse md:flex-row">
      <div className="overflow-y-hidden">
        <DeckHeader />
        <SelectedDeck />
      </div>
      <PlayersSection />
      <GameBackground />
    </div>
  );
};

export default Lobby;
