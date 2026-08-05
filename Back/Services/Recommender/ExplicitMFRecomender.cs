using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Recommender
{
    public record InteractionRow(int U, int I, float R, float W);

    public class IdIndexer
    {
        private readonly Dictionary<int, int> _toIdx = new();
        private readonly List<int> _fromIdx = new();
        public int Count => _fromIdx.Count;

        public int GetOrAdd(int rawId)
        {
            if (_toIdx.TryGetValue(rawId, out var idx)) return idx;
            idx = _fromIdx.Count;
            _toIdx[rawId] = idx;
            _fromIdx.Add(rawId);
            return idx;
        }
        public bool TryGet(int rawId, out int idx) => _toIdx.TryGetValue(rawId, out idx);
        public int this[int idx] => _fromIdx[idx];
        public IEnumerable<int> RawIds => _fromIdx;
    }

    public class MatrixFactorization
    {
        readonly int _k;
        readonly float _lambda;
        readonly float _eta0;
        readonly Random _rng = new(42);

        public float[][] P = default!; // users x k
        public float[][] Q = default!; // items x k

        public MatrixFactorization(int numUsers, int numItems, int k = 64, float lambda = 0.05f, float eta0 = 0.05f)
        {
            _k = k; _lambda = lambda; _eta0 = eta0;
            P = Init(numUsers, k, seed: 1);
            Q = Init(numItems, k, seed: 2);
        }

        static float[][] Init(int n, int k, int seed)
        {
            var r = new Random(seed);
            var x = new float[n][];
            for (int i = 0; i < n; i++)
            {
                x[i] = new float[k];
                for (int f = 0; f < k; f++) x[i][f] = (float)((r.NextDouble() - 0.5) * 0.02);
            }
            return x;
        }

        void SgdStep(int u, int i, float t, float w, float eta)
        {
            var pu = P[u]; var qi = Q[i];

            // prediction
            float y = 0f;
            for (int f = 0; f < _k; f++) y += pu[f] * qi[f];
            float e = t - y;

            // updates
            for (int f = 0; f < _k; f++)
            {
                float puf = pu[f], qif = qi[f];
                float dpu = w * e * qif - _lambda * puf;
                float dqi = w * e * puf - _lambda * qif;
                pu[f] += eta * dpu;
                qi[f] += eta * dqi;
            }
        }

        public void Fit(IEnumerable<InteractionRow> train, int epochs = 10)
        {
            var data = train.ToArray();
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                float eta = _eta0 / (1f + 0.1f * epoch);
                // shuffle
                for (int n = data.Length - 1; n > 0; n--)
                {
                    int j = _rng.Next(n + 1);
                    (data[n], data[j]) = (data[j], data[n]);
                }
                foreach (var row in data)
                    SgdStep(row.U, row.I, row.R, row.W, eta);
            }
        }

        public float Score(int u, int i)
        {
            float s = 0f;
            var pu = P[u]; var qi = Q[i];
            for (int f = 0; f < _k; f++) s += pu[f] * qi[f];
            return s;
        }
    }


    public class MfTrainingDataBuilder
    {
        private readonly AppDbContext _db;
        public MfTrainingDataBuilder(AppDbContext db) { _db = db; }

        public async Task<(List<InteractionRow> rows, IdIndexer userIdx, IdIndexer itemIdx, HashSet<(int u, int i)> seen)> BuildAsync(
            DateTime since,
            CancellationToken ct = default)
        {
            var users = await _db.Users.Select(u => u.UserId) .ToListAsync(ct); 
            var items = await _db.Items.Where(i => i.IsActive) .Select(i => i.ItemId).ToListAsync(ct); 

            var uIndex = new IdIndexer();
            var iIndex = new IdIndexer();
            foreach (var uid in users) uIndex.GetOrAdd(uid);
            foreach (var iid in items) iIndex.GetOrAdd(iid);

            var clicks = await _db.ClickTracks.Where(c => c.UpdatedAt >= since).Select(c => new { c.UserId, c.ItemId, c.Clicks }).ToListAsync(ct);

            var orders = await _db.Orders.Where(o => o.DateUpdated >= since).Select(o => new { o.BuyerId, o.ItemId, o.BuyNow, o.AuctionId }).ToListAsync(ct);

            // (u,i) -> (r,w)
            // r: click=1, auction=3, buyNow=4 (take max)
            // w: 1 + clickWeight + auctionWeight + buyNowWeight
            //    clickWeight = min(1 + 0.25 * clicks, 5)
            //    auctionWeight = 4 if any auction win
            //    buyNowWeight = 6 if any buy-now
            var agg = new Dictionary<(int U, int I), (float r, float w)>();

               foreach (var c in clicks)
            {
                if (!uIndex.TryGet(c.UserId, out var u)) continue;
                if (!iIndex.TryGet(c.ItemId, out var i)) continue;
                var key = (u, i);

                float rClick = 1f;
                float wClick = MathF.Min(1f + 0.25f * (float)c.Clicks, 5f); // scale

                if (!agg.TryGetValue(key, out var cur))
                    cur = (rClick, 1f); // base 

                cur.r = MathF.Max(cur.r, rClick);
                cur.w += wClick;

                agg[key] = cur;
            }

            foreach (var o in orders)
            {
                if (!uIndex.TryGet(o.BuyerId, out var u)) continue;
                if (!iIndex.TryGet(o.ItemId, out var i)) continue;
                var key = (u, i);

                bool isBuyNow = o.BuyNow == true;                               
                bool isAuctionWin = !isBuyNow && o.AuctionId != null;           

                float rEvt = isBuyNow ? 4f : (isAuctionWin ? 3f : 0f);
                float wEvt = isBuyNow ? 6f : (isAuctionWin ? 4f : 0f);
                if (rEvt == 0f && wEvt == 0f) continue; // ignore garbage 

                if (!agg.TryGetValue(key, out var cur))
                    cur = (rEvt, 1f); // base 

                cur.r = MathF.Max(cur.r, rEvt);
                cur.w += wEvt;

                agg[key] = cur;
            }
            var rows = new List<InteractionRow>(agg.Count);
            foreach (var kv in agg)
            {
                var (u, i) = kv.Key;
                var (r, w) = kv.Value;
                w = MathF.Max(w, 1f);
                rows.Add(new InteractionRow(u, i, r, w));
            }
            var seen = new HashSet<(int u, int i)>(rows.Select(r => (r.U, r.I)));

            return (rows, uIndex, iIndex, seen);
        }
    }

    public class ExplicitMfRecommenderService
    {
        private readonly AppDbContext _db;
        private MatrixFactorization? _mf;
        private IdIndexer? _uIndex;
        private IdIndexer? _iIndex;
        private HashSet<(int u, int i)>? _seen;

        public ExplicitMfRecommenderService(AppDbContext db) { _db = db; }

        public async Task TrainAsync(DateTime since, CancellationToken ct = default)
        {
            var builder = new MfTrainingDataBuilder(_db);
            var (rows, uIndex, iIndex, seen) = await builder.BuildAsync(since, ct);

            // If no rows yet, initialize empty model so Recommend still works
            _uIndex = uIndex; _iIndex = iIndex; _seen = seen;
            _mf = new MatrixFactorization(uIndex.Count, iIndex.Count, k: 64, lambda: 0.05f, eta0: 0.05f);

            if (rows.Count > 0)
                _mf.Fit(rows, epochs: 10);
        }

        public async Task TrainAndPersistAsync(DateTime since, int topN = 20, CancellationToken ct = default)
        {
            await TrainAsync(since, ct);
            if (_mf == null || _uIndex == null || _iIndex == null) return;

            var now = DateTime.UtcNow;
            var modelTag = $"mf:k=64:lambda=0.05:eta0=0.05:since={since:yyyyMMdd}";

            var allUserIds = _uIndex.RawIds.ToArray();

            foreach (var rawUserId in allUserIds)
            {
                var recs = await RecommendForUserInternal(rawUserId, topN, ct);

                var old = _db.Set<UserRecommendation>().Where(x => x.UserId == rawUserId);
                _db.RemoveRange(old);

                int rank = 1;
                foreach (var (itemId, score) in recs)
                {
                    _db.Add(new UserRecommendation
                    {
                        UserId = rawUserId,
                        ItemId = itemId,
                        Score = score,
                        Rank = rank++,
                        GeneratedAt = now,
                        ModelTag = modelTag
                    });
                }

                await _db.SaveChangesAsync(ct);
            }
        }

        private async Task<List<(int itemId, float score)>> RecommendForUserInternal(int rawUserId, int topN, CancellationToken ct)
        {
            if (_mf == null || _uIndex == null || _iIndex == null)
            {
                await TrainAsync(DateTime.UtcNow.AddDays(-180), ct);
            }

            if (!_uIndex!.TryGet(rawUserId, out var u))
            {
                // cold user fallback → general popular
                var popularItemIds = await _db.Orders
                    .GroupBy(o => o.ItemId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .Take(topN)
                    .ToListAsync(ct);
                return popularItemIds.Select(id => (id, 0f)).ToList();
            }

            var scores = new List<(int idx, float s)>(_iIndex!.Count);
            for (int i = 0; i < _iIndex.Count; i++)
            {
                if (_seen!.Contains((u, i))) continue;
                scores.Add((i, _mf!.Score(u, i)));
            }

            var top = scores
                .OrderByDescending(t => t.s)
                .Take(topN)
                .Select(t => (_iIndex![t.idx], t.s))
                .ToList();

            if (top.Count < topN)
            {
                var chosen = new HashSet<int>(top.Select(t => t.Item1));
                var seenItems = new HashSet<int>(_seen.Where(p => p.u == u).Select(p => _iIndex![p.i]));
                var pad = await _db.Orders
                    .GroupBy(o => o.ItemId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .Where(id => !chosen.Contains(id) && !seenItems.Contains(id))
                    .Take(topN - top.Count)
                    .ToListAsync(ct);
                top.AddRange(pad.Select(id => (id, 0f)));
            }

            return top;
        }

    }
}
