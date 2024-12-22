const CardCopiesText = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="text-center w-20 text-orange-300 border-orange-300 border rounded-full mx-auto mt-1">
      {children}
    </div>
  )
}

export default CardCopiesText