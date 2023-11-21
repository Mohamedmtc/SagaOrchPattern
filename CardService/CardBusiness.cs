namespace CardService
{
    public class CardBusiness: ICardBusiness
    {
        private readonly ILogger<CardBusiness> _logger;

        public CardBusiness(ILogger<CardBusiness> logger)
        {
            _logger = logger;
        }
        public void GetCard()
        {
            _logger.LogInformation("Getting card details");

        }
    }
}
