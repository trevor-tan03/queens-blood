import Background from "../components/Home/Background";
import HomeMenu from "../components/Home/Menu";

const Home = () => {
  return (
    <div
      className="flex items-center justify-center h-dvh w-full relative"
    >
      <HomeMenu />
      <Background />
    </div>
  )
}

export default Home