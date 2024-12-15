using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controller
{
	[Route("api/cards")]
	[ApiController]
	public class CardController : ControllerBase
	{
		private readonly ICardRepository _cardRepository;

		public CardController (ICardRepository cardRepository)
		{
			_cardRepository = cardRepository;
		}

		[HttpGet("base")]
		public IActionResult GetBaseCards()
		{
			return Ok(_cardRepository.GetBaseCards());
		}
	}
}
