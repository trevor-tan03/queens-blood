namespace backend.DTO
{
    public struct MessageDTO
    {
        public string playerId;
        public string message;

        public MessageDTO(string _playerId, string _message)
        {
            playerId = _playerId;
            message = _message;
        }
    }
}
