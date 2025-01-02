const CardCopiesText = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="text-center sm:w-20 text-orange-300 border-orange-300 sm:border rounded-full mx-auto mt-1 w-auto">
      {children}
    </div>
  )
}

export default CardCopiesText