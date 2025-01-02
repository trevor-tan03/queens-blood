
const GameBackground = () => {
  return (
    <div className="w-full h-full fixed top-0 left-0 z-[-1] overflow-hidden">
      <div className="absolute bg-black w-full h-full opacity-50"></div>
      <img
        src="../../../assets/backgrounds/game_background.webp"
        alt="Background image"
        className="object-cover h-full w-full object-top blur-2xl scale-110"
      />
    </div>
  )
}

export default GameBackground