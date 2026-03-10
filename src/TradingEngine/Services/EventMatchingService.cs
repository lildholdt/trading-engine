// using TradingEngine.Domain;
// using TradingEngine.Utils;
// using TradingEngine.Infrastructure;
// using TradingEngine.Services.OddsApi;
// using TradingEngine.Services.PolyMarket;
// using IOddsApiEventCatalogue = TradingEngine.Services.OddsApi.IOddsApiEventCatalogue;
//
// namespace TradingEngine.Services;
//
// public class EventMatchingService(
//     IPolyMarketEventCatalogue polyMarketEventCatalogue,
//     IOddsApiEventCatalogue oddsApiEventCatalogue,
//     ITeamMatcher teamMatcher,
//     IRepository<MatchedMatch, string> matchedMatchRepository,
//     ILogger<EventMatchingService> logger) : BackgroundService
// {
//
//     protected override async Task ExecuteAsync(CancellationToken cancellationToken)
//     {
//         while (!cancellationToken.IsCancellationRequested)
//         {
//             try
//             {
//                 var sportEvents = polyMarketEventCatalogue.GetAllAsync();
//                 var oddsEvents = await oddsApiEventCatalogue.GetAllAsync(cancellationToken);
//
//                 foreach (var sportEvent in sportEvents)
//                 {
//                     foreach (var oddsEvent in oddsEvents)
//                     {
//                         Console.WriteLine($"Starting matching on matches: SportEventId={sportEvent.Id}, OddsEventId={oddsEvent.Id}");
//                         var score1 = teamMatcher.MatchScore(sportEvent.Team1, oddsEvent.Team1);
//                         var score2 = teamMatcher.MatchScore(sportEvent.Team2, oddsEvent.Team2);
//                         if (score1 > 0.9 && score2 > 0.9) // Threshold can be adjusted
//                         {
//                             var key = $"{sportEvent.Id}:{oddsEvent.Id}";
//                             var matched = new MatchedMatch(key)
//                             {
//                                 SportEventId = sportEvent.Id,
//                                 OddsEventId = oddsEvent.Id,
//                                 Team1 = sportEvent.Team1,
//                                 Team2 = sportEvent.Team2,
//                                 Score1 = score1,
//                                 Score2 = score2
//                             };
//                             // Check if already exists
//                             var existing = await matchedMatchRepository.GetByIdAsync(key, cancellationToken);
//                             if (existing == null)
//                             {
//                                 Console.WriteLine($"New MatchedMatch: SportEventId={matched.SportEventId}, OddsEventId={matched.OddsEventId}, Team1={matched.Team1}, Team2={matched.Team2}, Score1={matched.Score1}, Score2={matched.Score2}");
//                             }
//                             await matchedMatchRepository.SaveAsync(matched, cancellationToken);
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(ex, "Error matching teams");
//             }
//             await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
//         }
//     }
//
//     public async Task<IEnumerable<MatchedMatch>> GetMatchedMatchesAsync(CancellationToken cancellationToken = default)
//         => await matchedMatchRepository.GetAllAsync(cancellationToken);
// }
//
