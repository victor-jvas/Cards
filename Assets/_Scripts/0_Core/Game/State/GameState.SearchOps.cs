
    using System.Linq;

    public sealed partial class GameState
    {
        public bool TryFindCard(int ownerPlayerId, int cardInstanceId, out CardInstance card, out ZoneId zoneId)
        {
            foreach (var kvp in Zones)
            {
                var zid = kvp.Key;
                if (zid.PlayerId != ownerPlayerId) continue;

                var zone = kvp.Value;
                var found = zone.Cards.FirstOrDefault(c => c.Id == cardInstanceId);
                if (found != null)
                {
                    card = found;
                    zoneId = zid;
                    return true;
                }
            }

            card = null;
            zoneId = default;
            return false;
        }
    }
