
using UnityEngine.Events;
using UnityEngine;
namespace IACGGames
{
    [System.Serializable]
    public class NormalScore
    {
        public int Score { get; private set; }

        public event UnityAction<int> OnScoreChanged;

        public NormalScore(int initialScore = 0)
        {
            Score = initialScore;
        }

        public void Add(int amount, bool notify = true)
        {
            Score += amount;
            if (Score < 0)
                Score = 0;

            if (notify)
                OnScoreChanged?.Invoke(Score);
        }

        public void Set(int value, bool notify = true)
        {
            Score = value;
            if (Score < 0)
                Score = 0;

            if (notify)
                OnScoreChanged?.Invoke(Score);
        }
        public void InvokeOnScoreChanged()
        {
            OnScoreChanged?.Invoke(Score);

        }
    }
    [System.Serializable]
    public class MeteredScore
    {
        public NormalScore Normal { get; private set; }

        public int TotalScore => Normal.Score;
        public int MeterTarget => meterTarget;

        public int Meter { get; private set; }
        public int Multiplier { get; private set; }

        private readonly int meterTarget;
        private readonly int basePoints;
        private readonly int maxMultiplier;
        private readonly int minMultiplier;

        // Events for UI
        public event UnityAction<int> OnTotalScoreChanged;
        public event UnityAction<int, int, bool> OnMeterChanged;
        public event UnityAction<int> OnMultiplierChanged;

        public MeteredScore(int basePoints = 50, int meterTarget = 4, int startMultiplier = 1, int maxMultiplier = 10)
        {
            this.basePoints = basePoints;
            this.meterTarget = meterTarget;
            this.maxMultiplier = maxMultiplier;

            Meter = 0;
            minMultiplier = 1;
            Multiplier = startMultiplier;

            Normal = new NormalScore(0);

            OnTotalScoreChanged?.Invoke(TotalScore);
            OnMultiplierChanged?.Invoke(Multiplier);
            OnMeterChanged?.Invoke(Meter, Multiplier, true);
        }

        public int Correct()
        {
            int points = basePoints * Multiplier;
            Normal.Add(points);
            OnTotalScoreChanged?.Invoke(TotalScore);

         
            Meter++;
            OnMeterChanged?.Invoke(Meter, Multiplier, true);

           
            if (Meter >= meterTarget)
            {
                Meter = 0;

                if (Multiplier < maxMultiplier)
                {
                    Multiplier++;
                    OnMultiplierChanged?.Invoke(Multiplier);
                }

                OnMeterChanged?.Invoke(Meter, Multiplier, true);
            }

            return points;
        }

        public void Wrong()
        {
            if (Meter > 0)
            {
                Meter = 0;
                OnMeterChanged?.Invoke(Meter, Multiplier, false);
                return;
            }

            if (Multiplier > minMultiplier)
            {
                Multiplier--;
                OnMeterChanged?.Invoke(Meter, Multiplier, false);
                OnMultiplierChanged?.Invoke(Multiplier);
            }
        }

    }
}