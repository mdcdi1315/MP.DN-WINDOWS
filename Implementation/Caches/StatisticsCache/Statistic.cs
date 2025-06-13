

namespace MP.Caches.StatisticsCache
{
    public sealed class Statistic
    {
        private System.String statname;
        private System.Object statvalue;
        private StatisticType type;

        public Statistic(System.String name , System.Object value)
        {
            if (System.String.IsNullOrEmpty(name)) 
            {
                throw new System.ArgumentNullException(nameof(name));
            }
            if (value is null)
            {
                throw new System.ArgumentNullException(nameof(value));
            }
            statname = name;
            ValidateType(value);
            statvalue = value;
        }

        private void ValidateType(System.Object obj)
        {
            switch (obj)
            {
                case null:
                    type = StatisticType.Null;
                    break;
                case System.Int64:
                    type = StatisticType.LongNumber;
                    break;
                case System.UInt64:
                    type = StatisticType.UnsignedLongNumber;
                    break;
                case System.String:
                    type = StatisticType.String;
                    break;
                case System.DateTime:
                    type = StatisticType.DateTime;
                    break;
                case System.TimeSpan:
                    type = StatisticType.TimeSpan;
                    break;
                default:
                    throw new System.ArgumentException("Invalid value for updating the statistic.");
            }
        }

        public System.String Name => statname;

        public System.Object Value
        {
            get => statvalue;
            set {
                ValidateType(value);
                statvalue = value;
            }
        }

        public StatisticType TypeOfValue => type;

        public void IncrementNumericValue()
        {
            switch (type)
            {
                case StatisticType.LongNumber:
                    statvalue = (System.Int64)statvalue + 1;
                    break;
                case StatisticType.UnsignedLongNumber:
                    statvalue = (System.UInt64)statvalue + 1;
                    break;
            }
        }

        public void DecrementNumericValue() 
        {
            switch (type)
            {
                case StatisticType.LongNumber:
                    statvalue = (System.Int64)statvalue - 1;
                    break;
                case StatisticType.UnsignedLongNumber:
                    statvalue = (System.UInt64)statvalue - 1;
                    break;
            }
        }

    }
}