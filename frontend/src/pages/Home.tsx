import { useState } from "react";
import Background from "../components/Home/Background";
import JoinModal from "../components/Home/JoinModal";
import HomeMenu from "../components/Home/Menu";
import { useOverlay } from "../components/Overlay";

const Home = () => {
  const { show } = useOverlay();
  const [name, setName] = useState('');

  return (
    <div>
      <div
        className="flex items-center justify-center h-dvh w-full relative"
      >
        {show && <JoinModal name={name} />}
        <HomeMenu name={name} setName={setName} />
        <Background />
      </div>
    </div>
  )
}

export default Home