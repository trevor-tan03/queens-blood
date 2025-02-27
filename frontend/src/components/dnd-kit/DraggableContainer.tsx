interface Props {
  offsetX: number;
  offsetY: number;
  rotate: number;
  children?: React.ReactNode;
}

const DraggableContainer = ({ offsetX, offsetY, rotate, children }: Props) => {
  return (
    <div
      className="origin-center absolute w-[140px] h-full"
      style={{
        transform: `translate(${offsetX}px, ${offsetY}px) rotateZ(${rotate}deg)`,
      }}
    >
      {children}
    </div>
  );
};

export default DraggableContainer;
